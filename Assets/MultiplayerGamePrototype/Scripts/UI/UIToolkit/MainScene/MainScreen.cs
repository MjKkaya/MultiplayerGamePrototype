using MultiplayerGamePrototype.Events;
using MultiplayerGamePrototype.UI.UIToolkit.Base;
using UnityEngine.UIElements;


namespace MultiplayerGamePrototype.UI.UIToolkit.MainScene
{
    public class MainScreen : UIScreen
    {
        private const string _createButtonName = "create-button";
        private const string _joinButtonName = "join-button";
        private const string _quickJoinButtonName = "quick-join-button";


        private Button _createButton;
        private Button _joinButton;
        private Button _quickJoinButton;


        public MainScreen(VisualElement rootElement) : base(rootElement)
        {
            LobbyEvents.OnFailedCreation += LobbyEvents_OnFailedCreation;
            SetVisualElements();
            RegisterVisualElementsCallbacks();
        }


        protected override void SetVisualElements()
        {
            _createButton = _rootElement.Q<Button>(_createButtonName);
            _joinButton = _rootElement.Q<Button>(_joinButtonName);
            _quickJoinButton = _rootElement.Q<Button>(_quickJoinButtonName);
        }

        protected override void RegisterVisualElementsCallbacks()
        {
            _createButton.RegisterCallback<ClickEvent>(OnClickedCreateButton);
            _joinButton.RegisterCallback<ClickEvent>(OnClickedJoinButton);
            _quickJoinButton.RegisterCallback<ClickEvent>(OnClickedQuickJoinButton);
        }

        protected override void UnregisterVisualElementsCallbacks()
        {
             _createButton.UnregisterCallback<ClickEvent>(OnClickedCreateButton);
            _joinButton.UnregisterCallback<ClickEvent>(OnClickedJoinButton);
            _quickJoinButton.UnregisterCallback<ClickEvent>(OnClickedQuickJoinButton);
        }

        public override void Disable()
        {
            base.Disable();
            UnregisterVisualElementsCallbacks();
            LobbyEvents.OnFailedCreation -= LobbyEvents_OnFailedCreation;
        }

        private void SetEnabledAllButton(bool isEnable)
        {
            _createButton.SetEnabled(false);
            _joinButton.SetEnabled(false);
            _quickJoinButton.SetEnabled(false);
        }


        private void OnClickedCreateButton(ClickEvent evt)
        {
            UIEvents.ShowCreateLobbyScreen?.Invoke();
        }

        private void OnClickedJoinButton(ClickEvent evt)
        {
            UIEvents.ShowJoinLobbyScreen?.Invoke();
        }

        private void OnClickedQuickJoinButton(ClickEvent evt)
        {
            SetEnabledAllButton(false);
            LobbyEvents.QuickJoin?.Invoke();
        }

        private void LobbyEvents_OnFailedCreation()
        {
            SetEnabledAllButton(true);
        }
    }
}