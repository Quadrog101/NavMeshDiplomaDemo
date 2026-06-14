using UnityEngine;
using UnityEngine.AI;

namespace NavMeshDiplomaDemo
{
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class NavMeshDemoAgent : MonoBehaviour
    {
        [SerializeField] private float destinationEpsilon = 0.55f;

        private NavMeshAgent agent;
        private NavMeshDemoFinishZone finishZone;
        private Vector3 destination;
        private bool isRunning;
        private bool hasReachedTarget;
        private float startTime;

        public NavMeshAgent Agent => agent;
        public float PathLength { get; private set; }
        public float TravelTime { get; private set; }
        public int RepathCount { get; private set; }
        public bool HasReachedTarget => hasReachedTarget;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            destination = transform.position;
        }

        private void Update()
        {
            if (!isRunning || hasReachedTarget || agent.pathPending)
            {
                return;
            }

            PathLength = CalculatePathLength(agent.path);

            if (HasArrived())
            {
                hasReachedTarget = true;
                isRunning = false;
                TravelTime = Time.time - startTime;
                agent.isStopped = true;
            }
        }

        public void SetFinish(NavMeshDemoFinishZone zone, Vector3 finishPoint)
        {
            finishZone = zone;
            destination = finishPoint;
        }

        public void Begin()
        {
            if (finishZone == null)
            {
                return;
            }

            isRunning = true;
            hasReachedTarget = false;
            startTime = Time.time;
            TravelTime = 0f;
            SetDestination(destination, true);
        }

        public void ResetAgent(Vector3 position, Quaternion rotation)
        {
            isRunning = false;
            hasReachedTarget = false;
            TravelTime = 0f;
            PathLength = 0f;
            RepathCount = 0;

            agent.ResetPath();
            agent.Warp(position);
            transform.rotation = rotation;
            agent.isStopped = true;
        }

        public void RecalculatePath()
        {
            if (finishZone == null)
            {
                return;
            }

            bool shouldKeepMoving = isRunning;
            SetDestination(destination, false);
            agent.isStopped = !shouldKeepMoving;
        }

        private void SetDestination(Vector3 nextDestination, bool initialPath)
        {
            agent.isStopped = false;
            agent.SetDestination(nextDestination);

            if (!initialPath && isRunning)
            {
                RepathCount++;
            }
        }

        private bool HasArrived()
        {
            if (finishZone != null && finishZone.Contains(transform.position))
            {
                return true;
            }

            if (agent.remainingDistance <= Mathf.Max(destinationEpsilon, agent.stoppingDistance))
            {
                return true;
            }

            return Vector3.Distance(transform.position, destination) <= destinationEpsilon;
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
