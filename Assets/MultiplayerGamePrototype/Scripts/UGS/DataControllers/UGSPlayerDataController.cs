using System.Collections.Generic;
using MultiplayerGamePrototype.UGS.Managers;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;


namespace MultiplayerGamePrototype.UGS.DataControllers
{
    public static class UGSPlayerDataController
    {
        #region Keys

        private static readonly string PLAYER_DATA_BULLET_COLOR = "PD_BC";
        private static readonly string PLAYER_DATA_BULLET_SIZE = "PD_BS";
        private static readonly string PLAYER_DATA_USERNAME = "PD_UN";

        #endregion


        #region Player

        public static Player CreateLobbyPlayer(string username)
        {
            Debug.Log($"UGSPlayerDataController-CreateLobbyPlayer:{username}");

            Dictionary<string, PlayerDataObject> playerDatas = CreateDefaultPlayerBulletData();
            playerDatas.Add(PLAYER_DATA_USERNAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, username));
            Player player = new(UGSAuthManager.MyPlayerId, null, playerDatas);

            return player;
        }

        #endregion


        #region Username

        public static string GetPlayerUsername(Player player)
        {
            string username = string.Empty;
            player.Data.TryGetValue(PLAYER_DATA_USERNAME, out PlayerDataObject playerDataObject);
            if (playerDataObject != null)
                username = playerDataObject.Value;
            return username;
        }

        #endregion


        #region Bullet Type

        public static UpdatePlayerOptions CreateNewPlayerBulletData(string bulletColor, string bulletSize)
        {
            UpdatePlayerOptions playerOptions = new()
            {
                Data = new()
                {
                    { PLAYER_DATA_BULLET_COLOR, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, bulletColor)},
                    { PLAYER_DATA_BULLET_SIZE, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, bulletSize)}
                }
            };

            return playerOptions;
        }

        private static Dictionary<string, PlayerDataObject> CreateDefaultPlayerBulletData()
        {
            Debug.Log("UGSPlayerDataController-CreateDefaultPlayerBulletData");

            Dictionary<string, PlayerDataObject> playerDatas = new()
            {
                { PLAYER_DATA_BULLET_COLOR, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, BulletColorTypes.Red.ToString())},
                { PLAYER_DATA_BULLET_SIZE, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, BulletSizeTypes.Standard.ToString())}
            };

            return playerDatas;
        }

        private static Dictionary<string, PlayerDataObject> CreateRandomPlayerBulletData()
        {
            string bulletColor = ((BulletColorTypes)(Random.Range(0, (int)BulletColorTypes.Max))).ToString();
            string bulletSize = ((BulletSizeTypes)(Random.Range(0, (int)BulletSizeTypes.Max))).ToString();
            Debug.Log($"UGSPlayerDataController-CreateRandomPlayerBulletData-bulletColor:{bulletColor}, bulletSize:{bulletSize}");

            Dictionary<string, PlayerDataObject> playerDatas = new()
            {
                { PLAYER_DATA_BULLET_COLOR, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, bulletColor)},
                { PLAYER_DATA_BULLET_SIZE, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, bulletSize)}
            };

            return playerDatas;
        }

        public static BulletColorTypes GetPlayerBulletColorType(string playerId)
        {
            Lobby currentLobby = UGSLobbyManager.CurrentLobby;
            BulletColorTypes bulletColorType = BulletColorTypes.Red;
            if (currentLobby != null)
            {
                foreach (Player player in currentLobby.Players)
                {
                    if (player.Id == playerId)
                    {
                        player.Data.TryGetValue(PLAYER_DATA_BULLET_COLOR, out PlayerDataObject bulletColor);
                        if (bulletColor != null)
                            System.Enum.TryParse(bulletColor.Value, out bulletColorType);

                        break;
                    }
                }
            }

            return bulletColorType;
        }

        public static BulletSizeTypes GetPlayerBulletSizeType(string playerId)
        {
            Lobby currentLobby = UGSLobbyManager.CurrentLobby;
            BulletSizeTypes bulletSizeType = BulletSizeTypes.Standard;
            if (currentLobby != null)
            {
                foreach (Player player in currentLobby.Players)
                {
                    if (player.Id == playerId)
                    {
                        player.Data.TryGetValue(PLAYER_DATA_BULLET_SIZE, out PlayerDataObject bulletColor);
                        if (bulletColor != null)
                            System.Enum.TryParse(bulletColor.Value, out bulletSizeType);

                        break;
                    }
                }
            }

            return bulletSizeType;
        }


        #region My Lobby Data

        public static string GetMyBulletMode()
        {
            Lobby currentLobby = UGSLobbyManager.CurrentLobby;
            string bulletMode = "-Null-"; ;
            if (currentLobby != null)
            {
                foreach (Player player in currentLobby.Players)
                {
                    if (player.Id == UGSAuthManager.MyPlayerId)
                    {
                        player.Data.TryGetValue(PLAYER_DATA_BULLET_COLOR, out PlayerDataObject bulletColor);
                        player.Data.TryGetValue(PLAYER_DATA_BULLET_SIZE, out PlayerDataObject bulletSize);

                        if (bulletColor != null && bulletSize != null)
                            bulletMode = bulletColor.Value + "-" + bulletSize.Value;

                        break;
                    }
                }
            }

            return bulletMode;
        }

        public static BulletColorTypes GetMyBulletColorType()
        {
            return GetPlayerBulletColorType(UGSAuthManager.MyPlayerId);
        }

        public static BulletSizeTypes GetMyBulletSizeType()
        {
            return GetPlayerBulletSizeType(UGSAuthManager.MyPlayerId);
        }

        #endregion

        #endregion
    }
}