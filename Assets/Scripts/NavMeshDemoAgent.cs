using UnityEngine;
using UnityEngine.AI;

namespace NavMeshDiplomaDemo
{
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class NavMeshDemoAgent : MonoBehaviour
    {
        private const int HazardArea = 3;
        private const int PackedSandArea = 4;
        private const int DeepSandArea = 5;
        private const int RockArea = 6;
        private const int OasisArea = 7;
        private const int RoadArea = 8;

        [SerializeField] private float destinationEpsilon = 0.65f;

        private NavMeshAgent agent;
        private NavMeshDemoFinishZone finishZone;
        private Vector3 destination;
        private bool isRunning;
        private bool hasReachedTarget;
        private float startTime;

        public NavMeshAgent Agent => agent;
        public string ProfileName { get; private set; } = "Ranger";
        public float PathLength { get; private set; }
        public float TravelTime { get; private set; }
        public int RepathCount { get; private set; }
        public bool HasReachedTarget => hasReachedTarget;

        private void Awake()
        {
            EnsureAgent();
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

        public void ConfigureProfile(
            string profileName,
            float speed,
            float acceleration,
            float radius)
        {
            EnsureAgent();
            ProfileName = profileName;
            agent.speed = speed;
            agent.acceleration = acceleration;
            agent.radius = radius;
            ApplyProfileAreaCosts(true);
        }

        public void ApplyProfileAreaCosts(bool weighted)
        {
            EnsureAgent();

            if (!weighted)
            {
                SetAreaCosts(1f, 1f, 1f, 1.1f, 1f, 1f);
                return;
            }

            switch (ProfileName)
            {
                case "Scout":
                    SetAreaCosts(4f, 1.7f, 2.4f, 3f, 1.1f, 1.0f);
                    break;
                case "Carrier":
                    SetAreaCosts(10f, 2.6f, 7f, 5f, 1.4f, 0.8f);
                    break;
                default:
                    SetAreaCosts(7f, 2.0f, 4f, 3f, 1.2f, 1.0f);
                    break;
            }
        }

        private void SetAreaCosts(
            float hazardCost,
            float packedSandCost,
            float deepSandCost,
            float rockCost,
            float oasisCost,
            float roadCost)
        {
            agent.SetAreaCost(HazardArea, hazardCost);
            agent.SetAreaCost(PackedSandArea, packedSandCost);
            agent.SetAreaCost(DeepSandArea, deepSandCost);
            agent.SetAreaCost(RockArea, rockCost);
            agent.SetAreaCost(OasisArea, oasisCost);
            agent.SetAreaCost(RoadArea, roadCost);
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
            EnsureAgent();
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

        public string DescribeRoute()
        {
            if (agent == null || agent.path == null || agent.path.corners.Length < 2)
            {
                return "not started";
            }

            float maxZ = float.MinValue;
            float minZ = float.MaxValue;
            for (int i = 0; i < agent.path.corners.Length; i++)
            {
                maxZ = Mathf.Max(maxZ, agent.path.corners[i].z);
                minZ = Mathf.Min(minZ, agent.path.corners[i].z);
            }

            if (maxZ > 5.3f)
            {
                return "North Road";
            }

            if (minZ < -5.0f)
            {
                return "South Oasis";
            }

            if (Mathf.Abs(maxZ) < 3.2f && Mathf.Abs(minZ) < 3.2f)
            {
                return "Central Dunes";
            }

            return "mixed route";
        }

        private void SetDestination(Vector3 nextDestination, bool initialPath)
        {
            EnsureAgent();
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

        private void EnsureAgent()
        {
            if (agent == null)
            {
                agent = GetComponent<NavMeshAgent>();
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
