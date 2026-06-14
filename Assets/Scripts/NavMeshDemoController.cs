using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace NavMeshDiplomaDemo
{
    public enum NavMeshDemoMode
    {
        ShortestPath,
        WeightedCost,
        DynamicObstacle,
        MultiAgent
    }

    public sealed class NavMeshDemoController : MonoBehaviour
    {
        private const int ExpensiveArea = 3;

        [Header("Scene Links")]
        [SerializeField] private NavMeshDemoFinishZone finishZone;
        [SerializeField] private NavMeshSurface navMeshSurface;
        [SerializeField] private GameObject dynamicObstacle;
        [SerializeField] private List<NavMeshDemoAgent> agents = new();

        [Header("Experiment Settings")]
        [SerializeField] private float normalAreaCost = 1f;
        [SerializeField] private float expensiveAreaCost = 18f;
        [SerializeField] private bool expensiveCostEnabled;
        [SerializeField] private bool obstacleEnabled;
        [SerializeField] private bool multipleAgentsEnabled;
        [SerializeField] private NavMeshDemoMode mode = NavMeshDemoMode.ShortestPath;

        private readonly Vector3[] spawnOffsets =
        {
            new(0f, 0f, 0f),
            new(-0.7f, 0f, 1.15f),
            new(-0.7f, 0f, -1.15f),
            new(-1.4f, 0f, 0f)
        };

        private Vector3 baseSpawnPosition;
        private Quaternion baseSpawnRotation;
        private float fpsTimer;
        private int fpsFrames;
        private float fps;
        private bool initialized;
        private Coroutine delayedRepathRoutine;

        private int ActiveAgentCount
        {
            get
            {
                int count = 0;
                foreach (NavMeshDemoAgent demoAgent in agents)
                {
                    if (demoAgent.gameObject.activeSelf)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        private int ReachedAgentCount
        {
            get
            {
                int count = 0;
                foreach (NavMeshDemoAgent demoAgent in agents)
                {
                    if (demoAgent.gameObject.activeSelf && demoAgent.HasReachedTarget)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        private void Start()
        {
            InitializeIfNeeded();
            ApplyMode(NavMeshDemoMode.ShortestPath);
            ResetDemo();
        }

        private void Update()
        {
            InitializeIfNeeded();
            UpdateFps();
            HandleInput();
        }

        private void InitializeIfNeeded()
        {
            if (initialized)
            {
                return;
            }

            if (agents.Count > 0)
            {
                baseSpawnPosition = agents[0].transform.position;
                baseSpawnRotation = agents[0].transform.rotation;
            }

            AssignFinishPoints();
            initialized = true;
        }

        private void OnGUI()
        {
            const float hudWidth = 360f;
            const float hudHeight = 315f;
            float hudX = Mathf.Max(12f, Screen.width - hudWidth - 12f);
            GUI.Box(new Rect(hudX, 12, hudWidth, hudHeight), GUIContent.none);
            GUILayout.BeginArea(new Rect(hudX + 10f, 18, hudWidth - 20f, hudHeight - 12f));

            GUILayout.Label("NavMesh Experiment");
            GUILayout.Label($"Mode: {GetModeTitle()}");
            GUILayout.Label($"Expected route: {GetExpectedRoute()}");
            GUILayout.Label($"Actual route: {GetActualRoute()}");
            GUILayout.Label($"Path status: {GetPathStatus()}");
            GUILayout.Label($"Repaths: {GetRepathCount()}");
            GUILayout.Label($"Agents: {ActiveAgentCount} active / {ReachedAgentCount} reached");
            GUILayout.Label($"Path length: {AveragePathLength():0.00} m");
            GUILayout.Label($"Travel time: {AverageTravelTimeText()}");
            GUILayout.Label($"Expensive cost: {CurrentExpensiveCost():0.0}");
            GUILayout.Label($"Obstacle: {(obstacleEnabled ? "ON" : "OFF")}   FPS: {fps:0}");
            GUILayout.Label($"Conclusion: {GetConclusion()}");
            GUILayout.Space(4);
            GUILayout.Label("1 shortest | 2 weighted | 3 obstacle | 4 multi-agent");
            GUILayout.Label("Space start | R reset | C cost | O obstacle | N agents");

            GUILayout.EndArea();
        }

        private void HandleInput()
        {
            if (WasPressed(KeyCode.Alpha1, "Digit1"))
            {
                ApplyModeAndReset(NavMeshDemoMode.ShortestPath);
            }

            if (WasPressed(KeyCode.Alpha2, "Digit2"))
            {
                ApplyModeAndReset(NavMeshDemoMode.WeightedCost);
            }

            if (WasPressed(KeyCode.Alpha3, "Digit3"))
            {
                ApplyModeAndReset(NavMeshDemoMode.DynamicObstacle);
            }

            if (WasPressed(KeyCode.Alpha4, "Digit4"))
            {
                ApplyModeAndReset(NavMeshDemoMode.MultiAgent);
            }

            if (WasPressed(KeyCode.M, "M"))
            {
                CycleMode();
            }

            if (WasPressed(KeyCode.Space, "Space"))
            {
                StartMovement();
            }

            if (WasPressed(KeyCode.R, "R"))
            {
                ResetDemo();
            }

            if (WasPressed(KeyCode.C, "C"))
            {
                ToggleCostMode();
            }

            if (WasPressed(KeyCode.O, "O"))
            {
                ToggleObstacle();
            }

            if (WasPressed(KeyCode.N, "N"))
            {
                ToggleNpcCount();
            }
        }

        private static bool WasPressed(KeyCode legacyKey, string inputSystemKeyName)
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null
                && System.Enum.TryParse(inputSystemKeyName, out Key inputSystemKey)
                && Keyboard.current[inputSystemKey].wasPressedThisFrame)
            {
                return true;
            }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            return Input.GetKeyDown(legacyKey);
#else
            return false;
#endif
        }

        public void CycleMode()
        {
            int next = ((int)mode + 1) % System.Enum.GetValues(typeof(NavMeshDemoMode)).Length;
            ApplyModeAndReset((NavMeshDemoMode)next);
        }

        public void ApplyModeAndReset(NavMeshDemoMode nextMode)
        {
            ApplyMode(nextMode);
            ResetDemo();
        }

        public void StartMovement()
        {
            InitializeIfNeeded();
            RecalculateAllPaths();

            foreach (NavMeshDemoAgent demoAgent in agents)
            {
                if (demoAgent.gameObject.activeSelf)
                {
                    demoAgent.Begin();
                }
            }
        }

        public void ResetDemo()
        {
            InitializeIfNeeded();
            AssignFinishPoints();

            for (int i = 0; i < agents.Count; i++)
            {
                bool shouldBeActive = i == 0 || multipleAgentsEnabled;
                Vector3 spawn = baseSpawnPosition + spawnOffsets[Mathf.Min(i, spawnOffsets.Length - 1)];

                if (shouldBeActive && !agents[i].gameObject.activeSelf)
                {
                    agents[i].gameObject.SetActive(true);
                }

                if (agents[i].gameObject.activeSelf)
                {
                    agents[i].ResetAgent(spawn, baseSpawnRotation);
                }

                if (!shouldBeActive)
                {
                    agents[i].gameObject.SetActive(false);
                }
            }

            RecalculateAllPaths();
        }

        public void ToggleCostMode()
        {
            expensiveCostEnabled = !expensiveCostEnabled;
            mode = expensiveCostEnabled ? NavMeshDemoMode.WeightedCost : NavMeshDemoMode.ShortestPath;
            ApplyCurrentSettings();
            ResetDemo();
            RecalculateAllPaths();
        }

        public void ToggleObstacle()
        {
            obstacleEnabled = !obstacleEnabled;
            if (obstacleEnabled)
            {
                mode = NavMeshDemoMode.DynamicObstacle;
            }
            else if (mode == NavMeshDemoMode.DynamicObstacle)
            {
                mode = NavMeshDemoMode.ShortestPath;
            }

            ApplyCurrentSettings();
            ResetDemo();
        }

        public void ToggleNpcCount()
        {
            multipleAgentsEnabled = !multipleAgentsEnabled;
            if (multipleAgentsEnabled)
            {
                mode = NavMeshDemoMode.MultiAgent;
            }

            ApplyCurrentSettings();
            ResetDemo();
        }

        public void ApplyMode(NavMeshDemoMode nextMode)
        {
            InitializeIfNeeded();

            mode = nextMode;
            expensiveCostEnabled = mode == NavMeshDemoMode.WeightedCost || mode == NavMeshDemoMode.MultiAgent;
            obstacleEnabled = mode == NavMeshDemoMode.DynamicObstacle;
            multipleAgentsEnabled = mode == NavMeshDemoMode.MultiAgent;
            ApplyCurrentSettings();
        }

        private void ApplyCurrentSettings()
        {
            NavMesh.SetAreaCost(ExpensiveArea, CurrentExpensiveCost());

            if (dynamicObstacle != null)
            {
                dynamicObstacle.SetActive(obstacleEnabled);
            }

            if (obstacleEnabled)
            {
                ScheduleDelayedRepath();
            }
        }

        private void ScheduleDelayedRepath()
        {
            if (delayedRepathRoutine != null)
            {
                StopCoroutine(delayedRepathRoutine);
            }

            delayedRepathRoutine = StartCoroutine(RepathAfterObstacleCarving());
        }

        private IEnumerator RepathAfterObstacleCarving()
        {
            yield return null;
            RecalculateAllPaths();
            delayedRepathRoutine = null;
        }

        private void AssignFinishPoints()
        {
            if (finishZone == null)
            {
                return;
            }

            int finishPointCount = multipleAgentsEnabled ? agents.Count : 1;

            for (int i = 0; i < agents.Count; i++)
            {
                int finishPointIndex = multipleAgentsEnabled ? i : 0;
                Vector3 finishPoint = finishZone.GetDestinationPoint(finishPointIndex, finishPointCount);
                agents[i].SetFinish(finishZone, finishPoint);
            }
        }

        private void RecalculateAllPaths()
        {
            foreach (NavMeshDemoAgent demoAgent in agents)
            {
                if (demoAgent.gameObject.activeSelf)
                {
                    demoAgent.RecalculatePath();
                }
            }
        }

        private void UpdateFps()
        {
            fpsTimer += Time.unscaledDeltaTime;
            fpsFrames++;

            if (fpsTimer >= 0.5f)
            {
                fps = fpsFrames / fpsTimer;
                fpsTimer = 0f;
                fpsFrames = 0;
            }
        }

        private float CurrentExpensiveCost()
        {
            return expensiveCostEnabled ? expensiveAreaCost : normalAreaCost;
        }

        private float AveragePathLength()
        {
            int count = 0;
            float total = 0f;

            foreach (NavMeshDemoAgent demoAgent in agents)
            {
                if (demoAgent.gameObject.activeSelf)
                {
                    total += demoAgent.PathLength;
                    count++;
                }
            }

            return count == 0 ? 0f : total / count;
        }

        private string AverageTravelTimeText()
        {
            int count = 0;
            float total = 0f;

            foreach (NavMeshDemoAgent demoAgent in agents)
            {
                if (demoAgent.gameObject.activeSelf && demoAgent.HasReachedTarget)
                {
                    total += demoAgent.TravelTime;
                    count++;
                }
            }

            return count == 0 ? "-" : $"{total / count:0.00} s";
        }

        private string GetModeTitle()
        {
            return mode switch
            {
                NavMeshDemoMode.ShortestPath => "1 Shortest path",
                NavMeshDemoMode.WeightedCost => "2 Weighted cost",
                NavMeshDemoMode.DynamicObstacle => "3 Dynamic obstacle",
                NavMeshDemoMode.MultiAgent => "4 Multi-agent",
                _ => mode.ToString()
            };
        }

        private string GetExpectedRoute()
        {
            return mode switch
            {
                NavMeshDemoMode.ShortestPath => "cost=1 -> central shortcut",
                NavMeshDemoMode.WeightedCost => "cost=18 -> long bypass",
                NavMeshDemoMode.DynamicObstacle => "bypass (shortcut blocked)",
                NavMeshDemoMode.MultiAgent => "several finish points",
                _ => "-"
            };
        }

        private string GetActualRoute()
        {
            NavMeshPath path = FirstActivePath();
            if (path == null || path.corners.Length < 2)
            {
                return "not started";
            }

            float maxAbsZ = 0f;
            for (int i = 0; i < path.corners.Length; i++)
            {
                maxAbsZ = Mathf.Max(maxAbsZ, Mathf.Abs(path.corners[i].z));
            }

            if (obstacleEnabled)
            {
                return maxAbsZ > 2.4f ? "bypass around obstacle" : "shortcut blocked";
            }

            return maxAbsZ > 2.4f ? "long bypass" : "central shortcut";
        }

        private NavMeshPath FirstActivePath()
        {
            NavMeshDemoAgent demoAgent = FirstActiveAgent();
            return demoAgent != null ? demoAgent.Agent.path : null;
        }

        private NavMeshDemoAgent FirstActiveAgent()
        {
            foreach (NavMeshDemoAgent demoAgent in agents)
            {
                if (demoAgent.gameObject.activeSelf && demoAgent.Agent != null)
                {
                    return demoAgent;
                }
            }

            return null;
        }

        private string GetPathStatus()
        {
            NavMeshPath path = FirstActivePath();
            if (path == null || path.corners.Length == 0)
            {
                return "Invalid";
            }

            return path.status switch
            {
                NavMeshPathStatus.PathComplete => "Complete",
                NavMeshPathStatus.PathPartial => "Partial",
                NavMeshPathStatus.PathInvalid => "Invalid",
                _ => path.status.ToString()
            };
        }

        private int GetRepathCount()
        {
            NavMeshDemoAgent demoAgent = FirstActiveAgent();
            return demoAgent != null ? demoAgent.RepathCount : 0;
        }

        private string GetConclusion()
        {
            return mode switch
            {
                NavMeshDemoMode.ShortestPath => "equal costs prefer distance",
                NavMeshDemoMode.WeightedCost => "area cost changes route choice",
                NavMeshDemoMode.DynamicObstacle => "carving blocks the shortcut",
                NavMeshDemoMode.MultiAgent => "wide finish accepts all agents",
                _ => "-"
            };
        }
    }
}
