using MultiplayerGamePrototype.Core;
using MultiplayerGamePrototype.UGS.Managers;
using MultiplayerGamePrototype.UI.Core;
using UnityEngine;
using UnityEngine.UI;


namespace MultiplayerGamePrototype.UI.Panels.Lobby
{
    public class LobbyPanel : UIBasePanel
    {
        [SerializeField] private Button m_StartGameButton;


        private void Awake()
        {
            Init();
        }

        public override void Init()
        {
            m_StartGameButton.onClick.AddListener(OnButtonClickedStartGame);
        }

        #region Events

        private void OnDestroy()
        {
            if(m_StartGameButton != null)
                m_StartGameButton.onClick.RemoveAllListeners();
        }

        #region Button Events

        private void OnButtonClickedStartGame()
        {
            m_StartGameButton.interactable = false;
            LoadingSceneManager.Singleton.LoadScene(SceneName.Gameplay);
        }

        #endregion

        #endregion
    }
}