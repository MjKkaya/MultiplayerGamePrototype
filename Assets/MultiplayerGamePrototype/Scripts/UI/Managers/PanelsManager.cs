using MultiplayerGamePrototype.Core;
using MultiplayerGamePrototype.UI.Panels;
using UnityEngine;


namespace MultiplayerGamePrototype.UI.Managers
{
    public enum PanelTypes
    {
        Initialize = 0,
        MainMenu = 1,
        GameMode = 2
    }


    public class PanelsManager : ManagerSingleton<PanelsManager>
    {
        [Tooltip("Array ordering and PanelType ordering must be the same.")]
        [SerializeField] private UIBasePanel[] m_Panels;


        /// <summary>
        ///All panels preparing.
        /// </summary>
        public override void Init()
        {
            base.Init();
            foreach (UIBasePanel subPanel in m_Panels)
            {
                subPanel.Init();
                subPanel.Hide();
            }
        }


        /// <summary>
        /// First beginning panel will show when app launched.
        /// </summary>
        private void Start()
        {
            m_Panels[(int)PanelTypes.Initialize].Show();
        }
    }
}