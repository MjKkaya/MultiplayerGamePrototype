using MultiplayerGamePrototype.UGS.Managers;
using MultiplayerGamePrototype.UGS.DataControllers;
using MultiplayerGamePrototype.UI.Core;
using MultiplayerGamePrototype.UI.Panels.Gameplay.Items;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;


namespace MultiplayerGamePrototype.UI.Panels.Gameplay
{
    public class UIGamePlayersScorePanel : UIBasePanel
    {
        [SerializeField] private UIPlayerScoreStatItem m_ScoreItemPrefab;
        [SerializeField] private Transform m_ScoreItemContainer;

        /// <summary>
        /// string : UGS PlayerId
        /// </summary>
        private Dictionary<string, UIPlayerScoreStatItem> m_ScoreItems;


        public override void Init()
        {
            base.Init();
            UGSLobbyManager.ActionOnJoinedLobby += OnJoinedLobby;
            UGSLobbyManager.ActionOnPlayerJoined += OnPlayerJoined;
            UGSLobbyManager.ActionOnChangedPlayersStatData += OnChangedPlayersStatData;
        }


        private void CreateStatItemsForAllLobbyPlayer()
        {
            m_ScoreItems = new();
            List<Player> players = UGSLobbyManager.CurrentLobby.Players;
            int playerScore;
            foreach (Player player in players)
            {
                playerScore = UGSLobbyDataController.GetPlayerScoreStat(player.Id);
                CreateScoreStatItem(player, playerScore);
            }
        }

        private void AddNewItemForNewPlayer(List<string> newPlayerIds)
        {
            Debug.Log("UIGamePlayersScorePanel-AddNewItemForNewPlayer");
            int playerScore;
            foreach (string newPlayerId in newPlayerIds)
            {
                Debug.Log($"UIGamePlayersScorePanel-AddNewItemForNewPlayer-newPlayerId:{newPlayerId}");
                if (!m_ScoreItems.ContainsKey(newPlayerId))
                {
                    playerScore = UGSLobbyDataController.GetPlayerScoreStat(newPlayerId);
                    CreateScoreStatItem(UGSLobbyManager.Singleton.GetPlayer(newPlayerId), playerScore);
                }
            }
        }

        private void CreateScoreStatItem(Player player, int score)
        {
            if(player == null){
                Debug.Log("UIGamePlayersScorePanel-CreateScoreStatItem-player is null!");
                return;
            }
            string username = UGSPlayerDataController.GetPlayerUsername(player);
            UIPlayerScoreStatItem statItem = Instantiate(m_ScoreItemPrefab, m_ScoreItemContainer);
            statItem.Init(player.Id, username, score);
            m_ScoreItems.TryAdd(player.Id, statItem);
        }


        #region Events

        private void OnJoinedLobby()
        {
            CreateStatItemsForAllLobbyPlayer();
        }

        private void OnPlayerJoined(List<string> newPlayerIds)
        {
            AddNewItemForNewPlayer(newPlayerIds);
        }

        /// <summary>
        /// Dictionary item has playerIds and new totol score.
        /// </summary>
        /// <param name="changedDataDict"></param>
        private void OnChangedPlayersStatData(string playerId, string newScore)
        {
            int playerScore = 0;
            int.TryParse(newScore, out playerScore);
            Debug.Log($"UIGamePlayersScorePanel-OnChangedPlayersStatData-playerId:{playerId}, playerScore:{playerScore}");

            if(m_ScoreItems.ContainsKey(playerId))
                m_ScoreItems[playerId].UpdateScore(playerScore);
            else
                CreateScoreStatItem(UGSLobbyManager.Singleton.GetPlayer(playerId), playerScore);
        }

        private void OnDestroy()
        {
            UGSLobbyManager.ActionOnJoinedLobby -= OnJoinedLobby;
            UGSLobbyManager.ActionOnPlayerJoined -= OnPlayerJoined;
            UGSLobbyManager.ActionOnChangedPlayersStatData -= OnChangedPlayersStatData;
        }

        #endregion




    }
}