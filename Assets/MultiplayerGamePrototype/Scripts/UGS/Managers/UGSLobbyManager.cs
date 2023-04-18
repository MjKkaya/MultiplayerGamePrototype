using MultiplayerGamePrototype.Core;
using MultiplayerGamePrototype.UGS.DataControllers;
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

        public static Player MyLobbyPlayer
        {
            get
            {
                Player myPlayer = null;
                if (m_CurrentLobby != null)
                {
                    foreach (Player player in m_CurrentLobby.Players)
                    {
                        if (player.Id == UGSAuthManager.MyPlayerId)
                        {
                            myPlayer = player;
                            break;
                        }
                    }
                }
                return myPlayer;
            }
        }

        public static bool AmIhost{
            get{
                if (m_CurrentLobby != null && m_CurrentLobby.HostId == UGSAuthManager.MyPlayerId)
                    return true;
                else
                    return false;
            }
        }

        public static Action<List<string>> ActionOnPlayerJoined;
        public static Action ActionOnJoinedLobby;
        public static Action ActionOnChangedGameBulletModeData;
        public static Action<Dictionary<string, string>> ActionOnChangedPlayersStatData;
        public static Action ActionOnChangedMyPlayerData;

        private LobbyEventCallbacks m_LobbyEventCallbacks;


        private async Task BindLobby(string lobbyID)
        {
            Debug.Log($"UGSLobbyManager-BindLobby-lobbyID:{lobbyID}");
            m_LobbyEventCallbacks = new LobbyEventCallbacks();
            m_LobbyEventCallbacks.LobbyChanged += OnLobbyChanged;
            m_LobbyEventCallbacks.PlayerJoined += OnPlayerJoined;
            m_LobbyEventCallbacks.DataChanged += OnDataChanged;
            m_LobbyEventCallbacks.PlayerLeft += OnPlayerLeft;
            ILobbyEvents lobbyEvents = await LobbyService.Instance.SubscribeToLobbyEventsAsync(lobbyID, m_LobbyEventCallbacks);
        }

        public Player GetPlayer(string playerId)
        {
            Player player = null;
            List<Player> players = m_CurrentLobby.Players;
            foreach (Player item in players)
            {
                if (item.Id == playerId)
                {
                    player = item;
                    break;
                }
            }
            return player;
        }

        private async void AddNewPlayerStatsToLobbyData(List<string> playersId)
        {
            Debug.Log($"UGSLobbyManager-AddNewPlayerStatsToLobbyData-playersId:{playersId.Count}");
            List<string> newPlayersId = new();
            foreach (string playerId in newPlayersId)
            {
                if (!CurrentLobby.Data.ContainsKey(playerId))
                    newPlayersId.Add(playerId);
            }

            Debug.Log($"UGSLobbyManager-AddNewPlayerStatsToLobbyData-newPlayersId:{newPlayersId.Count}");
            if (newPlayersId.Count > 0)
            {
                await UpdateLobbyDataAsync(UGSLobbyDataController.CreateNewLobbyPlayersStatsData(newPlayersId));
            }
        }

        //private async void UpdatePlayerScore()
        //{

        //}


        #region LobbyService Methods

        //public async Task<List<Lobby>> GetLobbyListAsync()
        //public async Task GetLobbyListAsync()
        //{
        //    QueryResponse queryResponse;
        //    List<Lobby> lobblist;
        //    try
        //    {
        //        queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
        //        lobblist = queryResponse.Results;
        //        Debug.Log($"UGSLobbyManager-GetLobbyListAsync-Count: {lobblist.Count}");
        //    }
        //    catch (LobbyServiceException e)
        //    {
        //        Debug.Log(e);
        //        lobblist = null;
        //    }
        //    //return lobblist;
        //}

        public async Task<bool> CreateLobbyAsync(string username)
        {
            try
            {
                m_CurrentLobby = await LobbyService.Instance.CreateLobbyAsync("Lobby-1", 100, UGSLobbyDataController.CreateLobbyOption(username));
                Debug.Log($"UGSLobbyManager-CreateLobbyAsync-LobbyId:{m_CurrentLobby.Id}, LobbyCode:{m_CurrentLobby.LobbyCode}");
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

        /*public async Task<bool> QuickJoinLobbyAsync(string username)
        {
            try
            {
                QuickJoinLobbyOptions options = new()
                {
                    Player = UGSLobbyDataController.CreateLobbyPlayer(username),
                    //Filter = new List<QueryFilter>
                    //{
                    //    new QueryFilter(
                    //            field: QueryFilter.FieldOptions.AvailableSlots,
                    //            value: "0",
                    //            op: QueryFilter.OpOptions.GT)
                    //}
                };

                m_CurrentLobby = await Lobbies.Instance.QuickJoinLobbyAsync();
                Debug.Log($"UGSLobbyManager-QuickJoinLobbyAsync-m_CurrentLobby: {m_CurrentLobby.Id}");
                await BindLobby(m_CurrentLobby.Id);
                ActionOnJoinedLobby?.Invoke();
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.Log($"UGSLobbyManager-QuickJoinLobbyAsync-ex: {ex}");
                return false;
            }
            //finally
            //{
            //    return false;
            //}
        }*/

        public async Task<bool> JoinLobbyByCodeAsync(string lobbyCode, string username)
        {
            try
            {
                JoinLobbyByCodeOptions options = new()
                {
                    Player = UGSPlayerDataController.CreateLobbyPlayer(username)
                };

                m_CurrentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, options);
                await BindLobby(m_CurrentLobby.Id);
                ActionOnJoinedLobby?.Invoke();
                return true;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log($"UGSLobbyManager-JoinLobbyByCodeAsync-ex: {e}");
                return false;
            }
        }

        public async Task<bool> UpdateLobbyDataAsync(UpdateLobbyOptions lobbyOptions)
        {
            Debug.Log("UGSLobbyManager-UpdateLobbyDataAsync");
            try
            {
                await LobbyService.Instance.UpdateLobbyAsync(m_CurrentLobby.Id, lobbyOptions);
                return true;
            }
            catch (Exception ex)
            {
                Debug.Log($"UGSLobbyManager-UpdateLobbyDataAsync-ex:{ex}");
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


        #region Events

        private void OnApplicationQuit()
        {
            if (m_CurrentLobby == null)
                return;

            try
            {
                Debug.Log("UGSLobbyManager-OnApplicationQuit");
                LobbyService.Instance.RemovePlayerAsync(m_CurrentLobby.Id, UGSAuthManager.MyPlayerId);
            }
            catch (System.Exception ex)
            {
                Debug.Log($"UGSLobbyManager-OnApplicationQuit-ex:{ex}");
            }
        }

        #region Lobby Events

        private void OnPlayerJoined(List<LobbyPlayerJoined> joinedPlayers)
        {
            Debug.Log($"UGSLobbyManager-OnPlayerJoined-Count:{joinedPlayers.Count}");
            List<string> newPlayersId = new();

            foreach (LobbyPlayerJoined item in joinedPlayers)
            {
                newPlayersId.Add(item.Player.Id);
                Debug.Log($"UGSLobbyManager-OnPlayerJoined-Index:{item.PlayerIndex}, PlayerId:{item.Player.Id}");
            }

            if (AmIhost)
                AddNewPlayerStatsToLobbyData(newPlayersId);

            Debug.Log($"UGSLobbyManager-OnPlayerJoined-Players.Count:{m_CurrentLobby.Players.Count}");
            ActionOnPlayerJoined?.Invoke(newPlayersId);
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
            Dictionary<string, string> changedDatas = new(); 

            Debug.Log($"UGSLobbyManager-OnDataChanged-Count:" + changedData.Count);
            foreach (var item in changedData)
            {
                changedDatas.Add(item.Key, item.Value.Value.Value);
                Debug.Log($"UGSLobbyManager-OnDataChanged-Key:{item.Key}, lobbyValue=> {item.Value.Value.Value}");
            }

            if (changedDatas.ContainsKey(UGSLobbyDataController.LOBBY_DATA_BULLET_COLOR) || changedDatas.ContainsKey(UGSLobbyDataController.LOBBY_DATA_BULLET_SIZE))
            {
                changedDatas.Remove(UGSLobbyDataController.LOBBY_DATA_BULLET_COLOR);
                changedDatas.Remove(UGSLobbyDataController.LOBBY_DATA_BULLET_COLOR);
                ActionOnChangedGameBulletModeData?.Invoke();
            }
            else
            {
                ActionOnChangedPlayersStatData?.Invoke(changedDatas);
            }
        }

        private void OnPlayerLeft(List<int> leftPlayerIds)
        {
            Debug.Log($"UGSLobbyManager-OnPlayerLeft-Count:{leftPlayerIds.Count}");
            //ActionOnPlayerLeft?.Invoke(leftPlayerIds);
        }

        #endregion

        #endregion
    }
}
