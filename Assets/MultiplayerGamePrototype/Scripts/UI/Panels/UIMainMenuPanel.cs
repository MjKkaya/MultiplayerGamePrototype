using DG.Tweening;
using MultiplayerGamePrototype.UGS.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace MultiplayerGamePrototype.UI.Panels
{
    public class UIMainMenuPanel : UIBasePanel
    {
        private string Username{
            get{
                return m_UsernameInput.text.Length == 0 ? "Player-" : m_UsernameInput.text;
            }
        }


        [SerializeField] TMP_InputField m_UsernameInput;
        [SerializeField] Button m_CreateGameButton;
        [SerializeField] Button m_JoinGameButton;


        public override void Init()
        {
            UGSAuthManager.ActionOnCompletedSignIn += OnCompletedSignIn;
            UGSLobbyManager.ActionOnJoinedLobby += OnJoinedLobby;
            m_CreateGameButton.onClick.AddListener(OnButtonClickedCreateGame);
            m_JoinGameButton.onClick.AddListener(OnButtonClickedJoinGame);
        }


        private async void CreateLobby()
        {
            string username = GetUsername();
            if (username != null)
            {
                SetInteractablePanelButtons(false);
                bool isSucceed = await UGSLobbyManager.Instance.CreateLobbyAsync("Lobby-1", Username);
                if(!isSucceed)
                    SetInteractablePanelButtons(true);
            }
        }

        private async void JoinLobby()
        {
            string username = GetUsername();
            if (username != null)
            {
                SetInteractablePanelButtons(false);
                bool isSucceed = await UGSLobbyManager.Instance.QuickJoinLobbyAsync(Username);
                if (!isSucceed)
                    CreateLobby();
            }
        }

        private void SetInteractablePanelButtons(bool isActive)
        {
            m_CreateGameButton.interactable = isActive;
            m_JoinGameButton.interactable = isActive;
        }

        private string GetUsername()
        {
            if (m_UsernameInput.text.Length > 0)
            {
                return m_UsernameInput.text;
            }
            else
            {
                m_UsernameInput.transform.DOShakePosition(0.75f, 30 ,100, 360);
                return null;
            }
        }


        #region Events

        private void OnCompletedSignIn()
        {
            Show();
        }

        private void OnJoinedLobby()
        {
            Hide();
        }

        private void OnDestroy()
        {
            UGSAuthManager.ActionOnCompletedSignIn -= OnCompletedSignIn;
            UGSLobbyManager.ActionOnJoinedLobby -= OnJoinedLobby;
            if (m_JoinGameButton != null)
                m_JoinGameButton.onClick.RemoveAllListeners();
            if (m_CreateGameButton != null)
                m_CreateGameButton.onClick.RemoveAllListeners();
        }

        #region Button Events

        private void OnButtonClickedCreateGame()
        {
            CreateLobby();

        }

        private void OnButtonClickedJoinGame()
        {
            JoinLobby();
        }

        #endregion

        #endregion
    }
}