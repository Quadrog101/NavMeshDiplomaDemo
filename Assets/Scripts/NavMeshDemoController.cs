using System.Collections.Generic;
using System.Text;
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
        CostAware,
        DynamicObstacle,
        MultipleAgents
    }

    public sealed class NavMeshDemoController : MonoBehaviour
    {
        private const int ExpensiveArea = 3;

        [Header("Scene Links")]
        [SerializeField] private Transform target;
        [SerializeField] private NavMeshSurface navMeshSurface;
        [SerializeField] private GameObject dynamicObstacle;
        [SerializeField] private List<NavMeshDemoAgent> agents = new();

        [Header("Experiment Settings")]
        [SerializeField] private float normalAreaCost = 1f;
        [SerializeField] private float expensiveAreaCost = 8f;
        [SerializeField] private bool expensiveCostEnabled;
        [SerializeField] private bool obstacleEnabled;
        [SerializeField] private bool multipleAgentsEnabled;
        [SerializeField] private NavMeshDemoMode mode = NavMeshDemoMode.ShortestPath;

        private readonly Vector3[] spawnOffsets =
        {
            new(0f, 0f, 0f),
            new(-0.75f, 0f, 1.1f),
            new(-0.75f, 0f, -1.1f),
            new(-1.5f, 0f, 2.2f)
        };

        private Vector3 baseSpawnPosition;
        private Quaternion baseSpawnRotation;
        private float fpsTimer;
        private int fpsFrames;
        private float fps;
        private bool initialized;

        private void Awake()
        {
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

            foreach (NavMeshDemoAgent demoAgent in agents)
            {
                demoAgent.SetTarget(target);
            }

            initialized = true;
        }

        private void OnGUI()
        {
            const int width = 460;
            GUILayout.BeginArea(new Rect(16, 16, width, Screen.height - 32), GUI.skin.box);

            GUILayout.Label("NavMesh Diploma Demo");
            GUILayout.Space(4);
            GUILayout.Label("Space - старт | R - сброс | C - стоимость | O - препятствие | N - NPC");
            GUILayout.Space(8);
            GUILayout.Label($"Режим: {GetModeTitle()}");
            GUILayout.Label($"Стоимость Expensive: {(expensiveCostEnabled ? expensiveAreaCost : normalAreaCost):0.0}");
            GUILayout.Label($"Динамическое препятствие: {(obstacleEnabled ? "включено" : "выключено")}");
            GUILayout.Label($"Активных агентов: {ActiveAgentCount}");
            GUILayout.Label($"FPS: {fps:0}");
            GUILayout.Space(8);

            StringBuilder builder = new();
            for (int i = 0; i < agents.Count; i++)
            {
                if (!agents[i].gameObject.activeSelf)
                {
                    continue;
                }

                builder.Append("NPC ");
                builder.Append(i + 1);
                builder.Append(": путь ");
                builder.Append(agents[i].PathLength.ToString("0.00"));
                builder.Append(" м, время ");
                builder.Append(agents[i].TravelTime > 0f ? agents[i].TravelTime.ToString("0.00") : "-");
                builder.Append(" c, перестроений ");
                builder.AppendLine(agents[i].RepathCount.ToString());
            }

            GUILayout.Label(builder.ToString());
            GUILayout.EndArea();
        }

        private void HandleInput()
        {
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

        public void StartMovement()
        {
            InitializeIfNeeded();

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
            mode = expensiveCostEnabled ? NavMeshDemoMode.CostAware : NavMeshDemoMode.ShortestPath;
            ApplyCurrentSettings();
        }

        public void ToggleObstacle()
        {
            obstacleEnabled = !obstacleEnabled;
            mode = obstacleEnabled ? NavMeshDemoMode.DynamicObstacle : mode;
            ApplyCurrentSettings();
        }

        public void ToggleNpcCount()
        {
            multipleAgentsEnabled = !multipleAgentsEnabled;
            mode = multipleAgentsEnabled ? NavMeshDemoMode.MultipleAgents : mode;
            ApplyCurrentSettings();
            ResetDemo();
        }

        public void ApplyMode(NavMeshDemoMode nextMode)
        {
            InitializeIfNeeded();

            mode = nextMode;
            expensiveCostEnabled = mode == NavMeshDemoMode.CostAware || mode == NavMeshDemoMode.MultipleAgents;
            obstacleEnabled = mode == NavMeshDemoMode.DynamicObstacle;
            multipleAgentsEnabled = mode == NavMeshDemoMode.MultipleAgents;
            ApplyCurrentSettings();
        }

        private void ApplyCurrentSettings()
        {
            // Одна и та же геометрия показывает разницу между кратчайшим и стоимостным маршрутом.
            NavMesh.SetAreaCost(ExpensiveArea, expensiveCostEnabled ? expensiveAreaCost : normalAreaCost);

            if (dynamicObstacle != null)
            {
                dynamicObstacle.SetActive(obstacleEnabled);
            }

            RecalculateAllPaths();
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

        private string GetModeTitle()
        {
            return mode switch
            {
                NavMeshDemoMode.ShortestPath => "кратчайший путь",
                NavMeshDemoMode.CostAware => "учет стоимости зон",
                NavMeshDemoMode.DynamicObstacle => "динамическое препятствие",
                NavMeshDemoMode.MultipleAgents => "несколько NPC",
                _ => mode.ToString()
            };
        }

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
    }
}
