using System.Collections;
using MultiplayerGamePrototype.Core;
using MultiplayerGamePrototype.Events;
using MultiplayerGamePrototype.UGS.DataControllers;
using MultiplayerGamePrototype.UGS.Managers;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UIElements;


namespace MultiplayerGamePrototype.UI.UIToolkit.MainScene
{
    [RequireComponent(typeof(UIDocument))]
    public class LobbySceneUIController : MonoBehaviour
    {
        private const string _startButtonName = "start-button";
        private const string _leaveButtonName = "leave-button";
        private const string _infoLabelName = "info-label";

        private VisualElement _rootElement;
        private Button _startButton;
        private Button _leaveButton;
        private Label _infoLabel;

        private bool m_IsRelayProgress;


        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            Debug.Log("LobbySceneUIController-Initialize");
            m_IsRelayProgress = false;
            _rootElement = GetComponent<UIDocument>().rootVisualElement;
            SetVisualElements();
            NetworkManagerEvents.OnClientStarted += NetworkManagerEvents_OnClientStarted;
            UGSLobbyManager.ActionOnChangedHost += OnChangedLobbyHost;
        }

        private void SetVisualElements()
        {
            _startButton = _rootElement.Q<Button>(_startButtonName);
            _startButton.RegisterCallback<ClickEvent>(OnClickedStartButton);

            _leaveButton = _rootElement.Q<Button>(_leaveButtonName);
            _leaveButton.RegisterCallback<ClickEvent>(OnClickedLeaveButton);
            
            _infoLabel = _rootElement.Q<Label>(_infoLabelName);
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
            _infoLabel.text = $"Name:{lobby.Name}, Code:{lobby.LobbyCode}\n" +
                $"Players:{lobby.Players.Count}/{lobby.MaxPlayers}, AvailableSlots:{lobby.AvailableSlots}, Private:{lobby.IsPrivate}, Password:{lobby.HasPassword}\n"  +
                $"Host:{lobby.HostId}";
        }

        private void SetStartGameButton()
        {
            if (UGSNetworkManager.Singleton.IsListening)
            {
                _startButton.SetEnabled(true);
                _startButton.text = UGSNetworkManager.Singleton.IsHost ? "Start" : "Ready";
            }
            else
            {
                _startButton.SetEnabled(false);
                _startButton.text = "Waiting";
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
            string defaultText = _startButton.text;
            int timer = 3;
            WaitForSeconds waitForSeconds = new(1);
            do
            {
                _startButton.text = $"{defaultText} ({timer})";
                yield return waitForSeconds;
                timer--;

            } while (timer > 0);
            OnClickedStartButton(null);
        }

        private void LeaveLobbyAsync()
        {
            Debug.Log("UILobbyPanel-LeaveLobbyAsync");
            LobbyEvents.Leave?.Invoke(UGSAuthManager.MyPlayerId);
        }

        #region Events

        private void NetworkManagerEvents_OnClientStarted()
        {
            Debug.Log($"UILobbyPanel-OnClientStarted-IsListening:{UGSNetworkManager.Singleton.IsListening}");
            SetStartGameButton();
            if (UGSLobbyDataController.GetLobbyData(UGSLobbyDataController.LOBBY_DATA_GAME_STATE, 0) == (int)GameStateTypes.Paused)
                StartCoroutine(IEStartGameTimer());
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
            _startButton.UnregisterCallback<ClickEvent>(OnClickedStartButton);
            _leaveButton.UnregisterCallback<ClickEvent>(OnClickedLeaveButton);

            NetworkManagerEvents.OnClientStarted -= NetworkManagerEvents_OnClientStarted;
            UGSLobbyManager.ActionOnChangedHost -= OnChangedLobbyHost;
            StopAllCoroutines();
        }

        #region Button Events

        private void OnClickedStartButton(ClickEvent evt)
        {
            _startButton.SetEnabled(false);
            if(UGSNetworkManager.Singleton.IsHost)
                SceneLoadingManager.Singleton.LoadScene(SceneName.Gameplay);
        }

        private void OnClickedLeaveButton(ClickEvent evt)
        {
            _leaveButton.SetEnabled(false);
            LeaveLobbyAsync();
        }

        #endregion

        #endregion
    }
}