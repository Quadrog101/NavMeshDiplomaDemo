using System.Collections.Generic;
using System.Reflection;
using NavMeshDiplomaDemo;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace NavMeshDiplomaDemo.Editor
{
    public static class NavMeshDemoSceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/NavMeshDemo.unity";
        private const string MaterialFolder = "Assets/Materials";
        private const int ExpensiveArea = 3;

        [MenuItem("Tools/NavMesh Diploma Demo/Rebuild Scene")]
        public static void RebuildScene()
        {
            EnsureProjectFolders();

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "NavMeshDemo";

            Material floorMaterial = GetOrCreateMaterial("M_Floor", new Color(0.72f, 0.75f, 0.70f));
            Material wallMaterial = GetOrCreateMaterial("M_Wall", new Color(0.28f, 0.31f, 0.34f));
            Material expensiveMaterial = GetOrCreateMaterial("M_ExpensiveZone", new Color(0.95f, 0.62f, 0.25f, 0.45f));
            Material obstacleMaterial = GetOrCreateMaterial("M_DynamicObstacle", new Color(0.83f, 0.22f, 0.18f));
            Material agentMaterial = GetOrCreateMaterial("M_Agent", new Color(0.18f, 0.46f, 0.86f));
            Material targetMaterial = GetOrCreateMaterial("M_Target", new Color(0.18f, 0.72f, 0.34f));
            Material pathMaterial = GetOrCreateMaterial("M_PathLine", new Color(0.08f, 0.19f, 0.28f));

            GameObject root = new("NavMesh Demo");
            GameObject geometryRoot = new("Geometry");
            geometryRoot.transform.SetParent(root.transform);

            GameObject floor = CreateCube("Floor", new Vector3(0f, -0.05f, 0f), new Vector3(24f, 0.1f, 18f), floorMaterial, geometryRoot.transform);
            GameObjectUtility.SetStaticEditorFlags(floor, StaticEditorFlags.NavigationStatic);

            CreateWall("North Border", new Vector3(0f, 0.55f, 9f), new Vector3(24f, 1.1f, 0.35f), wallMaterial, geometryRoot.transform);
            CreateWall("South Border", new Vector3(0f, 0.55f, -9f), new Vector3(24f, 1.1f, 0.35f), wallMaterial, geometryRoot.transform);
            CreateWall("West Border", new Vector3(-12f, 0.55f, 0f), new Vector3(0.35f, 1.1f, 18f), wallMaterial, geometryRoot.transform);
            CreateWall("East Border", new Vector3(12f, 0.55f, 0f), new Vector3(0.35f, 1.1f, 18f), wallMaterial, geometryRoot.transform);

            // A segmented barrier creates three choices: a short expensive gate and two longer bypasses.
            CreateWall("Barrier Upper A", new Vector3(0f, 0.55f, 6.75f), new Vector3(0.45f, 1.1f, 3.7f), wallMaterial, geometryRoot.transform);
            CreateWall("Barrier Upper B", new Vector3(0f, 0.55f, 2.35f), new Vector3(0.45f, 1.1f, 2.3f), wallMaterial, geometryRoot.transform);
            CreateWall("Barrier Lower A", new Vector3(0f, 0.55f, -2.35f), new Vector3(0.45f, 1.1f, 2.3f), wallMaterial, geometryRoot.transform);
            CreateWall("Barrier Lower B", new Vector3(0f, 0.55f, -6.75f), new Vector3(0.45f, 1.1f, 3.7f), wallMaterial, geometryRoot.transform);

            GameObject shortcutMarker = CreateCube("Expensive Cost Zone", new Vector3(0f, 0.02f, 0f), new Vector3(4.8f, 0.04f, 2.2f), expensiveMaterial, geometryRoot.transform);
            shortcutMarker.GetComponent<Collider>().enabled = false;
            NavMeshModifierVolume costVolume = shortcutMarker.AddComponent<NavMeshModifierVolume>();
            costVolume.area = ExpensiveArea;
            costVolume.center = Vector3.zero;
            costVolume.size = new Vector3(4.8f, 1.2f, 2.2f);

            CreateRouteLabel("Short expensive gate", new Vector3(0f, 0.06f, 0f), geometryRoot.transform);
            CreateRouteLabel("Long upper route", new Vector3(0f, 0.06f, 4.6f), geometryRoot.transform);
            CreateRouteLabel("Long lower route", new Vector3(0f, 0.06f, -4.6f), geometryRoot.transform);

            GameObject dynamicObstacle = CreateCube("Dynamic Obstacle (O)", new Vector3(0f, 0.55f, 0f), new Vector3(1.5f, 1.1f, 1.5f), obstacleMaterial, root.transform);
            NavMeshObstacle obstacle = dynamicObstacle.AddComponent<NavMeshObstacle>();
            obstacle.shape = NavMeshObstacleShape.Box;
            obstacle.size = new Vector3(1.5f, 1.1f, 1.5f);
            obstacle.center = Vector3.zero;
            obstacle.carving = true;
            dynamicObstacle.SetActive(false);

            GameObject startMarker = CreateCylinder("Start", new Vector3(-10f, 0.03f, 0f), new Vector3(1.2f, 0.06f, 1.2f), agentMaterial, root.transform);
            GameObject target = CreateCylinder("Target", new Vector3(10f, 0.03f, 0f), new Vector3(1.2f, 0.06f, 1.2f), targetMaterial, root.transform);

            List<NavMeshDemoAgent> agents = new();
            for (int i = 0; i < 4; i++)
            {
                GameObject agentObject = CreateAgent(i + 1, new Vector3(-10f, 1f, 0f), agentMaterial, pathMaterial, root.transform);
                agentObject.SetActive(i == 0);
                agents.Add(agentObject.GetComponent<NavMeshDemoAgent>());
            }

            GameObject navMeshObject = new("NavMesh Surface");
            navMeshObject.transform.SetParent(root.transform);
            NavMeshSurface surface = navMeshObject.AddComponent<NavMeshSurface>();
            surface.collectObjects = CollectObjects.All;
            surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            surface.BuildNavMesh();

            GameObject controllerObject = new("Demo Controller");
            controllerObject.transform.SetParent(root.transform);
            NavMeshDemoController controller = controllerObject.AddComponent<NavMeshDemoController>();
            SetPrivateField(controller, "target", target.transform);
            SetPrivateField(controller, "navMeshSurface", surface);
            SetPrivateField(controller, "dynamicObstacle", dynamicObstacle);
            SetPrivateField(controller, "agents", agents);

            CreateCamera();
            CreateLighting();

            Selection.activeObject = controllerObject;
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"NavMesh demo scene rebuilt: {ScenePath}");
        }

        private static void EnsureProjectFolders()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            {
                AssetDatabase.CreateFolder("Assets", "Scenes");
            }

            if (!AssetDatabase.IsValidFolder(MaterialFolder))
            {
                AssetDatabase.CreateFolder("Assets", "Materials");
            }
        }

        private static Material GetOrCreateMaterial(string name, Color color)
        {
            string path = $"{MaterialFolder}/{name}.mat";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
                AssetDatabase.CreateAsset(material, path);
            }

            material.color = color;
            EditorUtility.SetDirty(material);
            return material;
        }

        private static GameObject CreateAgent(int index, Vector3 position, Material agentMaterial, Material pathMaterial, Transform parent)
        {
            GameObject agentObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            agentObject.name = $"NPC {index}";
            agentObject.transform.SetParent(parent);
            agentObject.transform.position = position;
            agentObject.GetComponent<Renderer>().sharedMaterial = agentMaterial;

            NavMeshAgent agent = agentObject.AddComponent<NavMeshAgent>();
            agent.speed = 3.5f;
            agent.angularSpeed = 360f;
            agent.acceleration = 12f;
            agent.autoRepath = true;
            agent.avoidancePriority = 40 + index;

            NavMeshDemoAgent demoAgent = agentObject.AddComponent<NavMeshDemoAgent>();
            LineRenderer line = agentObject.AddComponent<LineRenderer>();
            line.sharedMaterial = pathMaterial;
            line.widthMultiplier = 0.08f;
            line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            line.receiveShadows = false;
            NavMeshDemoPathRenderer pathRenderer = agentObject.AddComponent<NavMeshDemoPathRenderer>();
            pathRenderer.Bind(agent);

            return agentObject;
        }

        private static GameObject CreateCube(string name, Vector3 position, Vector3 scale, Material material, Transform parent)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.SetParent(parent);
            cube.transform.position = position;
            cube.transform.localScale = scale;
            cube.GetComponent<Renderer>().sharedMaterial = material;
            return cube;
        }

        private static void CreateWall(string name, Vector3 position, Vector3 scale, Material material, Transform parent)
        {
            GameObject wall = CreateCube(name, position, scale, material, parent);
            GameObjectUtility.SetStaticEditorFlags(wall, StaticEditorFlags.NavigationStatic);
        }

        private static GameObject CreateCylinder(string name, Vector3 position, Vector3 scale, Material material, Transform parent)
        {
            GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cylinder.name = name;
            cylinder.transform.SetParent(parent);
            cylinder.transform.position = position;
            cylinder.transform.localScale = scale;
            cylinder.GetComponent<Renderer>().sharedMaterial = material;
            return cylinder;
        }

        private static void CreateRouteLabel(string text, Vector3 position, Transform parent)
        {
            GameObject label = new GameObject(text);
            label.transform.SetParent(parent);
            label.transform.position = position;
            TextMesh mesh = label.AddComponent<TextMesh>();
            mesh.text = text;
            mesh.characterSize = 0.35f;
            mesh.fontSize = 32;
            mesh.anchor = TextAnchor.MiddleCenter;
            mesh.alignment = TextAlignment.Center;
            label.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }

        private static void CreateCamera()
        {
            GameObject cameraObject = new("Main Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 16f, -13f);
            cameraObject.transform.rotation = Quaternion.Euler(58f, 0f, 0f);
            camera.fieldOfView = 45f;
            camera.clearFlags = CameraClearFlags.Skybox;
        }

        private static void CreateLighting()
        {
            GameObject lightObject = new("Directional Light");
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
            lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.55f, 0.58f, 0.62f);
        }

        private static void SetPrivateField<T>(Object target, string fieldName, T value)
        {
            FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            field?.SetValue(target, value);
            EditorUtility.SetDirty(target);
        }
    }
}
