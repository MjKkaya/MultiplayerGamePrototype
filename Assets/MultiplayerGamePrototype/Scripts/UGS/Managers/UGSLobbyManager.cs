using MultiplayerGamePrototype.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;


namespace MultiplayerGamePrototype.UGS.Managers
{
    public class UGSLobbyManager : ManagerSingleton<UGSLobbyManager>
    {
        private static Lobby m_CurrentLobby;
        public static Lobby CurrentLobby
        {
            get
            {
                return m_CurrentLobby;
            }
        }

        public static Action<List<LobbyPlayerJoined>> ActionOnPlayerJoined;
        public static Action ActionOnJoinedLobby;

        private LobbyEventCallbacks m_LobbyEventCallbacks;


        #region LobbyService Methods

        public async Task<bool> CreateLobbyAsync(string lobbyName, int maxPlayers, CreateLobbyOptions options)
        {
            try
            {
                m_CurrentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
                Debug.Log($"UGSLobbyManager-CreateLobbyAsync-LobbyId:{m_CurrentLobby.Id}");
                //StartHeartBeat();
                await BindLobby(m_CurrentLobby.Id);
                ActionOnJoinedLobby?.Invoke();
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"UGSLobbyManager-CreateLobbyAsync-ex:{ex}");
                //ActionOnFailedCreateLobby?.Invoke();
                return false;
            }
        }

        #endregion

        private async Task BindLobby(string lobbyID)
        {
            Debug.Log($"UGSLobbyManager-BindLobby-lobbyID:{lobbyID}");
            m_LobbyEventCallbacks = new LobbyEventCallbacks();
            //m_LobbyEventCallbacks.DataAdded += OnDataAdded;
            //m_LobbyEventCallbacks.DataChanged += OnDataChanged;
            //m_LobbyEventCallbacks.DataRemoved += DataRemoved;
            //m_LobbyEventCallbacks.LobbyChanged += OnLobbyChanged;
            //m_LobbyEventCallbacks.LobbyDeleted += OnLobbyDeleted;
            //m_LobbyEventCallbacks.LobbyEventConnectionStateChanged += OnLobbyEventConnectionStateChanged;
            //m_LobbyEventCallbacks.PlayerDataAdded += OnPlayerDataAdded;
            //m_LobbyEventCallbacks.PlayerDataChanged += OnPlayerDataChanged;
            //m_LobbyEventCallbacks.PlayerDataRemoved += OnPlayerDataRemoved;
            m_LobbyEventCallbacks.PlayerJoined += OnPlayerJoined;
            //m_LobbyEventCallbacks.PlayerLeft += OnPlayerLeft;

            ILobbyEvents lobbyEvents = await LobbyService.Instance.SubscribeToLobbyEventsAsync(lobbyID, m_LobbyEventCallbacks);
        }


        #region HeartBbeat
        /*
        async Task<Lobby> CreateLobbyWithHeartbeatAsync()
        {
            string lobbyName = "test lobby";
            int maxPlayers = 4;
            CreateLobbyOptions options = new CreateLobbyOptions();

            // Lobby parameters code goes here...
            // See 'Creating a Lobby' for example parameters
            var lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

            // Heartbeat the lobby every 15 seconds.
            StartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, 15));
            return lobby;
        }

        IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
        {
            var delay = new WaitForSecondsRealtime(waitTimeSeconds);

            while (true)
            {
                LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
                yield return delay;
            }


        }
        */
        #endregion


        #region Lobby Events

        private void OnPlayerJoined(List<LobbyPlayerJoined> joinedPlayers)
        {
            Debug.Log($"UGSLobbyManager-OnPlayerJoined-Count:{joinedPlayers.Count}");
            foreach (LobbyPlayerJoined item in joinedPlayers)
            {
                Debug.Log($"UGSLobbyManager-OnPlayerJoined-Index:{item.PlayerIndex}\n" +
                    $"PlayerId:{item.Player.Id}");
            }

            Debug.Log($"UGSLobbyManager-OnPlayerJoined-Players.Count:{m_CurrentLobby.Players.Count}");
            ActionOnPlayerJoined?.Invoke(joinedPlayers);
        }

        #endregion
    }
}
