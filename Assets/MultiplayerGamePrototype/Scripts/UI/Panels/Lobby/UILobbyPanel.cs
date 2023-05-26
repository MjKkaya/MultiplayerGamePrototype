using MultiplayerGamePrototype.Core;
using MultiplayerGamePrototype.Gameplay;
using MultiplayerGamePrototype.UGS.Managers;
using MultiplayerGamePrototype.UGS.DataControllers;
using MultiplayerGamePrototype.UI.Core;
using MultiplayerGamePrototype.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;


namespace MultiplayerGamePrototype.UI.Panels.LobbyPanel
{
    public class UILobbyPanel : UIBasePanel
    {
        [SerializeField] private Button m_StartGameButton;
        [SerializeField] private Button m_LeaveLobbyButton;
        [SerializeField] private TextMeshProUGUI m_InfoText;
        [SerializeField] private TextMeshProUGUI m_ButtonText;

        private bool m_IsRelayProgress;


        private void Awake()
        {
            Init();
        }

        public override void Init()
        {
            Debug.Log("UILobbyPanel-Init");
            UGSLobbyManager.ActionOnPlayerJoined += OnPlayerJoined;
            UGSNetworkManager.ActionOnClientStarted += OnClientStarted;
            UGSLobbyManager.ActionOnChangedHost += OnChangedLobbyHost;
            m_StartGameButton.onClick.AddListener(OnButtonClickedStartGame);
            m_LeaveLobbyButton.onClick.AddListener(OnButtonClickedLeaveLobby);
            m_IsRelayProgress = false;
        }

        private void OnEnable()
        {
            SetInfo();
            if (UGSLobbyManager.Singleton.AmIhost && !UGSNetworkManager.Singleton.IsListening)
                TryToStartRelayServer();
        }

        private void SetInfo()
        {
            Debug.Log($"UILobbyPanel-SetInfo-IsListening:{UGSNetworkManager.Singleton.IsListening}, AmIhost:{UGSLobbyManager.Singleton.AmIhost}");
            SetStartGameButton();
            Lobby lobby = UGSLobbyManager.CurrentLobby;
            m_InfoText.text = $"Name:{lobby.Name}, Code:{lobby.LobbyCode}\n" +
                $"Players:{lobby.Players.Count}/{lobby.MaxPlayers}, AvailableSlots:{lobby.AvailableSlots}, Private:{lobby.IsPrivate}, Password:{lobby.HasPassword}\n"  +
                $"Host:{lobby.HostId}";
        }

        private void SetStartGameButton()
        {
            if (UGSNetworkManager.Singleton.IsListening)
            {
                m_StartGameButton.interactable = true;
                m_ButtonText.text = UGSNetworkManager.Singleton.IsHost ? "Start" : "Ready";
            }
            else
            {
                m_StartGameButton.interactable = false;
                m_ButtonText.text = "Waiting";
            }
        }

        private async void TryToStartRelayServer()
        {
            Debug.Log($"UILobbyPanel-TryToStartRelayServer-m_IsRelayProgress:{m_IsRelayProgress}");
            if (m_IsRelayProgress)
                return;
            m_IsRelayProgress = true;
            m_IsRelayProgress = await UGSRelayManager.Singleton.AllocateRelayServerAndGetJoinCode(UGSLobbyManager.CurrentLobby.MaxPlayers);
            Debug.Log($"UILobbyPanel-TryToStartRelayServer-m_IsRelayProgress{m_IsRelayProgress}");
            //todo:If allocation to be fail?
        }

        private IEnumerator IEStartGameTimer()
        {
            string defaultText = m_ButtonText.text;
            int timer = 3;
            WaitForSeconds waitForSeconds = new(1);
            do
            {
                m_ButtonText.text = $"{defaultText} ({timer})";
                yield return waitForSeconds;
                timer--;

            } while (timer > 0);
            OnButtonClickedStartGame();
        }

        private void LeaveLobbyAsync()
        {
            Debug.Log("UILobbyPanel-LeaveLobbyAsync");
            m_LeaveLobbyButton.interactable = false;
            UGSLobbyManager.Singleton.RemovePlayerAsync(UGSAuthManager.MyPlayerId);
            UGSNetworkManager.Singleton.Shutdown();
        }

        #region Events

        private void OnClientStarted()
        {
            Debug.Log($"UILobbyPanel-OnClientStarted-IsListening:{UGSNetworkManager.Singleton.IsListening}");
            SetStartGameButton();
            if (UGSLobbyDataController.GetLobbyData(UGSLobbyDataController.LOBBY_DATA_GAME_STATE, 0) == (int)GameStateTypes.Paused)
                StartCoroutine(IEStartGameTimer());
        }

        private void OnPlayerJoined(List<string> players)
        {
            Debug.Log($"UILobbyPanel-OnPlayerJoined:{players.Count}");
        }

        private void OnChangedLobbyHost()
        {
            Debug.Log("UILobbyPanel-OnChangedLobbyHost");
            SetInfo();
            if (UGSLobbyManager.Singleton.AmIhost && !UGSNetworkManager.Singleton.IsListening)
                TryToStartRelayServer();
        }


        private void OnDestroy()
        {
            if(m_StartGameButton != null)
                m_StartGameButton.onClick.RemoveAllListeners();
            if(m_LeaveLobbyButton != null)
                m_LeaveLobbyButton.onClick.RemoveAllListeners();
            UGSNetworkManager.ActionOnClientStarted -= OnClientStarted;
            UGSLobbyManager.ActionOnPlayerJoined -= OnPlayerJoined;
            UGSLobbyManager.ActionOnChangedHost -= OnChangedLobbyHost;
            StopAllCoroutines();
        }

        #region Button Events

        private void OnButtonClickedStartGame()
        {
            m_StartGameButton.interactable = false;
            if(UGSNetworkManager.Singleton.IsHost)
                LoadingSceneManager.Singleton.LoadScene(SceneName.Gameplay);
        }

        private void OnButtonClickedLeaveLobby()
        {
            LeaveLobbyAsync();
        }

        #endregion

        #endregion
    }
}