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
        private const int RiverArea = 9;

        [SerializeField] private float destinationEpsilon = 0.65f;

        private NavMeshAgent agent;
        private NavMeshDemoFinishZone finishZone;
        private Vector3 destination;
        private bool isRunning;
        private bool hasReachedTarget;
        private float startTime;

        public NavMeshAgent Agent => agent;
        public string ProfileName { get; private set; } = "Scout";
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
                SetAreaCosts(1f, 1f, 1f, 1.1f, 1f, 1f, 1f);
                return;
            }

            switch (ProfileName)
            {
                case "Scout":
                    SetAreaCosts(18f, 1.6f, 3.2f, 1.8f, 1.1f, 1.0f, 1.5f);
                    break;
                case "Carrier":
                    SetAreaCosts(18f, 2.8f, 7f, 5.5f, 1.4f, 1.0f, 3.0f);
                    break;
                case "Nomad":
                    SetAreaCosts(12f, 1.4f, 2.4f, 3.0f, 1.1f, 1.2f, 1.8f);
                    break;
                default:
                    SetAreaCosts(18f, 2.0f, 4f, 3f, 1.2f, 1.0f, 2.2f);
                    break;
            }
        }

        private void SetAreaCosts(
            float hazardCost,
            float packedSandCost,
            float deepSandCost,
            float rockCost,
            float oasisCost,
            float roadCost,
            float riverCost)
        {
            agent.SetAreaCost(HazardArea, hazardCost);
            agent.SetAreaCost(PackedSandArea, packedSandCost);
            agent.SetAreaCost(DeepSandArea, deepSandCost);
            agent.SetAreaCost(RockArea, rockCost);
            agent.SetAreaCost(OasisArea, oasisCost);
            agent.SetAreaCost(RoadArea, roadCost);
            agent.SetAreaCost(RiverArea, riverCost);
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
            float maxX = float.MinValue;
            for (int i = 0; i < agent.path.corners.Length; i++)
            {
                maxZ = Mathf.Max(maxZ, agent.path.corners[i].z);
                minZ = Mathf.Min(minZ, agent.path.corners[i].z);
                maxX = Mathf.Max(maxX, agent.path.corners[i].x);
            }

            if (maxZ > 5.3f)
            {
                return "North Road";
            }

            if (minZ < -5.0f)
            {
                return "South Oasis";
            }

            if (maxZ > 2.5f && maxX > 4.5f)
            {
                return "Mountains/Canyon";
            }

            if (Mathf.Abs(maxZ) < 3.2f && Mathf.Abs(minZ) < 3.2f)
            {
                return "Central Desert";
            }

            return "mixed route";
        }

        public RouteResourceMetrics EstimateResources()
        {
            string route = DescribeRoute();
            float distance = Mathf.Max(0f, PathLength);
            RouteResourceMetrics metrics = new()
            {
                Distance = distance
            };

            float waterRate = 0.30f;
            float foodRate = 0.16f;
            float staminaRate = 0.22f;
            float riskRate = 0.08f;

            switch (route)
            {
                case "Central Desert":
                    waterRate = 0.62f;
                    foodRate = 0.18f;
                    staminaRate = 0.34f;
                    riskRate = 0.28f;
                    break;
                case "South Oasis":
                    waterRate = 0.18f;
                    foodRate = 0.14f;
                    staminaRate = 0.22f;
                    riskRate = 0.06f;
                    metrics.OasisBonus = distance * 0.18f;
                    break;
                case "Mountains/Canyon":
                    waterRate = 0.26f;
                    foodRate = 0.24f;
                    staminaRate = 0.55f;
                    riskRate = 0.12f;
                    break;
                case "North Road":
                    waterRate = 0.24f;
                    foodRate = 0.13f;
                    staminaRate = 0.16f;
                    riskRate = 0.04f;
                    break;
            }

            ApplyProfileResourceBias(ref waterRate, ref foodRate, ref staminaRate, ref riskRate, ref metrics);

            metrics.Water = Mathf.Max(0f, distance * waterRate - metrics.OasisBonus);
            metrics.Food = distance * foodRate;
            metrics.Stamina = distance * staminaRate;
            metrics.Risk = distance * riskRate;
            return metrics;
        }

        public string GetProfileSummary()
        {
            return ProfileName switch
            {
                "Scout" => "Scout: горы/ущелья ок",
                "Carrier" => "Carrier: любит дорогу",
                "Nomad" => "Nomad: пустыня/оазис",
                _ => $"{ProfileName}: balanced"
            };
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

        private void ApplyProfileResourceBias(
            ref float waterRate,
            ref float foodRate,
            ref float staminaRate,
            ref float riskRate,
            ref RouteResourceMetrics metrics)
        {
            switch (ProfileName)
            {
                case "Scout":
                    staminaRate *= 0.78f;
                    riskRate *= 0.90f;
                    break;
                case "Carrier":
                    waterRate *= 1.18f;
                    staminaRate *= 1.28f;
                    foodRate *= 1.10f;
                    break;
                case "Nomad":
                    waterRate *= 0.72f;
                    staminaRate *= 0.92f;
                    metrics.OasisBonus *= 1.35f;
                    break;
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

    public struct RouteResourceMetrics
    {
        public float Distance;
        public float Water;
        public float Food;
        public float Stamina;
        public float Risk;
        public float OasisBonus;
    }
}
