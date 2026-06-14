using NavMeshDiplomaDemo;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NavMeshDiplomaDemo.Editor
{
    public static class NavMeshDemoSceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/NavMeshDemo.unity";

        [MenuItem("Tools/NavMesh Diploma Demo/Rebuild Bootstrap Scene")]
        public static void RebuildScene()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            {
                AssetDatabase.CreateFolder("Assets", "Scenes");
            }

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "NavMeshDemo";

            GameObject bootstrap = new("NavMesh Demo Bootstrap");
            bootstrap.AddComponent<NavMeshDemoBootstrap>();

            Selection.activeObject = bootstrap;
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"NavMesh demo bootstrap scene rebuilt: {ScenePath}");
        }
    }
}
