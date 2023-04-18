using MultiplayerGamePrototype.UGS.Managers;
using MultiplayerGamePrototype.UGS.DataControllers;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;


namespace MultiplayerGamePrototype.UI.Panels.GamePanels
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
            int playerScore;
            foreach (string newPlayerId in newPlayerIds)
            {
                if (!m_ScoreItems.ContainsKey(newPlayerId))
                {
                    playerScore = UGSLobbyDataController.GetPlayerScoreStat(newPlayerId);
                    CreateScoreStatItem(UGSLobbyManager.Instance.GetPlayer(newPlayerId), playerScore);
                }
            }
        }

        private void CreateScoreStatItem(Player player, int score)
        {
            string username = UGSPlayerDataController.GetPlayerUsernameData(player);
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
        private void OnChangedPlayersStatData(Dictionary<string, string> changedPlayerDatas)
        {
            string playerId;
            int playerScore = 0;
            foreach (var item in changedPlayerDatas)
            {
                playerId = item.Key;
                int.TryParse(item.Value, out playerScore);
                Debug.Log($"UIGamePlayersScorePanel-OnChangedPlayersStatData-playerId:{playerId}, playerScore:{playerScore}");

                if(m_ScoreItems.ContainsKey(item.Key))
                {
                    m_ScoreItems[item.Key].UpdateScore(item.Value);
                }
                else
                {
                    CreateScoreStatItem(UGSLobbyManager.Instance.GetPlayer(playerId), playerScore);
                }
            }
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