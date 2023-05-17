using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using MultiplayerGamePrototype.UI.Core;
using MultiplayerGamePrototype.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;


namespace MultiplayerGamePrototype.UI.Panels.Gameplay
{
    public enum GameplayPanels
    {
        Gameplay = 0,
        PlayerScore = 1
    }


    public enum GameplayPopups
    {
        ChangePlayerGameMode = 0,
        Immobilized = 1
    }


    public class UIGameplayPanelsController : UIBaseScenePanelsController
    {
        public static new UIGameplayPanelsController Singleton{ get; protected set; }


        public override void Awake()
        {
            if (Singleton == null)
                Singleton = this;
            else
                Destroy(gameObject);
        }

        public override void Init()
        {
            base.Init();
            ShowPanel(GameplayPanels.Gameplay);
        }


        #region Panels

        public void ShowPanel(GameplayPanels gameplayPanel)
        {
            m_Panels[(int)gameplayPanel].Show();
        }

        public void HidePanel(GameplayPanels gameplayPanel)
        {
            m_Panels[(int)gameplayPanel].Show();
        }

        public void SwitchPanel(GameplayPanels gameplayPanel)
        {
            m_Panels[(int)gameplayPanel].Switch();
        }

        #endregion


        #region Popups

        public void ShowPopup(GameplayPopups gameplayPopups)
        {
            m_Popups[(int)gameplayPopups].Show();
        }

        public void HidePopup(GameplayPopups gameplayPopups)
        {
            m_Popups[(int)gameplayPopups].Show();
        }

        public void SwitchPopup(GameplayPopups gameplayPopups)
        {
            m_Popups[(int)gameplayPopups].Switch();
        }

        #endregion


        StarterAssetsInputs m_StarterAssetsInputs;
        public void SetPlayerInput(StarterAssetsInputs starterAssetsInputs)
        {
            m_StarterAssetsInputs = starterAssetsInputs;
        }
    }
}