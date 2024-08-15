using System;
using MultiplayerGamePrototype.Utilities;
using MultiplayerGamePrototype.UGS.DataControllers;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using System.Threading.Tasks;
using MultiplayerGamePrototype.Events;


namespace MultiplayerGamePrototype.UGS.Managers
{
    public class UGSRelayManager : SingletonMonoPersistent<UGSRelayManager>
    {
        public static event Action ActionOnFailedToJoinRelayServer;


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
            LobbyEvents.OnCompletedCreation += LobbyEvents_OnCompletedCreation;
            LobbyEvents.OnCompletedJoin += LobbyEvents_OnCompletedJoin;
            UGSLobbyManager.ActionOnChangedRelayJoinCode += OnChangedRelayJoinCode;
        }


        public async Task<bool> AllocateRelayServerAndGetJoinCode(int maxConnections)
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
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.Log($"UGSRelayManager-AllocateRelayServerAndGetJoinCode-GetJoinCode-ex:{ex}");
                    ActionOnFailedToJoinRelayServer?.Invoke();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"UGSRelayManager-AllocateRelayServerAndGetJoinCode-CreateAllocation-ex:{ex}");
                ActionOnFailedToJoinRelayServer?.Invoke();
                return false;
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
            }
            catch (Exception ex)
            {
                Debug.Log($"UGSRelayManager-JoinAllocationAsync-ex:{ex}");
            }
        }


        #region Events

        private async void LobbyEvents_OnCompletedCreation(int maxPlayers)
        {
            await AllocateRelayServerAndGetJoinCode(maxPlayers);
        }

        private void LobbyEvents_OnCompletedJoin()
        {
            if (!UGSLobbyManager.Singleton.AmIhost)
                JoinAllocationAsync(UGSLobbyDataController.GetLobbyData(UGSLobbyDataController.LOBBY_DATA_RELAY_JOIN_CODE));
        }

        private void OnChangedRelayJoinCode()
        {
            Debug.Log($"UGSRelayManager-OnChangedRelayJoinCode-IsListening:{UGSNetworkManager.Singleton.IsListening}");
            if (!UGSNetworkManager.Singleton.IsListening)
            {
                string relayJoinCode = UGSLobbyDataController.GetLobbyData(UGSLobbyDataController.LOBBY_DATA_RELAY_JOIN_CODE);
                Debug.Log($"UGSRelayManager-OnChangedRelayJoinCode-relayJoinCode :{relayJoinCode}");
                JoinAllocationAsync(relayJoinCode);
            }
        }

        private void OnDestroy()
        {
            LobbyEvents.OnCompletedCreation -= LobbyEvents_OnCompletedCreation;
            LobbyEvents.OnCompletedJoin -= LobbyEvents_OnCompletedJoin;
            UGSLobbyManager.ActionOnChangedRelayJoinCode -= OnChangedRelayJoinCode;
        }

        #endregion
    }
}