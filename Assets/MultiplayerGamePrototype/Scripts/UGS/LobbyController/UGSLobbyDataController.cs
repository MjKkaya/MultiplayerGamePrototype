using MultiplayerGamePrototype.UGS.Managers;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;


namespace MultiplayerGamePrototype.UGS.LobbyController
{
    enum BulletColorTypes
    {
        Red = 0,
        Blue = 1,
        Green = 2,

        Max = 3
    }


    enum BulletSizeTypes
    {
        Standard = 0,
        Large = 1,
        Small = 2,

        Max = 3
    }


    public static class UGSLobbyDataController
    {
        #region Keys

        #region Lobby Key

        private static readonly string LOBBY_DATA_BULLET_COLOR = "LD_BC";
        private static readonly string LOBBY_DATA_BULLET_SIZE = "LD_BS";

        #endregion

        #region Player Key

        private static readonly string PLAYER_DATA_USERNAME = "PD_UN";    
        private static readonly string PLAYER_DATA_BULLET_COLOR = "PD_BC";    
        private static readonly string PLAYER_DATA_BULLET_SIZE = "PD_BS";

        #endregion

        #endregion


        #region Lobby Data

        public static CreateLobbyOptions CreateLobbyOption(string username)
        {
            CreateLobbyOptions lobbyOptions = new()
            {
                Data = new()
                {
                    { UGSAuthManager.MyPlayerId, new DataObject(DataObject.VisibilityOptions.Public, username) },
                    { LOBBY_DATA_BULLET_COLOR, new DataObject(DataObject.VisibilityOptions.Member, BulletColorTypes.Red.ToString()) },
                    { LOBBY_DATA_BULLET_SIZE, new DataObject(DataObject.VisibilityOptions.Member, BulletSizeTypes.Standard.ToString()) }
                },

                Player = UGSLobbyDataController.CreateLobbyPlayer(username)
            };

            return lobbyOptions;
        }

        public static UpdateLobbyOptions CreateRandomLobbyBulletMode()
        {
            int randomColor = Random.Range(0, (int)BulletColorTypes.Max);
            int randomSize = Random.Range(0, (int)BulletSizeTypes.Max);

            Debug.Log($"UGSLobbyDataController-CreateRandomLobbyBulletMode-randomColor:{randomColor}, randomSize:{randomSize}");

            UpdateLobbyOptions lobbyOptions = new()
            {
                Data = new()
                {
                    { LOBBY_DATA_BULLET_COLOR, new DataObject(DataObject.VisibilityOptions.Member, ((BulletColorTypes)randomColor).ToString()) },
                    { LOBBY_DATA_BULLET_SIZE, new DataObject(DataObject.VisibilityOptions.Member, ((BulletSizeTypes)randomSize).ToString()) }
                }
            };

            return lobbyOptions;
        }

        public static string GetLobbyBulletMode()
        {
            Lobby currentLobby = UGSLobbyManager.CurrentLobby;
            string bulletMode = "-Null-"; ;
            DataObject bulletColor;
            DataObject bulletSize;
            if (currentLobby != null)
            {
                currentLobby.Data.TryGetValue(LOBBY_DATA_BULLET_COLOR, out bulletColor);
                currentLobby.Data.TryGetValue(LOBBY_DATA_BULLET_SIZE, out bulletSize);

                if (bulletColor != null && bulletSize != null)
                    bulletMode = bulletColor.Value + "-" + bulletSize.Value;
            }

            return bulletMode;
        }

        #endregion


        #region Player Data

        public static Player CreateLobbyPlayer(string username)
        {
            Debug.Log($"UGSLobbyDataController-CreateLobbyPlayer:{username}");
            Player player = new(
                id: UGSAuthManager.MyPlayerId,
                data: new Dictionary<string, PlayerDataObject>()
                {
                    { PLAYER_DATA_USERNAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, username)},
                    { PLAYER_DATA_BULLET_COLOR, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Private, BulletColorTypes.Red.ToString())},
                    { PLAYER_DATA_BULLET_SIZE, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Private, BulletSizeTypes.Standard.ToString())}
                });

            return player;
        }

        public static UpdatePlayerOptions CreateNewPlayerBulletData(string bulletColor, string bulletSize)
        {
            UpdatePlayerOptions playerOptions = new()
            {
                Data = new()
                {
                    { PLAYER_DATA_BULLET_COLOR, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Private, bulletColor)},
                    { PLAYER_DATA_BULLET_SIZE, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Private, bulletSize)}
                }
            };

            return playerOptions;
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

        #endregion
    }
}
