using MultiplayerGamePrototype.Core;
using MultiplayerGamePrototype.UGS.LobbyController;
using System;
using System.Collections;
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
        public static Action ActionOnChangedLobbyData;
        public static Action ActionOnChangedMyPlayerData;

        private LobbyEventCallbacks m_LobbyEventCallbacks;


        private async Task BindLobby(string lobbyID)
        {
            Debug.Log($"UGSLobbyManager-BindLobby-lobbyID:{lobbyID}");
            m_LobbyEventCallbacks = new LobbyEventCallbacks();
            m_LobbyEventCallbacks.LobbyChanged += OnLobbyChanged;
            m_LobbyEventCallbacks.PlayerJoined += OnPlayerJoined;
            m_LobbyEventCallbacks.DataChanged += OnDataChanged;
            ILobbyEvents lobbyEvents = await LobbyService.Instance.SubscribeToLobbyEventsAsync(lobbyID, m_LobbyEventCallbacks);
        }


        #region LobbyService Methods

        public async Task<bool> CreateLobbyAsync(string lobbyName, string username)
        {
            try
            {
                m_CurrentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 100, UGSLobbyDataController.CreateLobbyOption(username));
                Debug.Log($"UGSLobbyManager-CreateLobbyAsync-LobbyId:{m_CurrentLobby.Id}");
                //StartHeartBeat();
                await BindLobby(m_CurrentLobby.Id);
                StartCoroutine(HeartbeatLobbyCoroutine(m_CurrentLobby.Id, 15));
                ActionOnJoinedLobby?.Invoke();
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"UGSLobbyManager-CreateLobbyAsync-ex:{ex}");
                return false;
            }
        }

        public async Task<bool> QuickJoinLobbyAsync(string username)
        {
            try
            {
                QuickJoinLobbyOptions options = new()
                {
                    Player = UGSLobbyDataController.CreateLobbyPlayer(username)
                };

                m_CurrentLobby = await Lobbies.Instance.QuickJoinLobbyAsync(options);
                await BindLobby(m_CurrentLobby.Id);
                ActionOnJoinedLobby?.Invoke();
                return true;
            }
            catch (Exception ex)
            {
                Debug.Log($"UGSLobbyManager-QuickJoinLobbyAsync-ex: {ex}");
                return false;
            }
        }

        public async Task<bool> UpdateLobbyDataAsync(UpdateLobbyOptions lobbyOptions)
        {
            try
            {
                await LobbyService.Instance.UpdateLobbyAsync(m_CurrentLobby.Id, lobbyOptions);
                return true;
            }
            catch (Exception ex)
            {
                Debug.Log($"UGSLobbyManager-UpdateLobbyData-ex:{ex}");
                return false;
            }
        }

        public async Task<bool> UpdateMyPlayerDataAsync(UpdatePlayerOptions playerOptions)
        {
            Debug.Log("UGSLobbyManager-UpdateMyPlayerDataAsync");
            try
            {
                m_CurrentLobby = await LobbyService.Instance.UpdatePlayerAsync(m_CurrentLobby.Id, UGSAuthManager.MyPlayerId, playerOptions);
                ActionOnChangedMyPlayerData?.Invoke();
                return true;
            }
            catch (Exception ex)
            {
                Debug.Log($"UGSLobbyManager-UpdateMyPlayerDataAsync-ex:{ex}");
                return false;
            }
        }

        #endregion


        #region HeartBbeat
        
        IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
        {
            Debug.Log($"UGSLobbyManager-HeartbeatLobbyCoroutine-lobbyId:{lobbyId}");
            var delay = new WaitForSecondsRealtime(waitTimeSeconds);
            while (true)
            {
                Debug.Log($"UGSLobbyManager-HeartbeatLobbyCoroutine-Continue-lobbyId:{lobbyId}");
                LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
                yield return delay;
            }
        }
        
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

        private void OnLobbyChanged(ILobbyChanges lobbyChanges)
        {
            Debug.Log($"UGSLobbyManager-OnLobbyChanged-Players.Count:{m_CurrentLobby.Players.Count}");
            if (!lobbyChanges.LobbyDeleted)
                lobbyChanges.ApplyToLobby(m_CurrentLobby);
            Debug.Log($"UGSLobbyManager-OnLobbyChanged-Players.Count:{m_CurrentLobby.Players.Count}");
        }

        private void OnDataChanged(Dictionary<string, ChangedOrRemovedLobbyValue<DataObject>> changedData)
        {
            Debug.Log($"UGSLobbyManager-OnDataChanged-Count:" + changedData.Count);
            foreach (var item in changedData)
            {
                Debug.Log($"UGSLobbyManager-OnDataChanged-Key:{item.Key}, lobbyValue=> {item.Value.Value.Value}");
            }
            ActionOnChangedLobbyData?.Invoke();
        }

        #endregion
    }
}
