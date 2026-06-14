using System.Collections.Generic;
using System.Reflection;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace NavMeshDiplomaDemo
{
    public sealed class NavMeshDemoBootstrap : MonoBehaviour
    {
        private const int HazardArea = 3;
        private const int PackedSandArea = 4;
        private const int DeepSandArea = 5;
        private const int RockArea = 6;
        private const int OasisArea = 7;
        private const int RoadArea = 8;
        private const int RiverArea = 9;

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
            Material floorMaterial = CreateMaterial("Desert Base", new Color(0.72f, 0.62f, 0.42f));
            Material wallMaterial = CreateMaterial("Rock Walls", new Color(0.30f, 0.25f, 0.22f));
            Material roadMaterial = CreateMaterial("Road", new Color(0.42f, 0.43f, 0.40f));
            Material packedSandMaterial = CreateMaterial("Packed Sand", new Color(0.82f, 0.70f, 0.46f));
            Material dunesMaterial = CreateMaterial("Desert Dunes", new Color(0.93f, 0.55f, 0.20f));
            Material rockMaterial = CreateMaterial("Mountains", new Color(0.45f, 0.38f, 0.34f));
            Material oasisMaterial = CreateMaterial("Oasis", new Color(0.25f, 0.60f, 0.48f));
            Material riverMaterial = CreateMaterial("River Crossing", new Color(0.16f, 0.45f, 0.78f));
            Material hazardMaterial = CreateMaterial("Heat Hazard", new Color(0.90f, 0.22f, 0.14f));
            Material obstacleMaterial = CreateMaterial("Rockfall Gate", new Color(0.72f, 0.28f, 0.16f));
            Material scoutMaterial = CreateMaterial("Scout Agent", new Color(0.10f, 0.55f, 0.95f));
            Material carrierMaterial = CreateMaterial("Carrier Agent", new Color(0.15f, 0.72f, 0.28f));
            Material nomadMaterial = CreateMaterial("Nomad Agent", new Color(0.62f, 0.32f, 0.88f));
            Material finishMaterial = CreateMaterial("Finish Camp", new Color(0.12f, 0.66f, 0.30f));
            Material scoutPathMaterial = CreateMaterial("Scout Route Line", new Color(0.05f, 0.92f, 1f));
            Material carrierPathMaterial = CreateMaterial("Carrier Route Line", new Color(0.20f, 1f, 0.25f));
            Material nomadPathMaterial = CreateMaterial("Nomad Route Line", new Color(1f, 0.88f, 0.05f));

            GameObject root = new("Desert Route Test");
            GameObject geometryRoot = new("Map Geometry");
            geometryRoot.transform.SetParent(root.transform);

            CreateCube("Desert Floor", new Vector3(0f, -0.05f, 0f), new Vector3(42f, 0.1f, 28f), floorMaterial, geometryRoot.transform);
            CreateWall("North Border", new Vector3(0f, 0.55f, 14f), new Vector3(42f, 1.1f, 0.35f), wallMaterial, geometryRoot.transform);
            CreateWall("South Border", new Vector3(0f, 0.55f, -14f), new Vector3(42f, 1.1f, 0.35f), wallMaterial, geometryRoot.transform);
            CreateWall("West Border", new Vector3(-21f, 0.55f, 0f), new Vector3(0.35f, 1.1f, 28f), wallMaterial, geometryRoot.transform);
            CreateWall("East Border", new Vector3(21f, 0.55f, 0f), new Vector3(0.35f, 1.1f, 28f), wallMaterial, geometryRoot.transform);

            CreateWall("North Fork Rock", new Vector3(-7.0f, 0.55f, 4.1f), new Vector3(3.0f, 1.1f, 2.0f), wallMaterial, geometryRoot.transform);
            CreateWall("Center Split Rock A", new Vector3(-1.5f, 0.55f, 3.4f), new Vector3(1.5f, 1.1f, 3.0f), wallMaterial, geometryRoot.transform);
            CreateWall("Center Split Rock B", new Vector3(2.6f, 0.55f, -3.0f), new Vector3(1.4f, 1.1f, 3.2f), wallMaterial, geometryRoot.transform);
            CreateWall("South Fork Rock", new Vector3(-5.2f, 0.55f, -6.4f), new Vector3(3.3f, 1.1f, 1.2f), wallMaterial, geometryRoot.transform);
            CreateWall("Canyon West Wall", new Vector3(5.2f, 0.55f, 1.8f), new Vector3(0.55f, 1.1f, 4.1f), wallMaterial, geometryRoot.transform);
            CreateWall("Canyon East Wall", new Vector3(8.0f, 0.55f, 1.8f), new Vector3(0.55f, 1.1f, 4.1f), wallMaterial, geometryRoot.transform);
            CreateWall("Oasis Bend Rock", new Vector3(8.2f, 0.55f, -6.8f), new Vector3(3.0f, 1.1f, 1.1f), wallMaterial, geometryRoot.transform);
            CreateWall("River Cliff North", new Vector3(11.0f, 0.55f, 4.8f), new Vector3(0.8f, 1.1f, 4.0f), wallMaterial, geometryRoot.transform);
            CreateWall("River Cliff South", new Vector3(11.0f, 0.55f, -3.0f), new Vector3(0.8f, 1.1f, 3.6f), wallMaterial, geometryRoot.transform);

            CreateAreaVolume("Road cost 1", new Vector3(-1.0f, 0.02f, 8.2f), new Vector3(33f, 0.04f, 2.3f), roadMaterial, geometryRoot.transform, RoadArea);
            CreateAreaVolume("Packed Sand Connector cost 2", new Vector3(-9.5f, 0.025f, -1.8f), new Vector3(8.5f, 0.05f, 8.4f), packedSandMaterial, geometryRoot.transform, PackedSandArea);
            CreateAreaVolume("Desert Dunes cost 4", new Vector3(0f, 0.03f, 0f), new Vector3(13.2f, 0.06f, 3.4f), dunesMaterial, geometryRoot.transform, DeepSandArea);
            CreateAreaVolume("Heat Hazard cost 18", new Vector3(3.8f, 0.035f, 0f), new Vector3(5.8f, 0.07f, 2.8f), hazardMaterial, geometryRoot.transform, HazardArea);
            CreateAreaVolume("Oasis cost 1.1", new Vector3(0f, 0.025f, -7.6f), new Vector3(28f, 0.05f, 2.7f), oasisMaterial, geometryRoot.transform, OasisArea);
            CreateAreaVolume("Mountains cost 3", new Vector3(9.0f, 0.025f, 3.4f), new Vector3(7.0f, 0.05f, 3.0f), rockMaterial, geometryRoot.transform, RockArea);
            CreateAreaVolume("River Ford cost 3", new Vector3(11.0f, 0.03f, 0.7f), new Vector3(2.3f, 0.06f, 9.2f), riverMaterial, geometryRoot.transform, RiverArea);
            CreateAreaVolume("Bridge Road cost 1", new Vector3(11.0f, 0.04f, 8.2f), new Vector3(3.0f, 0.08f, 1.6f), roadMaterial, geometryRoot.transform, RoadArea);

            CreateRouteLabel("СТАРТ", new Vector3(-16.2f, 0.10f, -10.2f), geometryRoot.transform);
            CreateRouteLabel("ДОРОГА: безопасно", new Vector3(-1.0f, 0.12f, 10.0f), geometryRoot.transform);
            CreateRouteLabel("ПУСТЫНЯ: коротко, дорого", new Vector3(0.0f, 0.12f, 2.0f), geometryRoot.transform);
            CreateRouteLabel("ОАЗИС: бонус воды", new Vector3(-0.5f, 0.12f, -9.5f), geometryRoot.transform);
            CreateRouteLabel("ГОРЫ / УЩЕЛЬЕ", new Vector3(8.0f, 0.12f, 5.3f), geometryRoot.transform);
            CreateRouteLabel("РЕКА / БРОД", new Vector3(12.5f, 0.12f, 1.0f), geometryRoot.transform);
            CreateRouteLabel("ЛАГЕРЬ", new Vector3(16.0f, 0.12f, 9.2f), geometryRoot.transform);

            GameObject startZone = CreateCylinder("Start Base", new Vector3(-16f, 0.04f, -8.5f), new Vector3(2.8f, 0.08f, 2.8f), scoutMaterial, root.transform);
            startZone.GetComponent<Collider>().enabled = false;
            CreateCampMarker("Start Base", new Vector3(-16f, 0.15f, -8.5f), scoutMaterial, root.transform);

            GameObject finish = CreateCube("Finish Camp", new Vector3(16.0f, 0.05f, 8.0f), new Vector3(5.2f, 0.1f, 7.0f), finishMaterial, root.transform);
            BoxCollider finishCollider = finish.GetComponent<BoxCollider>();
            finishCollider.isTrigger = true;
            finishCollider.size = new Vector3(1f, 30f, 1f);
            finishCollider.center = Vector3.zero;
            finishCollider.enabled = false;
            NavMeshDemoFinishZone finishZone = finish.AddComponent<NavMeshDemoFinishZone>();
            CreateCampMarker("Finish Camp", new Vector3(16f, 0.15f, 8.0f), finishMaterial, root.transform);

            GameObject surfaceObject = new("NavMesh Surface");
            surfaceObject.transform.SetParent(root.transform);
            NavMeshSurface surface = surfaceObject.AddComponent<NavMeshSurface>();
            surface.collectObjects = CollectObjects.All;
            surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            surface.BuildNavMesh();
            finishCollider.enabled = true;

            GameObject dynamicObstacle = CreateCube("Dynamic Rockfall Gate (O)", new Vector3(6.65f, 0.85f, 1.8f), new Vector3(2.6f, 1.7f, 3.6f), obstacleMaterial, root.transform);
            dynamicObstacle.GetComponent<Collider>().isTrigger = true;
            GameObject rockfallMarkerA = CreateCube("Rockfall Marker A", new Vector3(-0.55f, 0.38f, -0.55f), new Vector3(0.9f, 0.9f, 0.9f), wallMaterial, dynamicObstacle.transform);
            GameObject rockfallMarkerB = CreateCube("Rockfall Marker B", new Vector3(0.65f, 0.35f, 0.75f), new Vector3(0.8f, 0.8f, 0.8f), wallMaterial, dynamicObstacle.transform);
            rockfallMarkerA.GetComponent<Collider>().enabled = false;
            rockfallMarkerB.GetComponent<Collider>().enabled = false;
            NavMeshObstacle obstacle = dynamicObstacle.AddComponent<NavMeshObstacle>();
            obstacle.shape = NavMeshObstacleShape.Box;
            obstacle.size = new Vector3(2.6f, 1.7f, 3.6f);
            obstacle.carving = true;
            dynamicObstacle.SetActive(false);

            List<NavMeshDemoAgent> agents = new()
            {
                CreateAgent(1, "Scout", new Vector3(-16f, 1f, -8.5f), scoutMaterial, scoutPathMaterial, root.transform, 4.6f, 15f, 0.35f).GetComponent<NavMeshDemoAgent>(),
                CreateAgent(2, "Carrier", new Vector3(-16.8f, 1f, -7.2f), carrierMaterial, carrierPathMaterial, root.transform, 2.8f, 8f, 0.55f).GetComponent<NavMeshDemoAgent>(),
                CreateAgent(3, "Nomad", new Vector3(-16.8f, 1f, -9.8f), nomadMaterial, nomadPathMaterial, root.transform, 3.6f, 11f, 0.45f).GetComponent<NavMeshDemoAgent>()
            };

            for (int i = 1; i < agents.Count; i++)
            {
                agents[i].gameObject.SetActive(false);
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

        private static GameObject CreateAgent(
            int index,
            string profile,
            Vector3 position,
            Material agentMaterial,
            Material pathMaterial,
            Transform parent,
            float speed,
            float acceleration,
            float radius)
        {
            GameObject agentObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            agentObject.name = $"NPC {index} - {profile}";
            agentObject.transform.SetParent(parent);
            agentObject.transform.position = position;
            agentObject.transform.localScale = new Vector3(radius * 1.7f, 1f, radius * 1.7f);
            agentObject.GetComponent<Renderer>().sharedMaterial = agentMaterial;

            NavMeshAgent agent = agentObject.AddComponent<NavMeshAgent>();
            agent.angularSpeed = 420f;
            agent.stoppingDistance = 0.18f;
            agent.autoRepath = true;
            agent.avoidancePriority = 35 + index * 10;

            NavMeshDemoAgent demoAgent = agentObject.AddComponent<NavMeshDemoAgent>();
            demoAgent.ConfigureProfile(profile, speed, acceleration, radius);

            LineRenderer line = agentObject.AddComponent<LineRenderer>();
            line.sharedMaterial = pathMaterial;
            line.widthMultiplier = 0.38f;
            line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            line.receiveShadows = false;
            NavMeshDemoPathRenderer pathRenderer = agentObject.AddComponent<NavMeshDemoPathRenderer>();
            pathRenderer.Bind(agent);

            CreateRouteLabel(profile, position + new Vector3(0f, 0.05f, -0.9f), parent);
            return agentObject;
        }

        private static GameObject CreateCube(string name, Vector3 position, Vector3 scale, Material material, Transform parent)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.SetParent(parent);
            cube.transform.localPosition = position;
            cube.transform.localScale = scale;
            cube.GetComponent<Renderer>().sharedMaterial = material;
            return cube;
        }

        private static void CreateWall(string name, Vector3 position, Vector3 scale, Material material, Transform parent)
        {
            CreateCube(name, position, scale, material, parent);
        }

        private static GameObject CreateCylinder(string name, Vector3 position, Vector3 scale, Material material, Transform parent)
        {
            GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cylinder.name = name;
            cylinder.transform.SetParent(parent);
            cylinder.transform.localPosition = position;
            cylinder.transform.localScale = scale;
            cylinder.GetComponent<Renderer>().sharedMaterial = material;
            return cylinder;
        }

        private static GameObject CreateAreaVolume(string name, Vector3 position, Vector3 scale, Material material, Transform parent, int area)
        {
            GameObject marker = CreateCube(name, position, scale, material, parent);
            marker.GetComponent<Collider>().enabled = false;
            NavMeshModifierVolume volume = marker.AddComponent<NavMeshModifierVolume>();
            volume.area = area;
            volume.center = Vector3.zero;
            volume.size = new Vector3(1f, 30f, 1f);
            return marker;
        }

        private static void CreateRouteLabel(string text, Vector3 position, Transform parent)
        {
            GameObject label = new(text);
            label.transform.SetParent(parent);
            label.transform.localPosition = position;
            label.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

            TextMesh mesh = label.AddComponent<TextMesh>();
            mesh.text = text;
            mesh.characterSize = 0.32f;
            mesh.fontSize = 28;
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
            cameraObject.transform.position = new Vector3(0f, 36f, 0f);
            cameraObject.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            camera.orthographic = true;
            camera.orthographicSize = 16.2f;
        }

        private static void CreateLighting()
        {
            GameObject lightObject = new("Directional Light");
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.25f;
            lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.60f, 0.56f, 0.48f);
        }

        private static void SetPrivateField<T>(Object target, string fieldName, T value)
        {
            FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            field?.SetValue(target, value);
        }

        private static void CreateCampMarker(string name, Vector3 center, Material material, Transform parent)
        {
            GameObject flag = CreateCube($"{name} Flag", center + new Vector3(0.0f, 0.85f, 0.0f), new Vector3(0.18f, 1.6f, 0.18f), material, parent);
            flag.GetComponent<Collider>().enabled = false;
            GameObject tentA = CreateCube($"{name} Tent A", center + new Vector3(-0.75f, 0.22f, 0.55f), new Vector3(0.9f, 0.44f, 0.9f), material, parent);
            GameObject tentB = CreateCube($"{name} Tent B", center + new Vector3(0.85f, 0.22f, -0.55f), new Vector3(0.9f, 0.44f, 0.9f), material, parent);
            tentA.GetComponent<Collider>().enabled = false;
            tentB.GetComponent<Collider>().enabled = false;
        }
    }
}
