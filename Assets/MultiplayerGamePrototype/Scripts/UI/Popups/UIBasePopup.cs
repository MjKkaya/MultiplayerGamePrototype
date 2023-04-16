using UnityEngine;


namespace MultiplayerGamePrototype.UI.Popups
{
    public abstract class UIBasePopup : MonoBehaviour
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
    }
}