using UnityEngine;
using UnityEngine.AI;

namespace NavMeshDiplomaDemo
{
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class NavMeshDemoAgent : MonoBehaviour
    {
        [SerializeField] private float destinationEpsilon = 0.35f;

        private NavMeshAgent agent;
        private Vector3 startPosition;
        private Quaternion startRotation;
        private Transform target;
        private bool isRunning;
        private bool hasReachedTarget;
        private float startTime;
        private Vector3 lastDestination;

        public NavMeshAgent Agent => agent;
        public float PathLength { get; private set; }
        public float TravelTime { get; private set; }
        public int RepathCount { get; private set; }
        public bool HasReachedTarget => hasReachedTarget;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            startPosition = transform.position;
            startRotation = transform.rotation;
            lastDestination = transform.position;
        }

        private void Update()
        {
            if (!isRunning || hasReachedTarget || agent.pathPending)
            {
                return;
            }

            // Метрика длины берется из фактических углов текущего NavMeshPath.
            PathLength = CalculatePathLength(agent.path);

            if (agent.remainingDistance <= Mathf.Max(destinationEpsilon, agent.stoppingDistance))
            {
                hasReachedTarget = true;
                isRunning = false;
                TravelTime = Time.time - startTime;
                agent.isStopped = true;
            }
        }

        public void SetTarget(Transform nextTarget)
        {
            target = nextTarget;
            if (target != null)
            {
                lastDestination = target.position;
            }
        }

        public void Begin()
        {
            if (target == null)
            {
                return;
            }

            isRunning = true;
            hasReachedTarget = false;
            startTime = Time.time;
            TravelTime = 0f;
            SetDestination(target.position, true);
        }

        public void ResetAgent(Vector3 position, Quaternion rotation)
        {
            isRunning = false;
            hasReachedTarget = false;
            TravelTime = 0f;
            PathLength = 0f;
            RepathCount = 0;
            lastDestination = target != null ? target.position : position;

            agent.ResetPath();
            agent.Warp(position);
            transform.rotation = rotation;
            agent.isStopped = true;
        }

        public void RecalculatePath()
        {
            if (target == null)
            {
                return;
            }

            bool shouldKeepMoving = isRunning;
            SetDestination(target.position, false);
            agent.isStopped = !shouldKeepMoving;
        }

        private void SetDestination(Vector3 destination, bool initialPath)
        {
            agent.isStopped = false;
            agent.SetDestination(destination);
            lastDestination = destination;

            // Перестроения считаем только во время эксперимента, а не при предпросмотре пути.
            if (!initialPath && isRunning)
            {
                RepathCount++;
            }
        }

        private static float CalculatePathLength(NavMeshPath path)
        {
            if (path == null || path.corners.Length < 2)
            {
                return 0f;
            }

            float length = 0f;
            for (int i = 1; i < path.corners.Length; i++)
            {
                length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }

            return length;
        }
    }
}
