using MultiplayerGamePrototype.UI.Managers;
using MultiplayerGamePrototype.UGS.Managers;
using MultiplayerGamePrototype.UGS.LobbyController;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace MultiplayerGamePrototype.UI.Panels
{
    public class UIGameModePanel : UIBasePanel
    {
        [Header("Game Bullet Mode")]
        [SerializeField] private TextMeshProUGUI m_GameBulletModeText;
        [SerializeField] private Button m_ChangeGameBulletModeButton;

        [Header("My Bullet Mode")]
        [SerializeField] private TextMeshProUGUI m_MyBulletModeText;
        [SerializeField] private Button m_ChangeMyBulletModeButton;


        public override void Init()
        {
            UGSLobbyManager.ActionOnJoinedLobby += OnJoinedLobby;
            UGSLobbyManager.ActionOnChangedLobbyData += OnChangedLobbyData;
            UGSLobbyManager.ActionOnChangedMyPlayerData += OnChangedMyPlayerData;
            m_ChangeGameBulletModeButton.onClick.AddListener(OnButtonClickedChangeGameBulletMode);
            m_ChangeMyBulletModeButton.onClick.AddListener(OnButtonClickedChangeMyBulletMode);
        }

        public override void Show()
        {
            SetGameMode();
            SetMyBulletMode();
            base.Show();
        }


        private void SetGameMode()
        {
            m_GameBulletModeText.text = UGSLobbyDataController.GetLobbyBulletMode();
        } 
        
        public void SetMyBulletMode()
        {
            m_MyBulletModeText.text = UGSLobbyDataController.GetPlayerBulletMode(UGSAuthManager.MyPlayerId);
        }


        private async void ChangeLobbyBulletMode()
        {
            m_ChangeGameBulletModeButton.interactable = false;
            await UGSLobbyManager.Instance.UpdateLobbyDataAsync(UGSLobbyDataController.CreateRandomLobbyBulletMode());
            m_ChangeGameBulletModeButton.interactable = true;
        }

        #region Events

        private void OnJoinedLobby()
        {
            Show();
        }

        private void OnChangedLobbyData()
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
            UGSLobbyManager.ActionOnChangedLobbyData -= OnChangedLobbyData;
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
            PopupsManager.Instance.ShowPopup(PopupTypes.PlayerGameModeChange);
        }

        #endregion

        #endregion
    }
}