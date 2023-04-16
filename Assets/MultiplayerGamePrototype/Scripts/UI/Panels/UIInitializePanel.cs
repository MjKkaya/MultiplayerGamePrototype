using MultiplayerGamePrototype.UGS.Managers;


namespace MultiplayerGamePrototype.UI.Panels
{
    public class UIInitializePanel : UIBasePanel
    {
        public override void Init()
        {
            UGSAuthManager.ActionOnCompletedSignIn += OnCompletedSignIn;
        }


        #region Events

        private void OnCompletedSignIn()
        {
            Hide();
        }

        private void OnDestroy()
        {
            UGSAuthManager.ActionOnCompletedSignIn -= OnCompletedSignIn;
        }

        #endregion
    }
}
