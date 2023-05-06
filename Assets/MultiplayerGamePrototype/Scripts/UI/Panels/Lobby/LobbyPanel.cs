using MultiplayerGamePrototype.Core;
using MultiplayerGamePrototype.UGS.Managers;
using MultiplayerGamePrototype.UI.Core;
using MultiplayerGamePrototype.Utilities;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;


namespace MultiplayerGamePrototype.UI.Panels.LobbyPanel
{
    public class LobbyPanel : UIBasePanel
    {
        [SerializeField] private Button m_StartGameButton;
        [SerializeField] private TextMeshProUGUI m_InfoText;
        [SerializeField] private TextMeshProUGUI m_ButtonText;

        private bool m_IsHost; 


        private void Start()
        {
            Init();
        }

        public override void Init()
        {
            UGSLobbyManager.ActionOnPlayerJoined += OnPlayerJoined;
            m_StartGameButton.onClick.AddListener(OnButtonClickedStartGame);
        }

        private void OnEnable()
        {
            m_IsHost = UGSNetworkManager.Singleton.IsHost;
            SetInfo();
        }

        private void SetInfo()
        {
            m_ButtonText.text = m_IsHost ? "Start" : "Ready";
            Lobby lobby = UGSLobbyManager.CurrentLobby;
            m_InfoText.text = $"Name:{lobby.Name}, Code:{lobby.LobbyCode}\n" +
                $"Players:{lobby.Players.Count}/{lobby.MaxPlayers}, AvailableSlots:{lobby.AvailableSlots}, Private:{lobby.IsPrivate}, Password:{lobby.HasPassword}\n"  +
                $"Host:{lobby.HostId}";
        }

        #region Events

        private void OnPlayerJoined(List<string> players)
        {
            Debug.Log($"LobbyPanel-OnPlayerJoined:{players.Count}");
            SetInfo();
        }

        private void OnDestroy()
        {
            if(m_StartGameButton != null)
                m_StartGameButton.onClick.RemoveAllListeners();
            UGSLobbyManager.ActionOnPlayerJoined -= OnPlayerJoined;
        }

        #region Button Events

        private void OnButtonClickedStartGame()
        {
            m_StartGameButton.interactable = false;
            if(m_IsHost)
                LoadingSceneManager.Singleton.LoadScene(SceneName.Gameplay);
        }

        #endregion

        #endregion
    }
}