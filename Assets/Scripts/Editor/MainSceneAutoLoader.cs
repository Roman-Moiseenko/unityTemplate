using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;


namespace Script.Editor
{
    [InitializeOnLoad]
    public static class MainSceneAutoLoader
    {
        private const string MAIN_PATH = "Assets/Scenes/Init.unity";
        private const string PREV_KEY_PREV_SCENE = "PREVIOUS SCENE";

        static MainSceneAutoLoader()
        {
            EditorApplication.playModeStateChanged += OnEnterPlayModeAttribute;
        }

        private static void OnEnterPlayModeAttribute(PlayModeStateChange state)
        {
            if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode) {
                if (SceneManager.GetActiveScene().buildIndex == 0) return;

                var path = SceneManager.GetActiveScene().path;
                EditorPrefs.SetString(PREV_KEY_PREV_SCENE, path);
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
                    try {
                        EditorSceneManager.OpenScene(MAIN_PATH);
                    } catch {
                        Debug.LogError($"Не загрузилась начальная сцена {MAIN_PATH}");
                        EditorApplication.isPlaying = false;
                    }
                } else {
                    EditorApplication.isPlaying = false;
                }
            }

            if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode) {
                try {
                    EditorSceneManager.OpenScene(EditorPrefs.GetString(PREV_KEY_PREV_SCENE));
                } catch {
                    Debug.LogError($"Не загрузилась текущую сцену {EditorPrefs.GetString(PREV_KEY_PREV_SCENE)}");
                    EditorApplication.isPlaying = false;
                }
            }
        }
    }
}
