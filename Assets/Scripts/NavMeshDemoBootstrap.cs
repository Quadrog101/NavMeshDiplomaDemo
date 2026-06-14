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
            Material roadMaterial = CreateMaterial("North Road", new Color(0.42f, 0.43f, 0.40f));
            Material packedSandMaterial = CreateMaterial("Packed Sand", new Color(0.82f, 0.70f, 0.46f));
            Material dunesMaterial = CreateMaterial("Central Dunes", new Color(0.93f, 0.55f, 0.20f));
            Material rockMaterial = CreateMaterial("Rough Rock", new Color(0.45f, 0.38f, 0.34f));
            Material oasisMaterial = CreateMaterial("Oasis Path", new Color(0.25f, 0.60f, 0.48f));
            Material hazardMaterial = CreateMaterial("Heat Hazard", new Color(0.90f, 0.22f, 0.14f));
            Material obstacleMaterial = CreateMaterial("Sandstorm Gate", new Color(0.72f, 0.28f, 0.16f));
            Material scoutMaterial = CreateMaterial("Scout Agent", new Color(0.10f, 0.55f, 0.95f));
            Material carrierMaterial = CreateMaterial("Carrier Agent", new Color(0.15f, 0.72f, 0.28f));
            Material rangerMaterial = CreateMaterial("Ranger Agent", new Color(0.62f, 0.32f, 0.88f));
            Material finishMaterial = CreateMaterial("Finish Camp", new Color(0.12f, 0.66f, 0.30f));
            Material pathMaterial = CreateMaterial("Route Line", new Color(1f, 0.95f, 0.05f));

            GameObject root = new("Desert Route Test");
            GameObject geometryRoot = new("Map Geometry");
            geometryRoot.transform.SetParent(root.transform);

            CreateCube("Desert Floor", new Vector3(0f, -0.05f, 0f), new Vector3(36f, 0.1f, 24f), floorMaterial, geometryRoot.transform);
            CreateWall("North Border", new Vector3(0f, 0.55f, 12f), new Vector3(36f, 1.1f, 0.35f), wallMaterial, geometryRoot.transform);
            CreateWall("South Border", new Vector3(0f, 0.55f, -12f), new Vector3(36f, 1.1f, 0.35f), wallMaterial, geometryRoot.transform);
            CreateWall("West Border", new Vector3(-18f, 0.55f, 0f), new Vector3(0.35f, 1.1f, 24f), wallMaterial, geometryRoot.transform);
            CreateWall("East Border", new Vector3(18f, 0.55f, 0f), new Vector3(0.35f, 1.1f, 24f), wallMaterial, geometryRoot.transform);

            CreateWall("North Fork Rock", new Vector3(-6.4f, 0.55f, 4.1f), new Vector3(3.0f, 1.1f, 2.0f), wallMaterial, geometryRoot.transform);
            CreateWall("Center Split Rock A", new Vector3(-1.5f, 0.55f, 3.4f), new Vector3(1.5f, 1.1f, 3.0f), wallMaterial, geometryRoot.transform);
            CreateWall("Center Split Rock B", new Vector3(2.6f, 0.55f, -3.0f), new Vector3(1.4f, 1.1f, 3.2f), wallMaterial, geometryRoot.transform);
            CreateWall("South Fork Rock", new Vector3(-5.2f, 0.55f, -6.4f), new Vector3(3.3f, 1.1f, 1.2f), wallMaterial, geometryRoot.transform);
            CreateWall("Canyon West Wall", new Vector3(5.2f, 0.55f, 1.8f), new Vector3(0.55f, 1.1f, 4.1f), wallMaterial, geometryRoot.transform);
            CreateWall("Canyon East Wall", new Vector3(8.0f, 0.55f, 1.8f), new Vector3(0.55f, 1.1f, 4.1f), wallMaterial, geometryRoot.transform);
            CreateWall("Oasis Bend Rock", new Vector3(8.2f, 0.55f, -6.8f), new Vector3(3.0f, 1.1f, 1.1f), wallMaterial, geometryRoot.transform);

            CreateAreaVolume("North Road Route cost 1", new Vector3(0f, 0.02f, 7.8f), new Vector3(30f, 0.04f, 2.2f), roadMaterial, geometryRoot.transform, RoadArea);
            CreateAreaVolume("Packed Sand Connector cost 2", new Vector3(-8.5f, 0.025f, 0f), new Vector3(7.4f, 0.05f, 7.8f), packedSandMaterial, geometryRoot.transform, PackedSandArea);
            CreateAreaVolume("Central Dunes cost 4", new Vector3(0f, 0.03f, 0f), new Vector3(12.0f, 0.06f, 3.2f), dunesMaterial, geometryRoot.transform, DeepSandArea);
            CreateAreaVolume("Heat Hazard cost 7", new Vector3(3.5f, 0.035f, 0f), new Vector3(5.4f, 0.07f, 2.8f), hazardMaterial, geometryRoot.transform, HazardArea);
            CreateAreaVolume("South Oasis Path cost 1.2", new Vector3(0f, 0.025f, -7.2f), new Vector3(26f, 0.05f, 2.5f), oasisMaterial, geometryRoot.transform, OasisArea);
            CreateAreaVolume("Rough Rock cost 3", new Vector3(10.8f, 0.025f, -2.9f), new Vector3(7.0f, 0.05f, 2.2f), rockMaterial, geometryRoot.transform, RockArea);

            CreateRouteLabel("START BASE", new Vector3(-15.2f, 0.10f, -2.0f), geometryRoot.transform);
            CreateRouteLabel("NORTH ROAD cost 1 - long safe", new Vector3(-1.0f, 0.12f, 9.45f), geometryRoot.transform);
            CreateRouteLabel("CENTRAL DUNES + HEAT - short costly", new Vector3(0.0f, 0.12f, 1.85f), geometryRoot.transform);
            CreateRouteLabel("SOUTH OASIS cost 1.2 - medium", new Vector3(-0.5f, 0.12f, -9.0f), geometryRoot.transform);
            CreateRouteLabel("CANYON GATE", new Vector3(6.65f, 0.12f, 4.25f), geometryRoot.transform);
            CreateRouteLabel("FINISH CAMP", new Vector3(15.0f, 0.12f, -3.5f), geometryRoot.transform);

            GameObject startZone = CreateCylinder("Start Base", new Vector3(-15f, 0.04f, 0f), new Vector3(2.4f, 0.08f, 2.4f), scoutMaterial, root.transform);
            startZone.GetComponent<Collider>().enabled = false;
            GameObject finish = CreateCube("Finish Camp", new Vector3(15.0f, 0.05f, 0f), new Vector3(4.6f, 0.1f, 7.4f), finishMaterial, root.transform);
            BoxCollider finishCollider = finish.GetComponent<BoxCollider>();
            finishCollider.isTrigger = true;
            finishCollider.size = new Vector3(1f, 30f, 1f);
            finishCollider.center = Vector3.zero;
            finishCollider.enabled = false;
            NavMeshDemoFinishZone finishZone = finish.AddComponent<NavMeshDemoFinishZone>();

            GameObject surfaceObject = new("NavMesh Surface");
            surfaceObject.transform.SetParent(root.transform);
            NavMeshSurface surface = surfaceObject.AddComponent<NavMeshSurface>();
            surface.collectObjects = CollectObjects.All;
            surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            surface.BuildNavMesh();
            finishCollider.enabled = true;

            GameObject dynamicObstacle = CreateCube("Dynamic Sandstorm Gate (O)", new Vector3(6.65f, 0.85f, 1.8f), new Vector3(2.6f, 1.7f, 3.6f), obstacleMaterial, root.transform);
            dynamicObstacle.GetComponent<Collider>().isTrigger = true;
            NavMeshObstacle obstacle = dynamicObstacle.AddComponent<NavMeshObstacle>();
            obstacle.shape = NavMeshObstacleShape.Box;
            obstacle.size = new Vector3(2.6f, 1.7f, 3.6f);
            obstacle.carving = true;
            dynamicObstacle.SetActive(false);

            List<NavMeshDemoAgent> agents = new()
            {
                CreateAgent(1, "Scout", new Vector3(-15f, 1f, 0f), scoutMaterial, pathMaterial, root.transform, 4.6f, 15f, 0.35f).GetComponent<NavMeshDemoAgent>(),
                CreateAgent(2, "Carrier", new Vector3(-15.8f, 1f, 1.15f), carrierMaterial, pathMaterial, root.transform, 2.8f, 8f, 0.55f).GetComponent<NavMeshDemoAgent>(),
                CreateAgent(3, "Ranger", new Vector3(-15.8f, 1f, -1.15f), rangerMaterial, pathMaterial, root.transform, 3.6f, 11f, 0.45f).GetComponent<NavMeshDemoAgent>()
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
            line.widthMultiplier = 0.28f;
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
            cube.transform.position = position;
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
            cylinder.transform.position = position;
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
            label.transform.position = position;
            label.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

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
            cameraObject.transform.position = new Vector3(0f, 32f, 0f);
            cameraObject.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            camera.orthographic = true;
            camera.orthographicSize = 14.2f;
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
    }
}
