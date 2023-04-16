using MultiplayerGamePrototype.UGS.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace MultiplayerGamePrototype.UI.Panels
{
    public class UIMainMenuPanel : UIBasePanel
    {
        [SerializeField] Button m_JoinGameButton;
        [SerializeField] TMP_InputField m_UsernameInput;


        public override void Init()
        {
            UGSAuthManager.ActionOnCompletedSignIn += OnCompletedSignIn;
            UGSLobbyManager.ActionOnJoinedLobby += OnJoinedLobby;
            m_JoinGameButton.onClick.AddListener(OnButtonClickedJoinGame);
        }


        private async void CreateLobby()
        {
            m_JoinGameButton.interactable = false;
            string username = m_UsernameInput.text;
            bool isSucceed = await UGSLobbyManager.Instance.CreateLobbyAsync("Lobby-1", 4, null);
            if(!isSucceed)
                m_JoinGameButton.interactable = true;
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
        }

        #region Button Events

        private void OnButtonClickedJoinGame()
        {
            CreateLobby();
        }

        #endregion

        #endregion
    }
}