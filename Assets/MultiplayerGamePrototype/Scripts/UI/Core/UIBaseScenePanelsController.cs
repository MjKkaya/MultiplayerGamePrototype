using MultiplayerGamePrototype.UI.Panels;
using MultiplayerGamePrototype.Utilities;
using UnityEngine;


namespace MultiplayerGamePrototype.UI.Core
{
    public abstract class UIBaseScenePanelsController : SingletonMono<UIBaseScenePanelsController>
    {
        [Tooltip("Array ordering and PanelType ordering must be the same.")]
        [SerializeField] protected UIBasePanel[] m_Panels;
        [SerializeField] protected UIBasePanel[] m_Popups;


        /// <summary>
        ///All panels preparing.
        /// </summary>
        public virtual void Init()
        {
            foreach (UIBasePanel subPanel in m_Panels)
            {
                subPanel.Init();
                subPanel.Hide();
            }

            foreach (UIBasePanel subPopup in m_Popups)
            {
                subPopup.Init();
                subPopup.Hide();
            }
        }
    }
}