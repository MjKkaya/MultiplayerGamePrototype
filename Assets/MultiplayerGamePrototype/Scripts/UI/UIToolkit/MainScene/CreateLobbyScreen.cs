using MultiplayerGamePrototype.Events;
using MultiplayerGamePrototype.UI.UIToolkit.Base;
using UnityEngine.UIElements;


namespace MultiplayerGamePrototype.UI.UIToolkit.MainScene
{
    public class CreateLobbyScreen : UIScreen
    {
        // String - IDs
        private const string _lobbyNameTextFieldName = "custom-textfield";
        private const string _createButtonName = "create-button";
        private const string _closeButtonName = "close-button";
        
        private TextField _lobbyNameTextfield;
        private Button _createButton;
        private Button _closeButton;


        public CreateLobbyScreen(VisualElement rootElement) : base(rootElement)
        {
            _isTransparent = true;
            LobbyEvents.OnFailedCreation += LobbyEvents_OnFailedCreation;
            SetVisualElements();
            RegisterVisualElementsCallbacks();
        }

       

        protected override void SetVisualElements()
        {
            _lobbyNameTextfield = _rootElement.Q<TextField>(_lobbyNameTextFieldName);
            _createButton = _rootElement.Q<Button>(_createButtonName);
            _closeButton = _rootElement.Q<Button>(_closeButtonName);
        }

        protected override void RegisterVisualElementsCallbacks()
        {
            _createButton.RegisterCallback<ClickEvent>(OnClickedCreateButton);
            _closeButton.RegisterCallback<ClickEvent>(OnClickedCloseButton);
        }

        protected override void UnregisterVisualElementsCallbacks()
        {
            _createButton.UnregisterCallback<ClickEvent>(OnClickedCreateButton);
            _closeButton.UnregisterCallback<ClickEvent>(OnClickedCloseButton); 
        }

        public override void Disable()
        {
            base.Disable();
            LobbyEvents.OnFailedCreation -= LobbyEvents_OnFailedCreation;
        }

       

        private void SetActiveButtons(bool isEnabled)
        {
            _createButton.SetEnabled(isEnabled);
            _closeButton.SetEnabled(isEnabled);
        }


        private void OnClickedCreateButton(ClickEvent evt)
        {
            SetActiveButtons(false);
            string lobbyName = _lobbyNameTextfield.value;
            LobbyEvents.Create?.Invoke(lobbyName);
        }

        private void OnClickedCloseButton(ClickEvent evt)
        {
            Hide();
        }

         private void LobbyEvents_OnFailedCreation()
        {
            SetActiveButtons(true);
        }
    }
}