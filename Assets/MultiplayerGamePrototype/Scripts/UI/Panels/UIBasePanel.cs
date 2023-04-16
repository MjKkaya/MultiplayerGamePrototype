using UnityEngine;
using UnityEngine.Events;


namespace MultiplayerGamePrototype.UI.Panels
{
    public abstract class UIBasePanel : MonoBehaviour
    {
        public abstract void Init();


        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Show(UnityAction callback)
        {
            gameObject.SetActive(true);
        }
    }
}