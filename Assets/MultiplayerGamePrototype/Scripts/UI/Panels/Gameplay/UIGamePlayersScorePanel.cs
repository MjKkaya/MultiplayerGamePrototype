using System.Collections.Generic;
using MultiplayerGamePrototype.Events;
using MultiplayerGamePrototype.UGS.DataControllers;
using MultiplayerGamePrototype.UGS.Managers;
using MultiplayerGamePrototype.UI.Core;
using MultiplayerGamePrototype.UI.Panels.Gameplay.Items;
using Unity.Services.Lobbies.Models;
using UnityEngine;


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
            UGSLobbyManager.ActionOnPlayerJoined += OnPlayerJoined;
            LobbyEvents.OnChangedPlayerScore += LobbyEvents_OnChangedPlayerScore;
            CreateStatItemsForAllLobbyPlayer();
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


        private void OnPlayerJoined(List<string> newPlayerIds)
        {
            AddNewItemForNewPlayer(newPlayerIds);
        }

        /// <summary>
        /// When any player's score data changes, this method triggers! 
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="newScore"></param>
        private void LobbyEvents_OnChangedPlayerScore(string playerId, string newScore)
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
            UGSLobbyManager.ActionOnPlayerJoined -= OnPlayerJoined;
            LobbyEvents.OnChangedPlayerScore -= LobbyEvents_OnChangedPlayerScore;
        }

        #endregion
    }
}