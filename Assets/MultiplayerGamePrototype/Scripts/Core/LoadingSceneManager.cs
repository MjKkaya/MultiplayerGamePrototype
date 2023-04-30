using MultiplayerGamePrototype.UGS.Managers;
using MultiplayerGamePrototype.UI.Utilities;
using MultiplayerGamePrototype.Utilities;
using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace MultiplayerGamePrototype.Core
{
    // Important: the names in the enum value should be the same as the scene you're trying to load
    public enum SceneName : byte
    {
        Bootstrap,
        Main,
        Lobby,
        Gameplay
    };


    public class LoadingSceneManager : SingletonMonoPersistent<LoadingSceneManager>
    {
        private SceneName m_sceneActive;
        public SceneName SceneActive => m_sceneActive;

        [SerializeField] private LoadingFadeEffect m_LoadingFadeEffect;


        public override void Awake()
        {
            base.Awake();
            UGSNetworkManager.ActionOnStartedServer += OnStartedServer;
        }

        //We cannot subscribe in the "awake" method as we have to wait for NetworkManager.Singleton to assign it.
        private void SubscribeNetworkManager()
        {
            //NetworkManager.Singleton.SceneManager.VerifySceneBeforeLoading
            NetworkManager.Singleton.SceneManager.OnLoadComplete += OnLoadComplete;
            //NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;
            NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEvent;

        }

        private void UnsubscribeNetworkManager()
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnLoadComplete;
            //NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnLoadEventCompleted;
            NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnSceneEvent;
        }

        public void LoadScene(SceneName sceneToLoad, bool isNetworkSessionActive = true)
        {
            Debug.Log($"LoadingSceneManager-LoadScene-sceneToLoad:{sceneToLoad}, isNetworkSessionActive:{isNetworkSessionActive}");
            //if (sceneToLoad == SceneName.Main)
            //    SubscribeNetworkManager();

            StartCoroutine(Loading(sceneToLoad, isNetworkSessionActive));
        }

        // Coroutine for the loading effect. It use an alpha in out effect
        private IEnumerator Loading(SceneName sceneToLoad, bool isNetworkSessionActive)
        {
            m_LoadingFadeEffect.FadeIn();
            // Here the player still sees the black screen
            yield return new WaitUntil(() => LoadingFadeEffect.CAN_LOAD);

            if (isNetworkSessionActive)
            {
                if (NetworkManager.Singleton.IsServer)
                    LoadSceneNetwork(sceneToLoad);
            }
            else
            {
                LoadSceneLocal(sceneToLoad);
            }

            // Because the scenes are not heavy we can just wait a second and continue with the fade.
            // In case the scene is heavy instead we should use additive loading to wait for the
            // scene to load before we continue
            yield return new WaitForSeconds(1f);

            m_LoadingFadeEffect.FadeOut();
        }

        // Load the scene using the regular SceneManager, use this if there's no active network session
        private void LoadSceneLocal(SceneName sceneToLoad)
        {
            SceneManager.LoadScene(sceneToLoad.ToString());
            switch (sceneToLoad)
            {
                case SceneName.Main:
                    //todo
                    //if (AudioManager.Instance != null)
                    //    AudioManager.Instance.PlayMusic(AudioManager.MusicName.intro);
                    break;
            }
        }

        // Load the scene using the SceneManager from NetworkManager. Use this when there is an active
        // network session
        private void LoadSceneNetwork(SceneName sceneToLoad)
        {
            SceneEventProgressStatus loadStatus = NetworkManager.Singleton.SceneManager.LoadScene(sceneToLoad.ToString(), LoadSceneMode.Single);
            Debug.Log($"LoadingSceneManager-LoadSceneNetwork-loadStatus:{loadStatus}");
        }

        // This callback function gets triggered when a scene is finished loading
        // Here we set up what to do for each scene, like changing the music
        private void OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
        {
            // We only care the host/server is loading because every manager handles
            // their information and behavior on the server runtime
            if (!NetworkManager.Singleton.IsServer)
                return;

            Enum.TryParse(sceneName, out m_sceneActive);

            //todo
            /*
            if (!ClientConnection.Instance.CanClientConnect(clientId))
                return;

            // What to initially do on every scene when it finishes loading
            switch (m_sceneActive)
            {
                // When a client/host connects tell the manager
                case SceneName.CharacterSelection:
                    CharacterSelectionManager.Instance.ServerSceneInit(clientId);
                    break;

                // When a client/host connects tell the manager to create the ship and change the music
                case SceneName.Gameplay:
                    GameplayManager.Instance.ServerSceneInit(clientId);
                    break;

                // When a client/host connects tell the manager to create the player score ships and
                // play the right SFX
                case SceneName.Victory:
                case SceneName.Defeat:
                    EndGameManager.Instance.ServerSceneInit(clientId);
                    break;
            }

            */
        }

        private void OnSceneEvent(SceneEvent sceneEvent)
        {
            Debug.Log($"LoadingSceneManager-OnSceneEvent-Scene:{sceneEvent.Scene.name}, SceneEventType:{sceneEvent.SceneEventType}, ClientId:{sceneEvent.ClientId}");
            //SceneEventType.
            //sceneEvent.ClientsThatCompleted
            //sceneEvent.ClientsThatTimedOut

            //I Think we have to set this variable when progress start than assign to this a local SceneEvent variable!
            //sceneEvent.AsyncOperation.progress
        }


        #region Events

        private void OnStartedServer()
        {
            SubscribeNetworkManager();
        }

        private void OnDestroy()
        {
            UGSNetworkManager.ActionOnStartedServer -= OnStartedServer;
        }

        #endregion
    }
}