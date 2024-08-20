using MultiplayerGamePrototype.Core;
using MultiplayerGamePrototype.Events;
using MultiplayerGamePrototype.UGS.DataControllers;
using MultiplayerGamePrototype.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;


namespace MultiplayerGamePrototype.UGS.Managers
{
    public class UGSLobbyManager : SingletonMonoPersistent<UGSLobbyManager>
    {
        private static readonly int LOBBY_MAX_PLAYERS = 2;

        public static event Action<List<string>> ActionOnPlayerJoined;
        // public static event Action<int> ActionOnCreatedLobby;
        // public static event Action ActionOnJoinedLobby;
        public static event Action ActionOnChangedMyPlayerData;
        public static event Action ActionOnChangedGameBulletModeData;
        // public static event Action<string, string> ActionOnChangedPlayersStatData;
        public static event Action ActionOnChangedHost;
        public static event Action ActionOnChangedRelayJoinCode;


        private static Lobby m_CurrentLobby;
        public static Lobby CurrentLobby
        {
            get
            {
                return m_CurrentLobby;
            }
        }

        public Player MyLobbyPlayer
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

        public bool AmIhost{
            get{
                if (m_CurrentLobby != null && m_CurrentLobby.HostId == UGSAuthManager.MyPlayerId)
                    return true;
                else
                    return false;
            }
        }

        private LobbyEventCallbacks m_LobbyEventCallbacks;
        private Coroutine m_HeartbeatLobbyCoroutine;


        public override void Awake()
        {
            Debug.Log("UGSLobbyManager-Awake");
            base.Awake();
            m_LobbyEventCallbacks = new LobbyEventCallbacks();
            NetworkManagerEvents.OnServerStarted += NetworkManagerEvents_OnServerStarted;
            SceneLoadingManager.ActionOnLoadClientGameplaySceneComplete += OnLoadClientGameplaySceneComplete;
            LobbyEvents.Create += LobbyEvents_Create;
            LobbyEvents.Join += LobbyEvents_Join;
            LobbyEvents.QuickJoin += LobbyEvents_QuickJoin;
            LobbyEvents.Leave += LobbyEvents_Leave;
        }


        private async Task BindLobby(string lobbyID)
        {
            Debug.Log($"UGSLobbyManager-BindLobby-lobbyID:{lobbyID}");
            m_LobbyEventCallbacks.LobbyChanged += OnLobbyChanged;
            m_LobbyEventCallbacks.DataAdded += OnDataAdded;
            m_LobbyEventCallbacks.DataChanged += OnDataChanged;
            m_LobbyEventCallbacks.DataRemoved+= OnDataRemoved;
            m_LobbyEventCallbacks.PlayerJoined += OnPlayerJoined;
            m_LobbyEventCallbacks.PlayerLeft += OnPlayerLeft;
            m_LobbyEventCallbacks.LobbyDeleted += OnLobbyDeleted;
            m_LobbyEventCallbacks.KickedFromLobby += OnKickedFromLobby;
            m_LobbyEventCallbacks.LobbyEventConnectionStateChanged += OnLobbyEventConnectionStateChanged;
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

        private void UnsubscribeLobby()
        {
            try
            {
                m_LobbyEventCallbacks = new LobbyEventCallbacks();
                Debug.Log($"UGSLobbyManager-UnsubscribedLobby-m_HeartbeatLobbyCoroutine:{m_HeartbeatLobbyCoroutine}");
                if (gameObject != null && m_HeartbeatLobbyCoroutine != null)
                {
                    Debug.Log("UGSLobbyManager-UnsubscribedLobby-StopCoroutine");
                    StopCoroutine(m_HeartbeatLobbyCoroutine);
                }
                m_CurrentLobby = null;
                    
            }
            catch (Exception ex)
            {
                Debug.Log($"UGSLobbyManager-UnsubscribedLobby-ex{ex}");
            }
        }

        private void DebugPlayers()
        {
            Debug.Log("UGSLobbyManager-DebugPlayers");
            List<Player> players = m_CurrentLobby.Players;
            int playersCount = players.Count;
            Player player;
            for (int i = 0; i < playersCount; i++)
            {
                player = players[i];
                if(player == null)
                {
                    Debug.Log($"UGSLobbyManager-DebugPlayers- {i}.player is null!!!");
                    continue;
                }

                Debug.Log($"UGSLobbyManager-DebugPlayers-{i}.player=>\n:{player.ToStringFull()}");
            }
        }


        #region LobbyService Methods

        private async void CreateLobbyAsync(string lobbyName)
        {
            try
            {
                m_CurrentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, LOBBY_MAX_PLAYERS, UGSLobbyDataController.CreateBaseLobbyData(UGSAuthManager.MyUsername));
                Debug.Log($"UGSLobbyManager-CreateLobbyAsync-LobbyId:{m_CurrentLobby.Id}, LobbyCode:{m_CurrentLobby.LobbyCode}");
                await BindLobby(m_CurrentLobby.Id);
                m_HeartbeatLobbyCoroutine = StartCoroutine(HeartbeatLobbyCoroutine(m_CurrentLobby.Id, 15));
                LobbyEvents.OnCompletedCreation?.Invoke(LOBBY_MAX_PLAYERS);
                LobbyEvents.OnCompletedJoin?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"UGSLobbyManager-CreateLobbyAsync-ex:{ex}");
                LobbyEvents.OnFailedCreation?.Invoke();
            }
        }

        public async void QuickJoinLobbyAsync()
        {
            try
            {
                QuickJoinLobbyOptions quickJoinLobbyOptions = new()
                {
                    Filter = new List<QueryFilter>
                    {
                        new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                    },
                    Player = UGSPlayerDataController.CreateLobbyPlayer(UGSAuthManager.MyUsername)
                };

                m_CurrentLobby = await LobbyService.Instance.QuickJoinLobbyAsync(quickJoinLobbyOptions);
                await BindLobby(m_CurrentLobby.Id);
                // ActionOnJoinedLobby?.Invoke();
                LobbyEvents.OnCompletedJoin?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"UGSLobbyManager-QuickJoinOrCreate-ex:{ex}");
                // LobbyEvents.OnFailedQuickJoin?.Invoke();
                CreateLobbyAsync("QuickJoin");
            }
        }

        

        private async void JoinLobbyByCodeAsync(string lobbyCode)
        {
            try
            {
                JoinLobbyByCodeOptions options = new()
                {
                    Player = UGSPlayerDataController.CreateLobbyPlayer(UGSAuthManager.MyUsername)
                };

                m_CurrentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, options);
                await BindLobby(m_CurrentLobby.Id);
                // ActionOnJoinedLobby?.Invoke();
                LobbyEvents.OnCompletedJoin?.Invoke();
            }
            catch (LobbyServiceException e)
            {
                Debug.Log($"UGSLobbyManager-JoinLobbyByCodeAsync-ex: {e}");
                LobbyEvents.OnFailedJoin?.Invoke();
            }
        }

        public async Task<bool> UpdateLobbyDataAsync(UpdateLobbyOptions lobbyOptions)
        {
            Debug.Log("UGSLobbyManager-UpdateLobbyDataAsync");
            try
            {
                m_CurrentLobby = await LobbyService.Instance.UpdateLobbyAsync(m_CurrentLobby.Id, lobbyOptions);
                return true;
            }
            catch (Exception ex)
            {
                Debug.Log($"UGSLobbyManager-UpdateLobbyDataAsync-ex:{ex}");
                return false;
            }
        }
        private async void UpdateGameStateData(GameStateTypes gameStateType)
        {
            await UpdateLobbyDataAsync(UGSLobbyDataController.UpdateGameState(gameStateType));
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

        public async Task<bool> DeleteCurrentLobbyAsync()
        {
            Debug.Log($"UGSLobbyManager-DeleteCurrentLobbyAsync-LobbyId:{m_CurrentLobby.Id}");
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(m_CurrentLobby.Id);
                return true;
            }
            catch (Exception ex)
            {
                Debug.Log($"UGSLobbyManager-DeleteCurrentLobbyAsync-ex:{ex}");
                return false;
            }
        }

        private async void RemovePlayerAsync(string playerId)
        {
            try
            {
                Debug.Log($"UGSLobbyManager-RemovePlayerAsync-playerId:{playerId}");
                await LobbyService.Instance.RemovePlayerAsync(m_CurrentLobby.Id, playerId);
            }
            catch (Exception ex)
            {
                Debug.Log($"UGSLobbyManager-RemovePlayerAsync-ex:{ex}");
            }
        }

        public async Task<bool> MigrateHostRandomly()
        {
            Debug.Log($"UGSLobbyManager-MigrateHostRandomly");
            try
            {
                string currentHostId = m_CurrentLobby.HostId;
                string newHostId = null;

                List<Player> players = m_CurrentLobby.Players;
                int playerCount = players.Count;
                Player player;
                for (int i = 0; i < playerCount; i++)
                {
                    player = players[i];
                    if (player != null && currentHostId != player.Id)
                    {
                        newHostId = player.Id;
                        break;
                    }
                }

                Debug.Log($"UGSLobbyManager-MigrateHostRandomly-Current/New => {currentHostId}/{newHostId}");
                if (string.IsNullOrEmpty(newHostId))
                    return false;

                UpdateLobbyOptions lobbyOptions = new()
                {
                    HostId = newHostId
                };

                m_CurrentLobby = await LobbyService.Instance.UpdateLobbyAsync(m_CurrentLobby.Id, lobbyOptions);
                //StopHeartBeat();
                return true;
            }
            catch (Exception ex)
            {
                Debug.Log($"UGSLobbyManager-MigrateHostRandomly-ex:{ex}");
                return false;
            }
        }

        public async Task<bool> MigrateLobbyHost(string newHostId)
        {
            Debug.Log($"UGSLobbyManager-MigrateLobbyHost:{m_CurrentLobby.HostId} => {newHostId}");
            try
            {
                UpdateLobbyOptions lobbyOptions = new()
                {
                    HostId = newHostId
                };

                m_CurrentLobby = await LobbyService.Instance.UpdateLobbyAsync(m_CurrentLobby.Id, lobbyOptions);
                return true;
            }
            catch (Exception ex)
            {
                Debug.Log($"UGSLobbyManager-MigrateLobbyHost-ex:{ex}");
                return false;
            }
        }

        #endregion


        #region HeartBbeat

        IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
        {
            //Debug.Log($"UGSLobbyManager-HeartbeatLobbyCoroutine-lobbyId:{lobbyId}");
            var delay = new WaitForSecondsRealtime(waitTimeSeconds);
            do
            {
                //Debug.Log($"UGSLobbyManager-HeartbeatLobbyCoroutine-Continue-lobbyId:{lobbyId}");
                LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
                yield return delay;
            } while (m_CurrentLobby != null);
        }

        #endregion


        #region Events

        /// <summary>
        //  When Host joined the "Relay Server" that "Relay Server"'s join code must be share to other joined players.
        /// </summary>
        /// <param name="relayJoinCode"></param>
        private async void NetworkManagerEvents_OnServerStarted()
        {
            Debug.Log("UGSLobbyManager-NetworkManagerEvents_OnServerStarted");
            await UpdateLobbyDataAsync(UGSLobbyDataController.CreateInitialData(UGSRelayManager.Singleton.JoinCode));
        }


        private async void OnLoadClientGameplaySceneComplete(ulong clientId)
        {
            Debug.Log($"UGSLobbyManager-OnLoadClientGameplaySceneComplete-clientId/ServerClientId:{clientId}/{UGSNetworkManager.ServerClientId}");
            if(clientId == UGSNetworkManager.ServerClientId)
                await UpdateLobbyDataAsync(UGSLobbyDataController.UpdateGameState(GameStateTypes.Started));
        }


        private void OnApplicationQuit()
        {
            if (m_CurrentLobby != null && SceneLoadingManager.Singleton.IsCurrentSceneSame(SceneName.Lobby))
            {
                try
                {
                    Debug.Log("UGSLobbyManager-OnApplicationQuit");
                    RemovePlayerAsync(UGSAuthManager.MyPlayerId);
                }
                catch (System.Exception ex)
                {
                    Debug.Log($"UGSLobbyManager-OnApplicationQuit-ex:{ex}");
                }
            }
        }

        private void OnDestroy()
        {
            LobbyEvents.Create -= LobbyEvents_Create;
            LobbyEvents.Join -= LobbyEvents_Join;
            LobbyEvents.QuickJoin -= LobbyEvents_QuickJoin;
            LobbyEvents.Leave -= LobbyEvents_Leave;
            NetworkManagerEvents.OnServerStarted -= NetworkManagerEvents_OnServerStarted;
            SceneLoadingManager.ActionOnLoadClientGameplaySceneComplete -= OnLoadClientGameplaySceneComplete;
        }

        private void LobbyEvents_Create(string lobbyName)
        {
            CreateLobbyAsync(lobbyName);
        }

        private void LobbyEvents_Join(string lobbyName)
        {
            JoinLobbyByCodeAsync(lobbyName);
        }

        private void LobbyEvents_QuickJoin()
        {
            QuickJoinLobbyAsync();
        }

        private void LobbyEvents_Leave(string playerId)
        {
            RemovePlayerAsync(playerId);
        }

    



        #region Lobby Events

        private void OnLobbyChanged(ILobbyChanges lobbyChanges)
        {
            if(m_CurrentLobby == null)
            {
                Debug.Log("UGSLobbyManager-OnLobbyChanged-m_CurrentLobby is null!");
                return;
            }
            Debug.Log($"UGSLobbyManager-OnLobbyChanged-before-Players.Count:{m_CurrentLobby.Players.Count}");
            if (!lobbyChanges.LobbyDeleted)
            {
                string currentHostId = m_CurrentLobby.HostId;
                lobbyChanges.ApplyToLobby(m_CurrentLobby);

                if (lobbyChanges.HostId.Changed)
                {
                    Debug.Log($"UGSLobbyManager-OnLobbyChanged-HostId Changed:{currentHostId}=>{lobbyChanges.HostId.Value}");
                    if(AmIhost)
                    {
                        UpdateGameStateData(GameStateTypes.Paused);
                        m_HeartbeatLobbyCoroutine = StartCoroutine(HeartbeatLobbyCoroutine(m_CurrentLobby.Id, 15));
                    }

                    ActionOnChangedHost?.Invoke();
                }
            }
            Debug.Log($"UGSLobbyManager-OnLobbyChanged-after-Players.Count:{m_CurrentLobby.Players.Count}");
        }

        private void OnPlayerJoined(List<LobbyPlayerJoined> joinedPlayers)
        {
            Debug.Log($"UGSLobbyManager-OnPlayerJoined-Count:{joinedPlayers.Count}");
            List<string> newPlayersId = new();

            foreach (LobbyPlayerJoined item in joinedPlayers)
            {
                newPlayersId.Add(item.Player.Id);
                Debug.Log($"UGSLobbyManager-OnPlayerJoined-Index:{item.PlayerIndex}, Player=>\n{item.Player.ToStringFull()}");
            }

            if (AmIhost)
                AddNewPlayerStatsToLobbyData(newPlayersId);
            DebugPlayers();
            ActionOnPlayerJoined?.Invoke(newPlayersId);
        }

        

        private void OnDataAdded(Dictionary<string, ChangedOrRemovedLobbyValue<DataObject>> addedData)
        {
            string key;
            string data;
            Debug.Log($"UGSLobbyManager-OnDataAdded-Count:" + addedData.Count);
            if (m_CurrentLobby == null)
                return;

            foreach (var item in addedData)
            {
                key = item.Key;
                data = item.Value.Value.Value;
                Debug.Log($"UGSLobbyManager-OnDataAdded-Key:{key}, data=> {data}");

                if (key == UGSLobbyDataController.LOBBY_DATA_BULLET_COLOR || key == UGSLobbyDataController.LOBBY_DATA_BULLET_SIZE)
                {
                    ActionOnChangedGameBulletModeData?.Invoke();
                }
                else if (key == UGSLobbyDataController.LOBBY_DATA_RELAY_JOIN_CODE)
                {
                    Debug.Log($"UGSLobbyManager-OnDataAdded-LOBBY_DATA_RELAY_JOIN_CODE");
                    ActionOnChangedRelayJoinCode?.Invoke();
                }
                else if (key == UGSLobbyDataController.LOBBY_DATA_GAME_STATE)
                {
                    Debug.Log($"UGSLobbyManager-OnDataAdded-LOBBY_DATA_GAME_STATE");
                }
                else
                {
                    if(AmIhost)
                        continue;
                    //for players score stats
                    foreach (Player player in m_CurrentLobby.Players)
                    {
                        if (player.Id == key)
                        {
                            LobbyEvents.OnChangedPlayerScore?.Invoke(key, data);
                            //ActionOnChangedPlayersStatData?.Invoke(key, data);
                            break;
                        }
                    }
                }
            }
        }


        private void OnDataChanged(Dictionary<string, ChangedOrRemovedLobbyValue<DataObject>> changedData)
        {
            string key;
            string data;
            Debug.Log($"UGSLobbyManager-OnDataChanged-Count:" + changedData.Count);
            if (m_CurrentLobby == null)
                return;

            foreach (var item in changedData)
            {
                key = item.Key;
                data = item.Value.Value.Value;
                Debug.Log($"UGSLobbyManager-OnDataChanged-Key:{key}, data=> {data}");

                if (key == UGSLobbyDataController.LOBBY_DATA_BULLET_COLOR || key == UGSLobbyDataController.LOBBY_DATA_BULLET_SIZE)
                {
                    ActionOnChangedGameBulletModeData?.Invoke();
                }
                else if (key == UGSLobbyDataController.LOBBY_DATA_RELAY_JOIN_CODE)
                {
                    Debug.Log($"UGSLobbyManager-OnDataChanged-LOBBY_DATA_RELAY_JOIN_CODE");
                    ActionOnChangedRelayJoinCode?.Invoke();
                }
                else if (key == UGSLobbyDataController.LOBBY_DATA_GAME_STATE)
                {
                    Debug.Log($"UGSLobbyManager-OnDataChanged-LOBBY_DATA_GAME_STATE");
                }
                else
                {
                    if(AmIhost)
                        continue;
                    //for players score stats
                    foreach (Player player in m_CurrentLobby.Players)
                    {
                        if (player.Id == key)
                        {
                            LobbyEvents.OnChangedPlayerScore?.Invoke(key, data);
                            //ActionOnChangedPlayersStatData?.Invoke(key, data);
                            break;
                        }
                    }
                }
            }
        }

        private void OnDataRemoved(Dictionary<string, ChangedOrRemovedLobbyValue<DataObject>> removedData)
        {
            string key;
            string data;
            Debug.Log($"UGSLobbyManager-OnDataRemoved-Count:" + removedData.Count);
            if (m_CurrentLobby == null)
                return;

            foreach (var item in removedData)
            {
                key = item.Key;
                data = item.Value.Value.Value;
                Debug.Log($"UGSLobbyManager-OnDataRemoved-Key:{key}, data=> {data}");

                if (key == UGSLobbyDataController.LOBBY_DATA_BULLET_COLOR || key == UGSLobbyDataController.LOBBY_DATA_BULLET_SIZE)
                {
                    ActionOnChangedGameBulletModeData?.Invoke();
                }
                else if (key == UGSLobbyDataController.LOBBY_DATA_RELAY_JOIN_CODE)
                {
                    Debug.Log($"UGSLobbyManager-OnDataRemoved-LOBBY_DATA_RELAY_JOIN_CODE");
                    ActionOnChangedRelayJoinCode?.Invoke();
                }
                else if (key == UGSLobbyDataController.LOBBY_DATA_GAME_STATE)
                {
                    Debug.Log($"UGSLobbyManager-OnDataRemoved-LOBBY_DATA_GAME_STATE");
                }
                else
                {
                    if(AmIhost)
                        continue;
                    //for players score stats
                    foreach (Player player in m_CurrentLobby.Players)
                    {
                        if (player.Id == key)
                        {
                            LobbyEvents.OnChangedPlayerScore?.Invoke(key, data);
                            // ActionOnChangedPlayersStatData?.Invoke(key, data);
                            break;
                        }
                    }
                }
            }
        }

        private void OnPlayerLeft(List<int> leftId)
        {
            Debug.Log($"UGSLobbyManager-OnPlayerLeft-leftId=>{leftId.ToStringFull()}");
            DebugPlayers();
        }

        private void OnLobbyDeleted()
        {
            Debug.Log("UGSLobbyManager-OnLobbyDeleted!");
        }

        private void OnKickedFromLobby()
        {
            Debug.Log("UGSLobbyManager-OnKickedFromLobby!");
        }

        private void OnLobbyEventConnectionStateChanged(LobbyEventConnectionState connectionState)
        {
            Debug.Log($"UGSLobbyManager-OnLobbyEventConnectionStateChanged:{connectionState}, m_CurrentLobby:{m_CurrentLobby}");
            if(connectionState == LobbyEventConnectionState.Unsubscribed)
            {
                UnsubscribeLobby();
                LobbyEvents.OnLeft?.Invoke();
            }
        }

        #endregion

        #endregion
    }
}
