using System.Collections;
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

        public static string GetPlayerUsernameData(Player player)
        {
            string username = string.Empty;
            PlayerDataObject playerDataObject;
            player.Data.TryGetValue(PLAYER_DATA_USERNAME, out playerDataObject);
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
                Data = CreateRandomPlayerBulletData()
            };

            return playerOptions;
        }

        private static Dictionary<string, PlayerDataObject> CreateDefaultPlayerBulletData()
        {
            Debug.Log("UGSPlayerDataController-CreateDefaultPlayerBulletData");

            Dictionary<string, PlayerDataObject> playerDatas = new()
            {
                { PLAYER_DATA_BULLET_COLOR, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Private, BulletColorTypes.Red.ToString())},
                { PLAYER_DATA_BULLET_SIZE, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Private, BulletSizeTypes.Standard.ToString())}
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
                { PLAYER_DATA_BULLET_COLOR, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Private, bulletColor)},
                { PLAYER_DATA_BULLET_SIZE, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Private, bulletSize)}
            };

            return playerDatas;
        }


        public static string GetPlayerBulletMode(string playerId)
        {
            Lobby currentLobby = UGSLobbyManager.CurrentLobby;
            string bulletMode = "-Null-"; ;
            PlayerDataObject bulletColor;
            PlayerDataObject bulletSize;
            if (currentLobby != null)
            {
                foreach (Player player in currentLobby.Players)
                {
                    if (player.Id == playerId)
                    {
                        player.Data.TryGetValue(PLAYER_DATA_BULLET_COLOR, out bulletColor);
                        player.Data.TryGetValue(PLAYER_DATA_BULLET_SIZE, out bulletSize);

                        if (bulletColor != null && bulletSize != null)
                            bulletMode = bulletColor.Value + "-" + bulletSize.Value;

                        break;
                    }
                }
            }

            return bulletMode;
        }

        public static BulletColorTypes GetMyPlayerBulletColorType()
        {
            Lobby currentLobby = UGSLobbyManager.CurrentLobby;
            BulletColorTypes bulletColorType = BulletColorTypes.Red;
            PlayerDataObject bulletColor;
            if (currentLobby != null)
            {
                foreach (Player player in currentLobby.Players)
                {
                    if (player.Id == UGSAuthManager.MyPlayerId)
                    {
                        player.Data.TryGetValue(PLAYER_DATA_BULLET_COLOR, out bulletColor);
                        if (bulletColor != null)
                            System.Enum.TryParse(bulletColor.Value, out bulletColorType);

                        break;
                    }
                }
            }

            return bulletColorType;
        }

        public static BulletSizeTypes GetMyPlayerBulletSizeType()
        {
            Lobby currentLobby = UGSLobbyManager.CurrentLobby;
            BulletSizeTypes bulletSizeType = BulletSizeTypes.Standard;
            PlayerDataObject bulletColor;
            if (currentLobby != null)
            {
                foreach (Player player in currentLobby.Players)
                {
                    if (player.Id == UGSAuthManager.MyPlayerId)
                    {
                        player.Data.TryGetValue(PLAYER_DATA_BULLET_SIZE, out bulletColor);
                        if (bulletColor != null)
                            System.Enum.TryParse(bulletColor.Value, out bulletSizeType);

                        break;
                    }
                }
            }

            return bulletSizeType;
        }

        #endregion
    }
}