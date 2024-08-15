using MultiplayerGamePrototype.Core;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;


public class EditorSceneController
{
    private static string m_lastOpenedScene;

    [MenuItem("MultiplayerGamePrototype/Scene Control/Play Beginning Scene")]
    static void OpenBeginningScene()
    {
        if(!EditorApplication.isPlaying)
        {
            //EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            EditorApplication.EnterPlaymode();
        }
    }

    static void OnPlayModeChanged(PlayModeStateChange playModeStateChange)
    {
        Debug.Log($"OnPlayModeChanged-playModeStateChange:{playModeStateChange}");
        if(playModeStateChange == PlayModeStateChange.ExitingEditMode)
        {
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            m_lastOpenedScene = EditorSceneManager.GetActiveScene().name;
            PlayerPrefs.SetString(PlayerPrefsManager.EDITOR_LAST_OPENED_SCENE, m_lastOpenedScene);
            EditorSceneManager.OpenScene("Assets/MultiplayerGamePrototype/Scenes/Bootstrap.unity");
        }
        else if(playModeStateChange == PlayModeStateChange.EnteredEditMode)
        {
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            EditorSceneManager.OpenScene($"Assets/MultiplayerGamePrototype/Scenes/{m_lastOpenedScene}.unity");
        }
    }

    [MenuItem("MultiplayerGamePrototype/Scene Control/Stop and Go Back Last Scene")]
    static void StopBackScene()
    {
        if(EditorApplication.isPlaying)
        {
            m_lastOpenedScene = PlayerPrefs.GetString(PlayerPrefsManager.EDITOR_LAST_OPENED_SCENE, m_lastOpenedScene);
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            EditorApplication.ExitPlaymode();
        }
    }
}