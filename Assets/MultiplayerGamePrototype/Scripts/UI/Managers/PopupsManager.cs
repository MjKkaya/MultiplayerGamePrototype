using MultiplayerGamePrototype.Core;
using MultiplayerGamePrototype.UI.Popups;
using UnityEngine;


namespace MultiplayerGamePrototype.UI.Managers
{
    public enum PopupTypes
    {
        PlayerGameModeChange = 0
    }


    public class PopupsManager : ManagerSingleton<PopupsManager>
    {
        [Tooltip("Array ordering and PanelType ordering must be the same.")]
        [SerializeField] private UIBasePopup[] m_Popups;


        /// <summary>
        ///All popups preparing.
        /// </summary>
        public override void Init()
        {
            base.Init();
            foreach (UIBasePopup subPopup in m_Popups)
            {
                subPopup.Init();
                subPopup.Hide();
            }
        }

        public void ShowPopup(PopupTypes popupType)
        {
            m_Popups[(int)popupType].Show();
        }
    }
}