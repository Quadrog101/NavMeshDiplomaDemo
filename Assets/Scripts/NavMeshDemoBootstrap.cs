using System.Collections.Generic;
using System.Reflection;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace NavMeshDiplomaDemo
{
    public sealed class NavMeshDemoBootstrap : MonoBehaviour
    {
        private const int ExpensiveArea = 3;

        private void Start()
        {
            if (FindFirstObjectByType<NavMeshDemoController>() != null)
            {
                return;
            }

            BuildRuntimeScene();
        }

        private void BuildRuntimeScene()
        {
            Material floorMaterial = CreateMaterial("Runtime Floor", new Color(0.72f, 0.75f, 0.70f));
            Material wallMaterial = CreateMaterial("Runtime Wall", new Color(0.28f, 0.31f, 0.34f));
            Material expensiveMaterial = CreateMaterial("Runtime Expensive", new Color(0.95f, 0.62f, 0.25f));
            Material obstacleMaterial = CreateMaterial("Runtime Obstacle", new Color(0.83f, 0.22f, 0.18f));
            Material agentMaterial = CreateMaterial("Runtime Agent", new Color(0.18f, 0.46f, 0.86f));
            Material targetMaterial = CreateMaterial("Runtime Target", new Color(0.18f, 0.72f, 0.34f));
            Material pathMaterial = CreateMaterial("Runtime Path", new Color(0.08f, 0.19f, 0.28f));

            GameObject root = new("NavMesh Demo Runtime");
            GameObject geometryRoot = new("Geometry");
            geometryRoot.transform.SetParent(root.transform);

            CreateCube("Floor", new Vector3(0f, -0.05f, 0f), new Vector3(24f, 0.1f, 18f), floorMaterial, geometryRoot.transform);
            CreateCube("North Border", new Vector3(0f, 0.55f, 9f), new Vector3(24f, 1.1f, 0.35f), wallMaterial, geometryRoot.transform);
            CreateCube("South Border", new Vector3(0f, 0.55f, -9f), new Vector3(24f, 1.1f, 0.35f), wallMaterial, geometryRoot.transform);
            CreateCube("West Border", new Vector3(-12f, 0.55f, 0f), new Vector3(0.35f, 1.1f, 18f), wallMaterial, geometryRoot.transform);
            CreateCube("East Border", new Vector3(12f, 0.55f, 0f), new Vector3(0.35f, 1.1f, 18f), wallMaterial, geometryRoot.transform);

            CreateCube("Barrier Upper A", new Vector3(0f, 0.55f, 6.75f), new Vector3(0.45f, 1.1f, 3.7f), wallMaterial, geometryRoot.transform);
            CreateCube("Barrier Upper B", new Vector3(0f, 0.55f, 2.35f), new Vector3(0.45f, 1.1f, 2.3f), wallMaterial, geometryRoot.transform);
            CreateCube("Barrier Lower A", new Vector3(0f, 0.55f, -2.35f), new Vector3(0.45f, 1.1f, 2.3f), wallMaterial, geometryRoot.transform);
            CreateCube("Barrier Lower B", new Vector3(0f, 0.55f, -6.75f), new Vector3(0.45f, 1.1f, 3.7f), wallMaterial, geometryRoot.transform);

            // Центральный проход короче, но этот объем назначает ему дорогой NavMesh area.
            GameObject costZone = CreateCube("Expensive Cost Zone", new Vector3(0f, 0.02f, 0f), new Vector3(4.8f, 0.04f, 2.2f), expensiveMaterial, geometryRoot.transform);
            costZone.GetComponent<Collider>().enabled = false;
            NavMeshModifierVolume modifierVolume = costZone.AddComponent<NavMeshModifierVolume>();
            modifierVolume.area = ExpensiveArea;
            modifierVolume.center = Vector3.zero;
            modifierVolume.size = new Vector3(4.8f, 1.2f, 2.2f);

            CreateRouteLabel("Short expensive gate", new Vector3(0f, 0.06f, 0f), geometryRoot.transform);
            CreateRouteLabel("Long upper route", new Vector3(0f, 0.06f, 4.6f), geometryRoot.transform);
            CreateRouteLabel("Long lower route", new Vector3(0f, 0.06f, -4.6f), geometryRoot.transform);

            GameObject dynamicObstacle = CreateCube("Dynamic Obstacle (O)", new Vector3(0f, 0.55f, 0f), new Vector3(1.5f, 1.1f, 1.5f), obstacleMaterial, root.transform);
            NavMeshObstacle obstacle = dynamicObstacle.AddComponent<NavMeshObstacle>();
            obstacle.shape = NavMeshObstacleShape.Box;
            obstacle.size = new Vector3(1.5f, 1.1f, 1.5f);
            obstacle.carving = true;
            dynamicObstacle.SetActive(false);

            CreateCylinder("Start", new Vector3(-10f, 0.03f, 0f), new Vector3(1.2f, 0.06f, 1.2f), agentMaterial, root.transform);
            GameObject target = CreateCylinder("Target", new Vector3(10f, 0.03f, 0f), new Vector3(1.2f, 0.06f, 1.2f), targetMaterial, root.transform);

            List<NavMeshDemoAgent> agents = new();
            for (int i = 0; i < 4; i++)
            {
                GameObject agent = CreateAgent(i + 1, new Vector3(-10f, 1f, 0f), agentMaterial, pathMaterial, root.transform);
                agent.SetActive(i == 0);
                agents.Add(agent.GetComponent<NavMeshDemoAgent>());
            }

            GameObject surfaceObject = new("NavMesh Surface");
            surfaceObject.transform.SetParent(root.transform);
            NavMeshSurface surface = surfaceObject.AddComponent<NavMeshSurface>();
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
        }

        private static Material CreateMaterial(string name, Color color)
        {
            Material material = new(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"))
            {
                name = name,
                color = color
            };

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

            agentObject.AddComponent<NavMeshDemoAgent>();
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
            GameObject label = new(text);
            label.transform.SetParent(parent);
            label.transform.position = position;
            label.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

            TextMesh mesh = label.AddComponent<TextMesh>();
            mesh.text = text;
            mesh.characterSize = 0.35f;
            mesh.fontSize = 32;
            mesh.anchor = TextAnchor.MiddleCenter;
            mesh.alignment = TextAlignment.Center;
        }

        private static void CreateCamera()
        {
            if (Camera.main != null)
            {
                Destroy(Camera.main.gameObject);
            }

            GameObject cameraObject = new("Main Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 16f, -13f);
            cameraObject.transform.rotation = Quaternion.Euler(58f, 0f, 0f);
            camera.fieldOfView = 45f;
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
        }
    }
}
