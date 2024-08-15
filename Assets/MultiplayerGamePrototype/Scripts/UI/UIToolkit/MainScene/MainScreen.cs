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
            LobbyEvents.OnFailedJoin += LobbyEvents_OnFailedJoin;
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
        }

        public override void Disable()
        {
            base.Disable();
            UnregisterVisualElementsCallbacks();
            LobbyEvents.OnFailedJoin -= LobbyEvents_OnFailedJoin;
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
            _quickJoinButton.SetEnabled(false);
            LobbyEvents.QuickJoin?.Invoke();
        }

        private void LobbyEvents_OnFailedJoin()
        {
            _quickJoinButton.SetEnabled(true);
        }
    }
}