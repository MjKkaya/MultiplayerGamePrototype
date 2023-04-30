using MultiplayerGamePrototype.UGS.Managers;
using MultiplayerGamePrototype.UGS.DataControllers;
using MultiplayerGamePrototype.UI.Core;
using MultiplayerGamePrototype.UI.Panels.Gameplay.Popups;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


namespace MultiplayerGamePrototype.UI.Panels.Gameplay
{
    public class UIGameplayPanel : UIBasePanel
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
        [SerializeField] private Button m_LeaveButton;


        public override void Init()
        {
            base.Init();
            UGSLobbyManager.ActionOnChangedGameBulletModeData += OnChangedGameBulletModeData;
            UGSLobbyManager.ActionOnChangedMyPlayerData += OnChangedMyPlayerData;
            m_ChangeGameBulletModeButton.onClick.AddListener(OnButtonClickedChangeGameBulletMode);
            m_ChangeMyBulletModeButton.onClick.AddListener(OnButtonClickedChangeMyBulletMode);
            m_StatsPanelControlButton.onClick.AddListener(OnButtonClickedStatsPanelControl);
            m_LeaveButton.onClick.AddListener(OnButtonClickedLeave);
        }

        public override void Show()
        {
            SetGameMode();
            SetMyBulletMode();
            m_ChangeGameBulletModeButton.gameObject.SetActive(UGSLobbyManager.AmIhost);
            m_LobbyInfoText.text = $"Id: {UGSLobbyManager.CurrentLobby.Id} \n Code:{UGSLobbyManager.CurrentLobby.LobbyCode}";
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

        // Shutdown the network session and load the menu scene
        private void Shutdown()
        {
            //if (UGSNetworkManager.Singleton.IsServer)
            //    await UGSLobbyManager.Singleton.DeleteCurrentLobbyAsync();
            UGSNetworkManager.Singleton.Shutdown();
            UGSLobbyManager.Singleton.RemovePlayerAsync(UGSAuthManager.MyPlayerId);
        }


        #region Events

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
            UIGameplayPanelsController.Singleton.ShowPopup(GameplayPopups.ChangePlayerGameMode);
        }

        private void OnButtonClickedStatsPanelControl()
        {
            UIGameplayPanelsController.Singleton.SwitchPanel(GameplayPanels.PlayerScore);
        }

        private void OnButtonClickedLeave()
        {
            Shutdown();
        }

        #endregion

        #endregion
    }
}