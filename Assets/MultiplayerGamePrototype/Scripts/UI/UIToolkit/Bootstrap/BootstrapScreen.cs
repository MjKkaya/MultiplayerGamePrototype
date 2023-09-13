using MultiplayerGamePrototype.Core;
using MultiplayerGamePrototype.UGS.Managers;
using UnityEngine;
using UnityEngine.UIElements;


[RequireComponent(typeof(UIDocument))]
public class BootstrapScreen : MonoBehaviour
{
    // string IDs
    private const string k_InitializeElement = "menu__initialize__element";
    private const string k_SingInElement = "menu__sing-in__element";
    private const string k_UsernameTextField = "menu__username__text";
    private const string k_SignInButton = "menu__sign-in__button";

    private VisualElement m_Root;
    private TextField m_UsernameTextField;
    private Button m_SignInButton;
    private VisualElement m_InitializeElement;
    private VisualElement m_SingInElement;


    private void Awake()
    {
        Init();
    }

    private void OnDestroy()
    {
        UGSAuthManager.ActionOnCompletedSignIn -= OnCompletedSignIn;
        //if(m_SignInButton != null)
        //    m_SignInButton.RegisterCallback<ClickEvent>(OnClickedSingInButton);
    }

    private void Start()
    {
        Debug.Log("BootstrapScreen-Start");
        Invoke(nameof(ShowSingInPanel), 0.75f);
    }

    private void Init()
    {
        Debug.Log($"BootstrapScreen-Init-Singleton:{Unity.Netcode.NetworkManager.Singleton}");
        UGSAuthManager.ActionOnCompletedSignIn += OnCompletedSignIn;
        m_Root = GetComponent<UIDocument>().rootVisualElement;
        SetVisualElements();
    }


    private void SetVisualElements()
    {
        m_InitializeElement = m_Root.Q<VisualElement>(k_InitializeElement);
        m_InitializeElement.style.display = DisplayStyle.Flex;
        m_SingInElement = m_Root.Q<VisualElement>(k_SingInElement);
        m_SingInElement.style.display = DisplayStyle.None;

        m_UsernameTextField = m_Root.Q<TextField>(k_UsernameTextField);
        m_SignInButton = m_Root.Q<Button>(k_SignInButton);

        m_SignInButton.RegisterCallback<ClickEvent>(OnClickedSingInButton);
        //m_SignInButton.clicked += OnClickedSingInButton;
        Debug.Log("BootstrapScreen-SetVisualElements");
    }

    private void ShowSingInPanel()
    {
        m_UsernameTextField.value = PlayerPrefsManager.Singleton.GetString(PlayerPrefsManager.PLAYER_USERNAME_KEY, string.Empty);
        m_InitializeElement.style.display = DisplayStyle.None;
        m_SingInElement.style.display = DisplayStyle.Flex;
        Debug.Log("BootstrapScreen-ShowSingInPanel");
    }

    private async void SignInAnonymouslyAsync()
    {
        Debug.Log($"BootstrapScreen-SignInAnonymouslyAsync-UsernameTextField:{m_UsernameTextField.text}");

        //m_SignInButton.pickingMode = PickingMode.Ignore;
        //m_SignInButton.SetEnabled(false);

        bool isSucceed = await UGSAuthManager.Singleton.SignInAnonymouslyAsync(m_UsernameTextField.text);
        if (!isSucceed)
        {
            m_SignInButton.pickingMode = PickingMode.Position;
            m_SignInButton.SetEnabled(true);
        }
    }

    #region Events

    private void OnCompletedSignIn()
    {
        Debug.Log("BootstrapScreen-OnCompletedSignIn");
        LoadingSceneManager.Singleton.LoadScene(SceneName.Main, false);
    }


    #region Button Events

    private void OnClickedSingInButton(ClickEvent evt)
    {
        Debug.Log("BootstrapScreen-OnClickedSingInButton");
        SignInAnonymouslyAsync();
    }

    //private void OnClickedSingInButton()
    //{
    //    Debug.Log("BootstrapScreen-OnClickedSingInButton-2");
    //}

    #endregion


    #endregion
}
