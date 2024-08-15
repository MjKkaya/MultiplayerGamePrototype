using MultiplayerGamePrototype.Core;
using MultiplayerGamePrototype.Events;
using UnityEngine;
using UnityEngine.UIElements;


[RequireComponent(typeof(UIDocument))]
public class BootstrapScreen : MonoBehaviour
{
    // string IDs
    private const string _initializeElementName = "menu__initialize__element";
    private const string _singInElementName = "menu__sing-in__element";
    private const string _usernameTextFieldName = "custom-textfield";
    private const string _signInButtonName = "menu__sign-in__button";

    private VisualElement _rootElement;
    private TextField _usernameTextField;
    private Button _signInButton;
    private VisualElement _initializeElement;
    private VisualElement _singInElement;


    private void Awake()
    {
        Initialize();
    }

    private void OnDestroy()
    {
         AuthenticaitonEvents.OnCompletedSignedIn -= AuthenticaitonEvents_OnComplatedSignedIn;
        AuthenticaitonEvents.OnFailedSignedIn -= AuthenticaitonEvents_OnFailedSignedIn;
        if(_signInButton != null)
           _signInButton.RegisterCallback<ClickEvent>(OnClickedSingInButton);
    }

    private void Start()
    {
        Invoke(nameof(ShowSingInPanel), 0.75f);
    }

    private void Initialize()
    {
        AuthenticaitonEvents.OnCompletedSignedIn += AuthenticaitonEvents_OnComplatedSignedIn;
        AuthenticaitonEvents.OnFailedSignedIn += AuthenticaitonEvents_OnFailedSignedIn;
        _rootElement = GetComponent<UIDocument>().rootVisualElement;
        SetVisualElements();
    }


    private void SetVisualElements()
    {
        _initializeElement = _rootElement.Q<VisualElement>(_initializeElementName);
        _initializeElement.style.display = DisplayStyle.Flex;
        _singInElement = _rootElement.Q<VisualElement>(_singInElementName);
        _singInElement.style.display = DisplayStyle.None;
        _usernameTextField = _rootElement.Q<TextField>(_usernameTextFieldName);
        _signInButton = _rootElement.Q<Button>(_signInButtonName);
        _signInButton.RegisterCallback<ClickEvent>(OnClickedSingInButton);
    }

    private void ShowSingInPanel()
    {
        _usernameTextField.value = PlayerPrefsManager.Singleton.GetString(PlayerPrefsManager.PLAYER_USERNAME_KEY, string.Empty);
        _initializeElement.style.display = DisplayStyle.None;
        _singInElement.style.display = DisplayStyle.Flex;
    }

    private void SignInAnonymously()
    {
        string username = _usernameTextField.text;
        Debug.Log($"BootstrapScreen-SignInAnonymouslyAsync-username:{username}");
        _signInButton.pickingMode = PickingMode.Ignore;
        _signInButton.SetEnabled(false);
        AuthenticaitonEvents.SignInAnonymously?.Invoke(username);
    }

    #region Events

    private void AuthenticaitonEvents_OnComplatedSignedIn()
    {
        SceneLoadingManager.Singleton.LoadScene(SceneName.Main, false);
    }

    private void AuthenticaitonEvents_OnFailedSignedIn()
    {
        _signInButton.pickingMode = PickingMode.Position;
        _signInButton.SetEnabled(true);
    }

    private void OnClickedSingInButton(ClickEvent evt)
    {
        SignInAnonymously();
    }

    #endregion
}