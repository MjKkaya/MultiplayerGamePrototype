using MultiplayerGamePrototype.Core;
using MultiplayerGamePrototype.Events;
using MultiplayerGamePrototype.Utilities;
using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine;


namespace MultiplayerGamePrototype.UGS.Managers
{
    [RequireComponent(typeof(NetworkManager))]
    public class UGSNetworkManager : SingletonMonoPersistent<UGSNetworkManager>
    {
        public static ulong ServerClientId{
            get{
                return NetworkManager.ServerClientId;
            }
        }
            

        public bool IsHost{
            get{
                if (m_NetworkManager != null && m_NetworkManager.IsHost)
                    return true;
                else
                    return false;
            }
        }

        public bool IsListening{
            get{
                Debug.Log($"IsApproved:{m_NetworkManager.IsApproved}, IsListening:{m_NetworkManager.IsListening}, IsConnectedClient:{m_NetworkManager.IsConnectedClient}");
                if (m_NetworkManager != null && m_NetworkManager.IsListening)
                    return true;
                else
                    return false;
            }
        }


        private NetworkManager m_NetworkManager;
        private bool m_IsManuallyShutdown = false;


        public override void Awake()
        {
            Debug.Log("UGSNetworkManager-Awake");
            base.Awake();
            LobbyEvents.OnLeft += LobbyEvents_OnLeft;
            m_NetworkManager = GetComponent<NetworkManager>();
            Debug.Log("UGSNetworkManager-Awake-2");
        }

        private void Start()
        {
            Debug.Log("UGSNetworkManager-Start");
        }

        /// <summary>
        /// Every player can run this method even Host.
        /// If host hutdown the server ActionOnShutdownServer method will trigger every other clients!
        /// Than they will start their own shutdown process.
        /// </summary>
        private void Shutdown()
        {
            Debug.Log("UGSNetworkManager-Shutdown");
            m_IsManuallyShutdown = true;
            m_NetworkManager.Shutdown();
        }

        public void StartHost(RelayServerData relayServerData)
        {
            Debug.Log("UGSNetworkManager-StartHost");
            SetCallbacks(true);
            m_NetworkManager.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            bool isSucceed = m_NetworkManager.StartHost();
            if(!isSucceed)
                SetCallbacks(false);
        }

        public void StartClient(RelayServerData relayServerData)
        {
            Debug.Log("UGSNetworkManager-StartClient");
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
            NetworkManagerEvents.OnServerStarted?.Invoke();
        }

        private void OnServerStopped(bool isHost)
        {
            Debug.Log($"UGSNetworkManager-OnServerStopped-isHost:{isHost}");
            SetCallbacks(false);
            SceneLoadingManager.Singleton.LoadScene(SceneName.Main, false);
        }

        private void OnClientStarted()
        {
            Debug.Log("UGSNetworkManager-OnClientStarted");
            NetworkManagerEvents.OnClientStarted?.Invoke();
        }

        private void OnClientStopped(bool isHost)
        {
            Debug.Log($"UGSNetworkManager-OnClientStopped-isHost:{isHost}");
            if(!isHost)
            {
                SetCallbacks(false);
                if(m_IsManuallyShutdown)
                    SceneLoadingManager.Singleton.LoadScene(SceneName.Main, false);
                else
                {
                    //ActionOnServerStoppedByHost?.Invoke();
                    SceneLoadingManager.Singleton.LoadScene(SceneName.Lobby, false);
                }
            }
            m_IsManuallyShutdown = false;
        }

        private void OnClientConnectedCallback(ulong connectedClientId)
        {
            Debug.Log($"UGSNetworkManager-OnClientConnectedCallback-connectedClientId:{connectedClientId}");
        }

        private void OnClientDisconnectCallback(ulong disconnectedClient)
        {
            Debug.Log($"UGSNetworkManager-OnClientDisconnectCallback-disconnectedClient:{disconnectedClient}, ServerClientId:{NetworkManager.ServerClientId}, LocalClientId:{m_NetworkManager.LocalClientId}");
        }

        private void LobbyEvents_OnLeft()
        {
            Shutdown();
        }


        private void OnDestroy()
        {
            Debug.Log("UGSNetworkManager-OnDestroy");
            if(m_NetworkManager != null)
                SetCallbacks(false);

            LobbyEvents.OnLeft -= LobbyEvents_OnLeft;
        }

        #endregion
    }
}