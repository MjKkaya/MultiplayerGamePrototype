using DG.Tweening;
using MultiplayerGamePrototype.Core;
using MultiplayerGamePrototype.UGS.Managers;
using MultiplayerGamePrototype.UI.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace MultiplayerGamePrototype.UI.Panels
{
    public class UIMainMenuPanel : UIBasePanel
    {
        [SerializeField] Button m_CreateGameButton;
        [SerializeField] Button m_QuickJoinButton;
        [SerializeField] Button m_JoinGameButton;
        [SerializeField] TMP_InputField m_LobbyCodeInput;


        private void OnEnable()
        {
            Init();
        }

        public override void Init()
        {
            UGSAuthManager.ActionOnCompletedSignIn += OnCompletedSignIn;
            UGSNetworkManager.ActionOnStartedServer += OnStartedServer;
            m_QuickJoinButton.onClick.AddListener(OnButtonClickedQuickJoin);
            m_CreateGameButton.onClick.AddListener(OnButtonClickedCreateGame);
            m_JoinGameButton.onClick.AddListener(OnButtonClickedJoinGame);
        }


        private async void CreateLobby()
        {
            SetInteractablePanelButtons(false);
            bool isSucceed = await UGSLobbyManager.Singleton.CreateLobbyAsync();
            if(!isSucceed)
                SetInteractablePanelButtons(true);
        }

        private async void QuickJoin()
        {
            SetInteractablePanelButtons(false);
            bool isSucceed = await UGSLobbyManager.Singleton.QuickJoinLobbyAsync();
            if (!isSucceed)
                CreateLobby();
        }

        private async void JoinLobby()
        {
            string lobbyCode = GetLobbyCode();
            if (lobbyCode != null)
            {
                SetInteractablePanelButtons(false);
                bool isSucceed = await UGSLobbyManager.Singleton.JoinLobbyByCodeAsync(lobbyCode);
                if (!isSucceed)
                    SetInteractablePanelButtons(true);



                //bool isSucceed = await UGSLobbyManager.Instance.QuickJoinLobbyAsync(Username);
                //if (!isSucceed)
                //    CreateLobby();
                //await UGSLobbyManager.Instance.GetLobbyListAsync();
            }
        }

        private void SetInteractablePanelButtons(bool isActive)
        {
            m_CreateGameButton.interactable = isActive;
            m_QuickJoinButton.interactable = isActive;
            m_JoinGameButton.interactable = isActive;
        }

        private string GetLobbyCode()
        {
            if (m_LobbyCodeInput.text.Length > 0)
            {
                return m_LobbyCodeInput.text;
            }
            else
            {
                m_LobbyCodeInput.transform.DOShakePosition(0.75f, 30 ,100, 360);
                return null;
            }
        }



        #region Events

        private void OnCompletedSignIn()
        {
            Show();
        }

        private void OnStartedServer()
        {
            LoadingSceneManager.Singleton.LoadScene(SceneName.Lobby);
        }

        private void OnDestroy()
        {
            UGSAuthManager.ActionOnCompletedSignIn -= OnCompletedSignIn;
            UGSNetworkManager.ActionOnStartedServer -= OnStartedServer;
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

        private void OnButtonClickedQuickJoin()
        {
            QuickJoin();
        }

        private void OnButtonClickedJoinGame()
        {
            JoinLobby();
        }

        #endregion

        #endregion
    }
}