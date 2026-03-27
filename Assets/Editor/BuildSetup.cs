#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Unity.AI.Navigation;

public static class BuildSetup
{
    [MenuItem("MobaRoguelike/Setup Build Scenes")]
    public static void SetupBuildScenes()
    {
        var scenes = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/Game.unity", true),
        };
        EditorBuildSettings.scenes = scenes;
        Debug.Log("[BuildSetup] Build scenes configured: MainMenu=0, Game=1");
    }

    [MenuItem("MobaRoguelike/Bake NavMesh")]
    public static void BakeNavMesh()
    {
        var surface = Object.FindAnyObjectByType<NavMeshSurface>();
        if (surface != null)
        {
            surface.BuildNavMesh();
            Debug.Log("[BuildSetup] NavMesh baked successfully.");
        }
        else
        {
            Debug.LogWarning("[BuildSetup] No NavMeshSurface found in scene.");
        }
    }
}
#endif
