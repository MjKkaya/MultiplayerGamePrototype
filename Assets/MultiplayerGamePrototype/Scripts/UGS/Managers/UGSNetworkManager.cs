using MultiplayerGamePrototype.Core;
using MultiplayerGamePrototype.Utilities;
using System;
using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine;


namespace MultiplayerGamePrototype.UGS.Managers
{
    [RequireComponent(typeof(NetworkManager))]
    public class UGSNetworkManager : SingletonMonoPersistent<UGSNetworkManager>
    {
        public bool IsServer
        {
            get
            {
                if (m_NetworkManager != null && m_NetworkManager.IsServer)
                    return true;
                else
                    return false;
            }
        }

        //todo: is this class neccesary?
        //public static Action ActionOnInitilized;
        //public static Action ActionOnSceneManagerInitilized;
        public static Action ActionOnShutdownServer;
        public static Action ActionOnStartedServer;

        private NetworkManager m_NetworkManager;


        public override void Awake()
        {
            base.Awake();
            m_NetworkManager = GetComponent<NetworkManager>();
            //StartCoroutine(CheckSingletonInitialization());
        }

        /// <summary>
        /// Every player can run this method even Host.
        /// If host hutdown the server ActionOnShutdownServer method will trigger every other clients!
        /// Than they will start their won shutdown process.
        /// </summary>
        public void Shutdown()
        {
            Debug.Log("UGSNetworkManager-Shutdown!");
            m_NetworkManager.Shutdown();
            LoadingSceneManager.Singleton.LoadScene(SceneName.Main, false);
        }

       
        //IEnumerator CheckSingletonInitialization()
        //{
        //    do
        //    {
        //        yield return null;
        //        Debug.Log("UGSNetworkManager-CheckSingletonInitialization-while");
        //    } while (NetworkManager.Singleton == null);

        //    Debug.Log("UGSNetworkManager-CheckSingletonInitialization-END");
        //    //ActionOnInitilized?.Invoke();
        //    SetCallbacks(true);
        //}

        private void SetCallbacks(bool isActive)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
            ActionOnStartedServer?.Invoke();
        }

        /*
        IEnumerator CheckSceneManagerInitialization()
        {
            Debug.Log("UGSNetworkManager-CheckSceneManagerInitialization");
            do
            {
                yield return null;
                Debug.Log("UGSNetworkManager-CheckSceneManagerInitialization-while");
            } while (NetworkManager.Singleton.SceneManager == null);

            ActionOnSceneManagerInitilized?.Invoke();
            Debug.Log("UGSNetworkManager-CheckSceneManagerInitialization-END!");
        }
        */


        public void StartHost(RelayServerData relayServerData)
        {
            m_NetworkManager.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            bool isSucceed = m_NetworkManager.StartHost();
            if(isSucceed)
                SetCallbacks(true);
        }

        public void StartClient(RelayServerData relayServerData)
        {
            m_NetworkManager.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            bool isSucceed = m_NetworkManager.StartClient();
            if (isSucceed)
                SetCallbacks(true);
        }


        #region Events

        private void OnClientConnectedCallback(ulong connectedClientId)
        {
            Debug.Log($"UGSNetworkManager-OnClientConnectedCallback-connectedClientId:{connectedClientId}");
            //StartCoroutine(CheckSceneManagerInitialization());
        }

        private void OnClientDisconnectCallback(ulong disconnectedClient)
        {
            Debug.Log($"UGSNetworkManager-OnClientDisconnectCallback-disconnectedClient:{disconnectedClient}, ServerClientId:{NetworkManager.ServerClientId}, LocalClientId:{m_NetworkManager.LocalClientId}");
            if(disconnectedClient == NetworkManager.ServerClientId || disconnectedClient == m_NetworkManager.LocalClientId)
            {
                SetCallbacks(false);
                ActionOnShutdownServer?.Invoke();
                LoadingSceneManager.Singleton.LoadScene(SceneName.Main, false);
            }
        }

        private void OnDestroy()
        {
            if(NetworkManager.Singleton != null)
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
        }

        #endregion
    }
}