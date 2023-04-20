using MultiplayerGamePrototype.UGS.DataControllers;
using MultiplayerGamePrototype.UGS.Managers;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace MultiplayerGamePrototype.UI.Popups
{
    public class UIChangePlayerGameModePopup : UIBasePopup
    {
        [SerializeField] private TMP_Dropdown m_BulletColorDropdown;
        [SerializeField] private TMP_Dropdown m_BulletSizeDropdown;
        [SerializeField] private Button m_CloseButton;
        [SerializeField] private Button m_ChangeButton;


        public override void Init()
        {
            m_CloseButton.onClick.AddListener(OnButtonClickedClose);
            m_ChangeButton.onClick.AddListener(OnButtonClickedChnage);
            SetDropdowns();
        }

        public override void Show()
        {
            m_BulletColorDropdown.value = (int)UGSPlayerDataController.GetMyPlayerBulletColorType();
            m_BulletSizeDropdown.value = (int)UGSPlayerDataController.GetMyPlayerBulletSizeType();
            base.Show();
        }

        private void SetDropdowns()
        {
            m_BulletColorDropdown.ClearOptions();
            string[] enumTypes = Enum.GetNames(typeof(BulletColorTypes));
            List<string> list = new List<string>(enumTypes);
            list.RemoveAt(list.Count - 1);
            m_BulletColorDropdown.AddOptions(list);

            m_BulletSizeDropdown.ClearOptions();
            enumTypes = Enum.GetNames(typeof(BulletSizeTypes));
            list = new List<string>(enumTypes);
            list.RemoveAt(list.Count - 1);
            m_BulletSizeDropdown.AddOptions(list);
        }

        private async void UpdateMyBulletModeData()
        {
            m_ChangeButton.interactable = false;
            string bulletColor = ((BulletColorTypes)m_BulletColorDropdown.value).ToString();
            string bulletSize = ((BulletSizeTypes)m_BulletSizeDropdown.value).ToString();
            Debug.Log($"UIChangePlayerGameModePopup-UpdateMyBulletModeData:{bulletColor}-{bulletSize}");

            bool isSucceed = await UGSLobbyManager.Singleton.UpdateMyPlayerDataAsync(UGSPlayerDataController.CreateNewPlayerBulletData(bulletColor, bulletSize));
            m_ChangeButton.interactable = true;
            if(isSucceed)
                Hide();
        }

        #region Button Events

        private void OnButtonClickedClose()
        {
            Hide();
        }

        private void OnButtonClickedChnage()
        {
            UpdateMyBulletModeData();
        }

        private void OnDestroy()
        {
            if (m_CloseButton != null)
                m_CloseButton.onClick.RemoveAllListeners();
            if (m_ChangeButton != null)
                m_ChangeButton.onClick.RemoveAllListeners();
        }

        #endregion
    }
}