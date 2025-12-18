#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class EditorStartSceneOverride
{
    private const string _ProjectScenesRoot = "Assets/FHH/Scenes";
    private static string _lastAppliedScenePath;
    private static bool _suppressDuringPlayTransition;
    private static bool _installed;

    static EditorStartSceneOverride()
    {
        EditorApplication.delayCall += Install;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetOnSubsystemRegistration()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorSceneManager.activeSceneChangedInEditMode -= OnActiveSceneChangedInEditMode;
        EditorApplication.delayCall -= Install;

        _suppressDuringPlayTransition = false;
        _lastAppliedScenePath = null;
        _installed = false;

        EditorApplication.delayCall += Install;
    }

    private static void Install()
    {
        if (_installed) return;
        _installed = true;

        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorSceneManager.activeSceneChangedInEditMode -= OnActiveSceneChangedInEditMode;

        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        EditorSceneManager.activeSceneChangedInEditMode += OnActiveSceneChangedInEditMode;

        var path = EditorSceneManager.GetActiveScene().path;
        if (!string.IsNullOrEmpty(path))
        {
            ApplyForScene(path);
            _lastAppliedScenePath = path;
        }
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode ||
            state == PlayModeStateChange.EnteredPlayMode)
        {
            _suppressDuringPlayTransition = true;
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            _suppressDuringPlayTransition = false;
        }
    }

    private static void OnActiveSceneChangedInEditMode(Scene oldScene, Scene newScene)
    {
        if (_suppressDuringPlayTransition || EditorApplication.isPlayingOrWillChangePlaymode) return;

        var path = newScene.path;
        if (string.IsNullOrEmpty(path) || path == _lastAppliedScenePath) return;

        ApplyForScene(path);
        _lastAppliedScenePath = path;
    }

    private static void ApplyForScene(string path)
    {
        var isProjectScene = path.StartsWith(_ProjectScenesRoot);
        if (!isProjectScene)
        {
            EditorSceneManager.playModeStartScene = null;
            return;
        }

        var scenes = EditorBuildSettings.scenes;
        if (scenes == null || scenes.Length == 0)
        {
            EditorSceneManager.playModeStartScene = null;
            return;
        }

        var first = scenes[0].path;
        var start = AssetDatabase.LoadAssetAtPath<SceneAsset>(first);
        EditorSceneManager.playModeStartScene = start;
        Debug.Log($"Set Play Mode Start Scene to '{first}'.");
    }
}
#endif