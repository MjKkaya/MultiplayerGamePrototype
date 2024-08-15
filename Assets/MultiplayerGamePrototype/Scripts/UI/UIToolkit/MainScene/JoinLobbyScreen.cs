using MultiplayerGamePrototype.Events;
using MultiplayerGamePrototype.UI.UIToolkit.Base;
using UnityEngine.UIElements;


namespace MultiplayerGamePrototype.UI.UIToolkit.MainScene
{
    public class JoinLobbyScreen : UIScreen
    {
        private const string _lobbyNameTextFieldName = "custom-textfield";
        private const string _joinButtonName = "join-button";
        private const string _closeButtonName = "close-button";
        
        private TextField _lobbyNameTextfield;
        private Button _joinButton;
        private Button _closeButton;


        public JoinLobbyScreen(VisualElement rootElement) : base(rootElement)
        {
            _isTransparent = true;
            LobbyEvents.OnFailedJoin += LobbyEvents_OnFailedJoin;
            SetVisualElements();
            RegisterVisualElementsCallbacks();
        }


        protected override void SetVisualElements()
        {
            _lobbyNameTextfield = _rootElement.Q<TextField>(_lobbyNameTextFieldName);
            _closeButton = _rootElement.Q<Button>(_closeButtonName);
            _joinButton = _rootElement.Q<Button>(_joinButtonName);
        }

        protected override void RegisterVisualElementsCallbacks()
        {
            _closeButton.RegisterCallback<ClickEvent>(OnClickedCloseButton);
            _joinButton.RegisterCallback<ClickEvent>(OnClickedJoinButton); 
        }

        protected override void UnregisterVisualElementsCallbacks()
        {
            _closeButton.UnregisterCallback<ClickEvent>(OnClickedCloseButton);
            _joinButton.UnregisterCallback<ClickEvent>(OnClickedJoinButton); 
        }
        

        public override void Disable()
        {
            base.Disable();
            LobbyEvents.OnFailedJoin -= LobbyEvents_OnFailedJoin;
        }


        private void SetActiveButtons(bool isEnabled)
        {
            _closeButton.SetEnabled(isEnabled);
            _joinButton.SetEnabled(isEnabled);
        }

        public override void Show()
        {
            base.Show();
            SetActiveButtons(true);
        }


        #region  Events
        private void OnClickedCloseButton(ClickEvent evt)
        {
            Hide();
        }

        private void OnClickedJoinButton(ClickEvent evt)
        {
            SetActiveButtons(false);
            string lobbyName = _lobbyNameTextfield.value;
            LobbyEvents.Join?.Invoke(lobbyName);
        }

        private void LobbyEvents_OnFailedJoin()
        {
            SetActiveButtons(true);
        }

        

        #endregion
    }
}