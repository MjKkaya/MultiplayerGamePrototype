using MultiplayerGamePrototype.UI.Managers;
using MultiplayerGamePrototype.UGS.Managers;
using MultiplayerGamePrototype.UGS.DataControllers;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


namespace MultiplayerGamePrototype.UI.Panels.GamePanels
{
    public class UIGamePanel : UIBasePanel
    {
        [Header("Game Bullet Mode")]
        [SerializeField] private TextMeshProUGUI m_GameBulletModeText;
        [SerializeField] private Button m_ChangeGameBulletModeButton;

        [Header("My Bullet Mode")]
        [SerializeField] private TextMeshProUGUI m_MyBulletModeText;
        [SerializeField] private Button m_ChangeMyBulletModeButton;

        [Header("Lobby Info")]
        [SerializeField] private TextMeshProUGUI m_LobbyInfoText;
        [SerializeField] private Button m_StatsPanelControlButton;

        private bool m_IsPlayerSocrePanelOpen;

        public override void Init()
        {
            UGSLobbyManager.ActionOnJoinedLobby += OnJoinedLobby;
            UGSLobbyManager.ActionOnChangedGameBulletModeData += OnChangedGameBulletModeData;
            UGSLobbyManager.ActionOnChangedMyPlayerData += OnChangedMyPlayerData;
            m_ChangeGameBulletModeButton.onClick.AddListener(OnButtonClickedChangeGameBulletMode);
            m_ChangeMyBulletModeButton.onClick.AddListener(OnButtonClickedChangeMyBulletMode);
            m_StatsPanelControlButton.onClick.AddListener(OnButtonClickedStatsPanelControl);
        }

        public override void Show()
        {
            SetGameMode();
            SetMyBulletMode();
            m_ChangeGameBulletModeButton.gameObject.SetActive(UGSLobbyManager.AmIhost);
            m_LobbyInfoText.text = $"Id: {UGSLobbyManager.CurrentLobby.Id} \n Code:{UGSLobbyManager.CurrentLobby.LobbyCode}";
            m_IsPlayerSocrePanelOpen = false;
            base.Show();
        }


        private void SetGameMode()
        {
            m_GameBulletModeText.text = UGSLobbyDataController.GetLobbyBulletMode();
        } 
        
        public void SetMyBulletMode()
        {
            m_MyBulletModeText.text = UGSPlayerDataController.GetMyBulletMode();
        }


        private async void ChangeLobbyBulletMode()
        {
            m_ChangeGameBulletModeButton.interactable = false;
            await UGSLobbyManager.Singleton.UpdateLobbyDataAsync(UGSLobbyDataController.CreateRandomLobbyBulletMode());
            m_ChangeGameBulletModeButton.interactable = true;
        }

        #region Events

        private void OnJoinedLobby()
        {
            Show();
        }

        private void OnChangedGameBulletModeData()
        {
            SetGameMode();
        }

        private void OnChangedMyPlayerData()
        {
            Debug.Log("UIGameModePanel-OnChangedMyPlayerData");
            SetMyBulletMode();
        }

        private void OnDestroy()
        {
            UGSLobbyManager.ActionOnJoinedLobby -= OnJoinedLobby;
            UGSLobbyManager.ActionOnChangedGameBulletModeData -= OnChangedGameBulletModeData;
            UGSLobbyManager.ActionOnChangedMyPlayerData -= OnChangedMyPlayerData;
            if (m_ChangeGameBulletModeButton != null)
                m_ChangeGameBulletModeButton.onClick.RemoveAllListeners();
            if (m_ChangeMyBulletModeButton != null)
                m_ChangeMyBulletModeButton.onClick.RemoveAllListeners();
        }


        #region Button events

        private void OnButtonClickedChangeGameBulletMode()
        {
            ChangeLobbyBulletMode();
        }

        private void OnButtonClickedChangeMyBulletMode()
        {
            PopupsManager.Singleton.ShowPopup(PopupTypes.PlayerGameModeChange);
        }

        private void OnButtonClickedStatsPanelControl()
        {
            if(!m_IsPlayerSocrePanelOpen)
                PanelsManager.Singleton.ShowPanel(PanelTypes.GamePlayerScore);
            else
                PanelsManager.Singleton.HidePanel(PanelTypes.GamePlayerScore);
            m_IsPlayerSocrePanelOpen = !m_IsPlayerSocrePanelOpen;
        }

        #endregion

        #endregion
    }
}