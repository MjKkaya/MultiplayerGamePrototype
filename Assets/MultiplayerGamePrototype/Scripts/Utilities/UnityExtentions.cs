using UnityEngine;


namespace MultiplayerGamePrototype.Utilities
{
    public static class UnityExtentions
    {
        #region Canvas Group

        public static void SetActive(this CanvasGroup canvasGroup, bool isActive)
        {
            canvasGroup.alpha = isActive ? 1 : 0;
            canvasGroup.blocksRaycasts = isActive;
            canvasGroup.interactable = isActive;
        }

        #endregion
    }
}