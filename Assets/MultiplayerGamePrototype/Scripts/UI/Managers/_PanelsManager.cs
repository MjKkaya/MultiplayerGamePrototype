using MultiplayerGamePrototype.Core;
using MultiplayerGamePrototype.UI.Panels;
using UnityEngine;


namespace MultiplayerGamePrototype.UI.Managers
{
    public enum PanelTypes
    {
        MainMenu = 0,
        GameMode = 1,
        GamePlayerScore = 2
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


        public void ShowPanel(PanelTypes panelTypes)
        {
            m_Panels[(int)panelTypes].Show();
        }

        public void HidePanel(PanelTypes panelTypes)
        {
            m_Panels[(int)panelTypes].Hide();
        }

    }
}