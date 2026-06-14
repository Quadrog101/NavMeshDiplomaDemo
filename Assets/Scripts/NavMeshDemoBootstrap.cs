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
            Material floorMaterial = CreateMaterial("Runtime Floor", new Color(0.70f, 0.73f, 0.68f));
            Material wallMaterial = CreateMaterial("Runtime Wall", new Color(0.25f, 0.28f, 0.31f));
            Material expensiveMaterial = CreateMaterial("Runtime Expensive", new Color(0.95f, 0.58f, 0.18f));
            Material bypassMaterial = CreateMaterial("Runtime Bypass", new Color(0.52f, 0.64f, 0.86f));
            Material obstacleMaterial = CreateMaterial("Runtime Obstacle", new Color(0.82f, 0.18f, 0.16f));
            Material agentMaterial = CreateMaterial("Runtime Agent", new Color(0.12f, 0.38f, 0.82f));
            Material finishMaterial = CreateMaterial("Runtime Finish", new Color(0.14f, 0.68f, 0.30f));
            Material pathMaterial = CreateMaterial("Runtime Path", new Color(0.02f, 0.10f, 0.16f));

            GameObject root = new("NavMesh Demo Runtime");
            GameObject geometryRoot = new("Geometry");
            geometryRoot.transform.SetParent(root.transform);

            CreateCube("Floor", new Vector3(0f, -0.05f, 0f), new Vector3(24f, 0.1f, 18f), floorMaterial, geometryRoot.transform);
            CreateCube("North Border", new Vector3(0f, 0.55f, 9f), new Vector3(24f, 1.1f, 0.35f), wallMaterial, geometryRoot.transform);
            CreateCube("South Border", new Vector3(0f, 0.55f, -9f), new Vector3(24f, 1.1f, 0.35f), wallMaterial, geometryRoot.transform);
            CreateCube("West Border", new Vector3(-12f, 0.55f, 0f), new Vector3(0.35f, 1.1f, 18f), wallMaterial, geometryRoot.transform);
            CreateCube("East Border", new Vector3(12f, 0.55f, 0f), new Vector3(0.35f, 1.1f, 18f), wallMaterial, geometryRoot.transform);

            CreateCube("Barrier Upper A", new Vector3(0f, 0.55f, 6.75f), new Vector3(0.45f, 1.1f, 3.7f), wallMaterial, geometryRoot.transform);
            CreateCube("Barrier Upper B", new Vector3(0f, 0.55f, 2.1f), new Vector3(0.45f, 1.1f, 1.9f), wallMaterial, geometryRoot.transform);
            CreateCube("Barrier Lower A", new Vector3(0f, 0.55f, -2.1f), new Vector3(0.45f, 1.1f, 1.9f), wallMaterial, geometryRoot.transform);
            CreateCube("Barrier Lower B", new Vector3(0f, 0.55f, -6.75f), new Vector3(0.45f, 1.1f, 3.7f), wallMaterial, geometryRoot.transform);

            GameObject costZone = CreateCube("Expensive Cost Zone", new Vector3(0f, 0.02f, 0f), new Vector3(6.2f, 0.04f, 2.8f), expensiveMaterial, geometryRoot.transform);
            costZone.GetComponent<Collider>().enabled = false;
            NavMeshModifierVolume modifierVolume = costZone.AddComponent<NavMeshModifierVolume>();
            modifierVolume.area = ExpensiveArea;
            modifierVolume.center = Vector3.zero;
            modifierVolume.size = new Vector3(6.2f, 1.2f, 2.8f);

            GameObject upperBypass = CreateCube("Upper Bypass Marker", new Vector3(0f, 0.01f, 4.7f), new Vector3(20f, 0.02f, 1.0f), bypassMaterial, geometryRoot.transform);
            GameObject lowerBypass = CreateCube("Lower Bypass Marker", new Vector3(0f, 0.01f, -4.7f), new Vector3(20f, 0.02f, 1.0f), bypassMaterial, geometryRoot.transform);
            upperBypass.GetComponent<Collider>().enabled = false;
            lowerBypass.GetComponent<Collider>().enabled = false;

            CreateRouteLabel("EXPENSIVE SHORTCUT", new Vector3(0f, 0.08f, 0f), geometryRoot.transform);
            CreateRouteLabel("LONG BYPASS", new Vector3(0f, 0.08f, 4.7f), geometryRoot.transform);
            CreateRouteLabel("LONG BYPASS", new Vector3(0f, 0.08f, -4.7f), geometryRoot.transform);

            GameObject startZone = CreateCylinder("Start Zone", new Vector3(-10f, 0.03f, 0f), new Vector3(1.8f, 0.06f, 1.8f), agentMaterial, root.transform);
            startZone.GetComponent<Collider>().enabled = false;
            GameObject finish = CreateCube("Finish Zone", new Vector3(10f, 0.04f, 0f), new Vector3(4.2f, 0.08f, 6.4f), finishMaterial, root.transform);
            BoxCollider finishCollider = finish.GetComponent<BoxCollider>();
            finishCollider.isTrigger = true;
            finishCollider.size = new Vector3(1f, 30f, 1f);
            finishCollider.center = Vector3.zero;
            finishCollider.enabled = false;
            NavMeshDemoFinishZone finishZone = finish.AddComponent<NavMeshDemoFinishZone>();

            CreateRouteLabel("START", new Vector3(-10f, 0.08f, -2.0f), root.transform);
            CreateRouteLabel("FINISH ZONE", new Vector3(10f, 0.10f, -3.2f), root.transform);

            GameObject surfaceObject = new("NavMesh Surface");
            surfaceObject.transform.SetParent(root.transform);
            NavMeshSurface surface = surfaceObject.AddComponent<NavMeshSurface>();
            surface.collectObjects = CollectObjects.All;
            surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            surface.BuildNavMesh();
            finishCollider.enabled = true;

            GameObject dynamicObstacle = CreateCube("Dynamic Obstacle (O)", new Vector3(0f, 0.75f, 0f), new Vector3(2.4f, 1.5f, 2.6f), obstacleMaterial, root.transform);
            dynamicObstacle.GetComponent<Collider>().isTrigger = true;
            NavMeshObstacle obstacle = dynamicObstacle.AddComponent<NavMeshObstacle>();
            obstacle.shape = NavMeshObstacleShape.Box;
            obstacle.size = new Vector3(2.4f, 1.5f, 2.6f);
            obstacle.carving = true;
            dynamicObstacle.SetActive(false);

            List<NavMeshDemoAgent> agents = new();
            for (int i = 0; i < 4; i++)
            {
                GameObject agent = CreateAgent(i + 1, new Vector3(-10f, 1f, 0f), agentMaterial, pathMaterial, root.transform);
                agent.SetActive(i == 0);
                agents.Add(agent.GetComponent<NavMeshDemoAgent>());
            }

            GameObject controllerObject = new("Demo Controller");
            controllerObject.transform.SetParent(root.transform);
            NavMeshDemoController controller = controllerObject.AddComponent<NavMeshDemoController>();
            SetPrivateField(controller, "finishZone", finishZone);
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
            agent.stoppingDistance = 0.15f;
            agent.autoRepath = true;
            agent.avoidancePriority = 40 + index;

            agentObject.AddComponent<NavMeshDemoAgent>();
            LineRenderer line = agentObject.AddComponent<LineRenderer>();
            line.sharedMaterial = pathMaterial;
            line.widthMultiplier = 0.12f;
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
            cameraObject.transform.position = new Vector3(0f, 24f, 0f);
            cameraObject.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            camera.orthographic = true;
            camera.orthographicSize = 11f;
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
