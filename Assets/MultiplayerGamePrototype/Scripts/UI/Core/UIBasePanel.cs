using MultiplayerGamePrototype.Utilities;
using UnityEngine;
using UnityEngine.Events;


namespace MultiplayerGamePrototype.UI.Core
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIBasePanel : MonoBehaviour
    {
        private CanvasGroup m_CanvasGroup;
        private bool m_IsOpen = false;


        public virtual void Init()
        {
            m_CanvasGroup = GetComponent<CanvasGroup>();
        }

        public virtual void Hide()
        {
            m_CanvasGroup.SetActive(false);
            m_IsOpen = false;
        }

        public virtual void Show()
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
            m_CanvasGroup.SetActive(true);
            m_IsOpen = true;
        }

        public virtual void Show(UnityAction callback)
        {
            gameObject.SetActive(true);
        }

        public void Switch()
        {
            if (m_IsOpen)
                Hide();
            else
                Show();
        }
    }
}