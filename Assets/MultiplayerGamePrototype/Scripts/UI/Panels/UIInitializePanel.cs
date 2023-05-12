using MultiplayerGamePrototype.Core;
using MultiplayerGamePrototype.UGS.Managers;
using MultiplayerGamePrototype.UI.Core;
using MultiplayerGamePrototype.Utilities;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace MultiplayerGamePrototype.UI.Panels
{
    public class UIInitializePanel : UIBasePanel
    {
        [SerializeField] private CanvasGroup m_InfoTextCanvasGroup;
        [SerializeField] private CanvasGroup m_SignInCanvasGroup;
        [SerializeField] private TMP_InputField m_UsernameField;
        [SerializeField] private Button m_SignInButton;


        private void Start()
        {
            Init();
        }


        public override void Init()
        {
            Debug.Log($"UIInitializePanel-Init:{Unity.Netcode.NetworkManager.Singleton}");
            m_UsernameField.text = PlayerPrefsManager.Singleton.GetString(PlayerPrefsManager.PLAYER_USERNAME_KEY, string.Empty);
            m_SignInButton.interactable = true;
            m_InfoTextCanvasGroup.SetActive(false);
            m_SignInCanvasGroup.SetActive(true);
            UGSAuthManager.ActionOnCompletedSignIn += OnCompletedSignIn;
            m_SignInButton.onClick.AddListener(OnButtonClickedSignIn);
        }


        private async void SignInAnonymouslyAsync()
        {
            m_InfoTextCanvasGroup.SetActive(true);
            m_SignInCanvasGroup.SetActive(false);
            m_SignInButton.interactable = false;
            bool isSucceed = await UGSAuthManager.Singleton.SignInAnonymouslyAsync(m_UsernameField.text);
            if(!isSucceed)
            {
                m_SignInButton.interactable = true;
                m_InfoTextCanvasGroup.SetActive(false);
                m_SignInCanvasGroup.SetActive(true);
            }
        }


        #region Events

        private void OnCompletedSignIn()
        {
            Debug.Log("UIInitializePanel-OnCompletedSignIn");
            LoadingSceneManager.Singleton.LoadScene(SceneName.Main, false);
        }

        private void OnDestroy()
        {
            UGSAuthManager.ActionOnCompletedSignIn -= OnCompletedSignIn;
            if (m_SignInButton != null)
                m_SignInButton.onClick.RemoveAllListeners();
        }


        #region Button Events

        private void OnButtonClickedSignIn()
        {
            SignInAnonymouslyAsync();
        }

        #endregion

        #endregion
    }
}
