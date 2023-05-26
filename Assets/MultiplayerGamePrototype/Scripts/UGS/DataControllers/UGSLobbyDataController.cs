using MultiplayerGamePrototype.UGS.Managers;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;


namespace MultiplayerGamePrototype.UGS.DataControllers
{
    public enum BulletColorTypes
    {
        Red = 0,
        Blue = 1,
        Green = 2,

        Max = 3
    }


    public enum BulletSizeTypes
    {
        Standard = 0,
        Large = 1,
        Small = 2,

        Max = 3
    }

    public enum GameStateTypes
    {
        Preparation = 0,
        Started = 1,
        Paused = 2,
        Finished = 3
    }


    public static class UGSLobbyDataController
    {
        #region Keys

        public static readonly string LOBBY_DATA_BULLET_COLOR = "LD_BC";
        public static readonly string LOBBY_DATA_BULLET_SIZE = "LD_BS";
        public static readonly string LOBBY_DATA_RELAY_JOIN_CODE = "LD_RJC";
        public static readonly string LOBBY_DATA_GAME_STATE = "LD_GS";

        #endregion


        public static CreateLobbyOptions CreateBaseLobbyData(string username)
        {
            Dictionary<string, DataObject> data = CreateRandomLobbyBulletData();
            //This data is using for player's point stat.
            data.Add(UGSAuthManager.MyPlayerId, new DataObject(DataObject.VisibilityOptions.Member, "0"));

            CreateLobbyOptions lobbyOptions = new()
            {
                Data = data,
                Player = UGSPlayerDataController.CreateLobbyPlayer(username)
            };

            return lobbyOptions;
        }


        public static bool IsMyBulletTypeEqualsToLobbyBulletType()
        {
            return IsPlayerBulletTypeEqualsToLobbyBulletType(UGSAuthManager.MyPlayerId);
        }

        public static bool IsPlayerBulletTypeEqualsToLobbyBulletType(string playerId)
        {
            BulletColorTypes playerBulletColor = UGSPlayerDataController.GetPlayerBulletColorType(playerId);
            BulletColorTypes lobbyBulletColor = GetLobbyBulletColorType();

            BulletSizeTypes playerBulletSize = UGSPlayerDataController.GetPlayerBulletSizeType(playerId);
            BulletSizeTypes lobbyBulletSize = GetLobbyBulletSizeType();

            Debug.Log($"UGSLobbyDataController-IsPlayerBulletTypeEqualsToLobbyBulletType-playerId:{playerId}=>BulletColor:{playerBulletColor}/{lobbyBulletColor}, BulletSize:{playerBulletSize}/{lobbyBulletSize}");
            return playerBulletColor == lobbyBulletColor && playerBulletSize == lobbyBulletSize;
        }


        #region Initial Data

        public static UpdateLobbyOptions CreateInitialData(string relayJoinCode)
        {
            Debug.Log($"UGSLobbyDataController-CreateInitialData-relayJoinCode:{relayJoinCode}");

            UpdateLobbyOptions lobbyOptions = new()
            {
                Data = new Dictionary<string, DataObject>()
                {
                    { LOBBY_DATA_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) },
                    { LOBBY_DATA_GAME_STATE, new DataObject(DataObject.VisibilityOptions.Public, ((int)GameStateTypes.Preparation).ToString(), DataObject.IndexOptions.N1) }
                }
            };

            return lobbyOptions;
        }

        public static UpdateLobbyOptions UpdateGameState(GameStateTypes gameStateType)
        {
            Debug.Log($"UGSLobbyDataController-UpdateGameState-gameStateType:{gameStateType}");

            UpdateLobbyOptions lobbyOptions = new()
            {
                Data = new Dictionary<string, DataObject>()
                {
                    { LOBBY_DATA_GAME_STATE, new DataObject(DataObject.VisibilityOptions.Public, ((int)gameStateType).ToString(), DataObject.IndexOptions.N1) }
                }
            };

            return lobbyOptions;
        }

        //public static string GetRelayJoinCode()
        //{
        //    string joinCode = null;
        //    Lobby currentLobby = UGSLobbyManager.CurrentLobby;

        //    if (currentLobby != null)
        //    {
        //        currentLobby.Data.TryGetValue(LOBBY_DATA_RELAY_JOIN_CODE, out DataObject dataObject);
        //        joinCode = dataObject.Value;
        //    }

        //    return joinCode;
        //}

        public static string GetLobbyData(string key)
        {
            string stringData = null;
            Lobby currentLobby = UGSLobbyManager.CurrentLobby;

            if (currentLobby != null)
            {
                currentLobby.Data.TryGetValue(key, out DataObject dataObject);
                stringData = dataObject.Value;
            }

            return stringData;
        }

        public static int GetLobbyData(string key, int defaultValue = 0)
        {
            int intData = defaultValue;
            Lobby currentLobby = UGSLobbyManager.CurrentLobby;

            if (currentLobby != null)
            {
                currentLobby.Data.TryGetValue(key, out DataObject dataObject);
                int.TryParse(dataObject.Value, out intData);
            }

            return intData;
        }

        #endregion


        #region Bullet Type

        public static UpdateLobbyOptions CreateRandomLobbyBulletMode()
        {
            UpdateLobbyOptions lobbyOptions = new()
            {
                Data = CreateRandomLobbyBulletData()
            };
            return lobbyOptions;
        }

        private static Dictionary<string, DataObject> CreateRandomLobbyBulletData()
        {
            string bulletColor = ((BulletColorTypes)(Random.Range(0, (int)BulletColorTypes.Max))).ToString();
            string bulletSize = ((BulletSizeTypes)(Random.Range(0, (int)BulletSizeTypes.Max))).ToString();
            Debug.Log($"UGSLobbyDataController-CreateRandomLobbyBulletData-bulletColor:{bulletColor}, bulletSize:{bulletSize}");

            Dictionary<string, DataObject> bulletData = new()
            {
                { LOBBY_DATA_BULLET_COLOR, new DataObject(DataObject.VisibilityOptions.Member, bulletColor) },
                { LOBBY_DATA_BULLET_SIZE, new DataObject(DataObject.VisibilityOptions.Member, bulletSize) }
            };

            return bulletData;
        }

        public static string GetLobbyBulletMode()
        {
            Lobby currentLobby = UGSLobbyManager.CurrentLobby;
            string bulletMode = "-Null-"; ;
            if (currentLobby != null)
            {
                currentLobby.Data.TryGetValue(LOBBY_DATA_BULLET_COLOR, out DataObject bulletColor);
                currentLobby.Data.TryGetValue(LOBBY_DATA_BULLET_SIZE, out DataObject bulletSize);

                if (bulletColor != null && bulletSize != null)
                    bulletMode = bulletColor.Value + "-" + bulletSize.Value;
            }

            return bulletMode;
        }

        private static BulletColorTypes GetLobbyBulletColorType()
        {
            Lobby currentLobby = UGSLobbyManager.CurrentLobby;
            BulletColorTypes bulletColorTypes = BulletColorTypes.Red;

            if (currentLobby != null)
            {
                currentLobby.Data.TryGetValue(LOBBY_DATA_BULLET_COLOR, out DataObject bulletColor);
                if (bulletColor != null)
                    System.Enum.TryParse(bulletColor.Value, out bulletColorTypes);
            }

            return bulletColorTypes;
        }

        private static BulletSizeTypes GetLobbyBulletSizeType()
        {
            Lobby currentLobby = UGSLobbyManager.CurrentLobby;
            BulletSizeTypes bulletSizeTypes = BulletSizeTypes.Standard;

            if (currentLobby != null)
            {
                currentLobby.Data.TryGetValue(LOBBY_DATA_BULLET_SIZE, out DataObject bulletSize);
                if (bulletSize != null)
                    System.Enum.TryParse(bulletSize.Value, out bulletSizeTypes);
            }

            return bulletSizeTypes;
        }

        #endregion


        #region Score Stats

        public static UpdateLobbyOptions CreateNewLobbyPlayersStatsData(List<string> playerIds)
        {
            Dictionary<string, DataObject> newPlayerData = new();
            foreach (string playerId in playerIds)
            {
                newPlayerData.Add(playerId, new DataObject(DataObject.VisibilityOptions.Member, "0"));
            }

            UpdateLobbyOptions lobbyOptions = new()
            {
                Data = newPlayerData
            };
            return lobbyOptions;
        }

        public static int GetPlayerScoreStat(string playerId)
        {
            int score = 0;

            if (UGSLobbyManager.CurrentLobby.Data.ContainsKey(playerId))
            {
                DataObject dataObject;
                UGSLobbyManager.CurrentLobby.Data.TryGetValue(playerId, out dataObject);

                if (dataObject != null)
                    int.TryParse(dataObject.Value, out score);
            }

            return score;
        }

        public static UpdateLobbyOptions IncreasePlayerScoreStat(string playerId, int additionScore)
        {
            int score = GetPlayerScoreStat(playerId);
            Debug.Log($"UGSLobbyDataController-IncreasePlayerScoreStat-playerId:{playerId}, score:{score}, additionScore:{additionScore}");
            score += additionScore;
            Debug.Log($"UGSLobbyDataController-IncreasePlayerScoreStat-score:{score}");

            UpdateLobbyOptions lobbyOptions = new()
            {
                Data = new Dictionary<string, DataObject> () { { playerId, new DataObject(DataObject.VisibilityOptions.Member, score.ToString()) } }
            };

            return lobbyOptions;
        }

        #endregion
    }
}
