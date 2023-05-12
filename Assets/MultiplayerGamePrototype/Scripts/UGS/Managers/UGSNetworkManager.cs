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
    public class UGSNetworkManager : SingletonMono<UGSNetworkManager>
    {
        public bool IsHost
        {
            get
            {
                if (m_NetworkManager != null && m_NetworkManager.IsServer)
                    return true;
                else
                    return false;
            }
        }

        public static event Action ActionOnClientStopped;
        public static event Action ActionOnServerStarted;

        private NetworkManager m_NetworkManager;


        public override void Awake()
        {
            base.Awake();
            m_NetworkManager = GetComponent<NetworkManager>();
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

        public void StartHost(RelayServerData relayServerData)
        {
            SetCallbacks(true);
            m_NetworkManager.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            bool isSucceed = m_NetworkManager.StartHost();
            if(!isSucceed)
                SetCallbacks(false);
        }

        public void StartClient(RelayServerData relayServerData)
        {
            SetCallbacks(true);
            m_NetworkManager.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            bool isSucceed = m_NetworkManager.StartClient();
            if (!isSucceed)
                SetCallbacks(false);
        }


        private void SetCallbacks(bool isActive)
        {
            Debug.Log($"UGSNetworkManager-SetCallbacks-isActive:{isActive}");
            if(isActive)
            {
                m_NetworkManager.OnClientConnectedCallback += OnClientConnectedCallback;
                m_NetworkManager.OnClientDisconnectCallback += OnClientDisconnectCallback;
                m_NetworkManager.OnServerStarted += OnServerStarted;
                m_NetworkManager.OnServerStopped += OnServerStopped;
                m_NetworkManager.OnClientStarted += OnClientStarted;
                m_NetworkManager.OnClientStopped += OnClientStopped;
            }
            else
            {
                m_NetworkManager.OnClientConnectedCallback -= OnClientConnectedCallback;
                m_NetworkManager.OnClientDisconnectCallback -= OnClientDisconnectCallback;
                m_NetworkManager.OnServerStarted -= OnServerStarted;
                m_NetworkManager.OnServerStopped -= OnServerStopped;
                m_NetworkManager.OnClientStarted -= OnClientStarted;
                m_NetworkManager.OnClientStopped -= OnClientStopped;
            }
        }


        #region Events

        private void OnServerStarted()
        {
            Debug.Log("UGSNetworkManager-OnServerStarted");
            ActionOnServerStarted?.Invoke();
        }

        private void OnServerStopped(bool isStop)
        {
            Debug.Log($"UGSNetworkManager-OnServerStopped-isStop:{isStop}");
        }

        private void OnClientStarted()
        {
            Debug.Log("UGSNetworkManager-OnClientStarted");
        }

        private void OnClientStopped(bool isStop)
        {
            Debug.Log($"UGSNetworkManager-OnClientStopped-isStop:{isStop}");
            SetCallbacks(false);
            ActionOnClientStopped?.Invoke();
            LoadingSceneManager.Singleton.LoadScene(SceneName.Main, false);
        }

        private void OnClientConnectedCallback(ulong connectedClientId)
        {
            Debug.Log($"UGSNetworkManager-OnClientConnectedCallback-connectedClientId:{connectedClientId}");
        }

        private void OnClientDisconnectCallback(ulong disconnectedClient)
        {
            Debug.Log($"UGSNetworkManager-OnClientDisconnectCallback-disconnectedClient:{disconnectedClient}, ServerClientId:{NetworkManager.ServerClientId}, LocalClientId:{m_NetworkManager.LocalClientId}");
            //todo : will delete
            //if(disconnectedClient == NetworkManager.ServerClientId || disconnectedClient == m_NetworkManager.LocalClientId)
            //{
            //    SetCallbacks(false);
            //    ActionOnShutdownServer?.Invoke();
            //    LoadingSceneManager.Singleton.LoadScene(SceneName.Main, false);
            //}
        }

        private void OnDestroy()
        {
            if(m_NetworkManager != null)
            {
                SetCallbacks(false);
            }
        }

        #endregion
    }
}