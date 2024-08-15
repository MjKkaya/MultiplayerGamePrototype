using MultiplayerGamePrototype.Events;
using MultiplayerGamePrototype.UI.UIToolkit.Base;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


namespace MultiplayerGamePrototype.UI.UIToolkit.MainScene
{
    [RequireComponent(typeof(UIDocument))]
    public class MainSceneUIController : MonoBehaviour
    {
        private const string _mainScreenName = "main-screen";
        private const string _createLobbyScreenName = "create-lobby-screen";
        private const string _joinLobbyScreenName = "join-lobby-screen";

        [Tooltip("Required UI Document")]
        UIDocument _uiDocument;

        List<UIScreen> _screens = new();

        private UIScreen _mainScreen;
        private UIScreen _createLobbyScreen;
        private UIScreen _joinLobbyScreen;

        UIScreen _currentScreen;
        public UIScreen CurrentScreen => _currentScreen;

        public UIDocument UIDocument => _uiDocument;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Awake()
        {
            UIEvents.ShowCreateLobbyScreen += UIEvents_ShowCreateLobbyScreen;
            UIEvents.ShowJoinLobbyScreen += UIEvents_ShowJoinLobbyScreen;
            _uiDocument = GetComponent<UIDocument>();
        }

        // Register event listeners to game events
        private void OnEnable()
        {
            SubscribeToEvents();
            Initialize();
        }

        // Unregister the listeners to prevent errors
        private void OnDisable()
        {
            UIEvents.ShowCreateLobbyScreen -= UIEvents_ShowCreateLobbyScreen;
            UIEvents.ShowJoinLobbyScreen -= UIEvents_ShowJoinLobbyScreen;
            UnsubscribeFromEvents();
        }

        private void Initialize()
        {
            VisualElement root = _uiDocument.rootVisualElement;

            _mainScreen = new MainScreen(root.Q<VisualElement>(_mainScreenName));
            _createLobbyScreen = new CreateLobbyScreen(root.Q<VisualElement>(_createLobbyScreenName));
            _joinLobbyScreen = new JoinLobbyScreen(root.Q<VisualElement>(_joinLobbyScreenName));

            RegisterScreens();
            HideScreens();
            ShowScreen(_mainScreen);
        }

        private void SubscribeToEvents()
        {
        }

        private void UnsubscribeFromEvents()
        {
        }


        // Store each UIScreen into a master list so we can hide all of them easily.
        private void RegisterScreens()
        {
            _screens = new List<UIScreen>
            {
                _mainScreen,
                _createLobbyScreen,
                _joinLobbyScreen
            };
        }

        // Hide all Views
        private void HideScreens()
        {
            foreach (UIScreen screen in _screens)
            {
                screen.Hide();
            }
        }
        
        public void ShowScreen(UIScreen screen)
        {
            if (screen == null)
                return;

            if (_currentScreen != null)
            {
                if (!screen.IsTransparent)
                    _currentScreen.Hide();
            }

            screen.Show();
            _currentScreen = screen;
            Debug.Log("MainSceneUIController-ShowScreen:"+_currentScreen);
        }

        private void UIEvents_ShowCreateLobbyScreen()
        {
            ShowScreen(_createLobbyScreen);
        }

        private void UIEvents_ShowJoinLobbyScreen()
        {
            Debug.Log("MainSceneUIController-UIEvents_ShowJoinLobbyScreen:"+_joinLobbyScreen);
            ShowScreen(_joinLobbyScreen);
        } 
    }
}