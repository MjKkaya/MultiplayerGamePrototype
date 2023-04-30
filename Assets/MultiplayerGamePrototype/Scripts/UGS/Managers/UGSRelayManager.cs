using System;
using MultiplayerGamePrototype.Utilities;
using MultiplayerGamePrototype.UGS.DataControllers;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;


namespace MultiplayerGamePrototype.UGS.Managers
{
    public class UGSRelayManager : SingletonMonoPersistent<UGSRelayManager>
    {
        //public static Action ActionOnJoinedRelayServer;
        public static Action ActionOnFailedToJoinRelayServer;


        private string m_JoinCode;
        public string JoinCode
        {
            get
            {
                return m_JoinCode;
            }
        }


        public override void Awake()
        {
            base.Awake();
            UGSLobbyManager.ActionOnCreatedLobby += OnCreatedLobby;
            UGSLobbyManager.ActionOnJoinedLobby += OnJoinedLobby;
        }


        private async void AllocateRelayServerAndGetJoinCode(int maxConnections)
        {
            Debug.Log($"UGSRelayManager-AllocateRelayServerAndGetJoinCode:maxConnections{maxConnections}");

            //Ask Unity Services to allocate a Relay server that will handle up to eight players: seven peers and the host.
            try
            {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
                RelayServerData relayServerData = new(allocation, "dtls");
                try
                {
                    m_JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                    Debug.Log($"UGSRelayManager-AllocateRelayServerAndGetJoinCode-joinCode:{m_JoinCode}");
                    UGSNetworkManager.Singleton.StartHost(relayServerData);
                    //ActionOnJoinedRelayServer?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.Log($"UGSRelayManager-AllocateRelayServerAndGetJoinCode-GetJoinCode-ex:{ex}");
                    ActionOnFailedToJoinRelayServer?.Invoke();
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"UGSRelayManager-AllocateRelayServerAndGetJoinCode-CreateAllocation-ex:{ex}");
                ActionOnFailedToJoinRelayServer?.Invoke();
            }
        }

        private async void JoinAllocationAsync(string joinCode)
        {
            Debug.Log($"UGSRelayManager-JoinAllocationAsync-joinCode:{joinCode}");
            try
            {
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                RelayServerData relayServerData = new(joinAllocation, "dtls");
                UGSNetworkManager.Singleton.StartClient(relayServerData);
                m_JoinCode = joinCode;
                //ActionOnJoinedRelayServer?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.Log($"UGSRelayManager-JoinAllocationAsync-ex:{ex}");
            }
        }


        #region Events

        private void OnCreatedLobby(int maxPlayers)
        {
            AllocateRelayServerAndGetJoinCode(maxPlayers);
        }

        private void OnJoinedLobby()
        {
            if (!UGSLobbyManager.AmIhost)
                JoinAllocationAsync(UGSLobbyDataController.GetRelayJoinCode());
        }

        //private void OnClientDisconnectCallback(ulong connectedClientId)
        //{
        //    Debug.Log($"UGSRelayManager-OnClientDisconnectCallback-NetworkManager.Singleton.LocalClientId:{NetworkManager.Singleton.LocalClientId}, connectedClientId:{connectedClientId}, DisconnectReason:{NetworkManager.Singleton.DisconnectReason}");
        //    //todo:Change the host of the lobby and create new relay connection.
        //}

        private void OnDestroy()
        {
            //if(NetworkManager.Singleton != null)
            //    NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
            UGSLobbyManager.ActionOnCreatedLobby -= OnCreatedLobby;
            UGSLobbyManager.ActionOnJoinedLobby -= OnJoinedLobby;
        }

        #endregion
    }
}