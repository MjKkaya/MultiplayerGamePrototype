using MultiplayerGamePrototype.Core;
using MultiplayerGamePrototype.Events;
using MultiplayerGamePrototype.Utilities;
using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;


namespace MultiplayerGamePrototype.UGS.Managers
{
    public class UGSAuthManager : SingletonMonoPersistent<UGSAuthManager>
    {
        private static string m_MyPlayerId;
        public static string MyPlayerId => m_MyPlayerId;

        private static string m_MyUsername;
        public static string MyUsername => m_MyUsername;


        public override void Awake()
        {
            base.Awake();
            AuthenticaitonEvents.SignInAnonymously += AuthenticaitonEvents_SignInAnonymously;
        }


        private async void SignInAnonymouslyAsync(string username)
        {
            m_MyUsername = username;
            bool isInitialized = await InitializeUnityServiceAsync(username);

            Debug.Log($"UGSAuthManager-SignInAnonymouslyAsync-isInitialized:{isInitialized}");
            if (!isInitialized)
                AuthenticaitonEvents.OnFailedSignedIn?.Invoke();

            try
            {
                Debug.Log($"UGSAuthManager-SignInAnonymouslyAsync-IsSignedIn:{AuthenticationService.Instance.IsSignedIn}");
                if(AuthenticationService.Instance.IsSignedIn)
                {
                    OnSignedIn();
                }

                SubscribeToUGSEvents();
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            catch (AuthenticationException ex)
            {
                Debug.Log($"UGSAuthManager-SignInAnonymouslyAsync-ex:{ex}");
                AuthenticaitonEvents.OnFailedSignedIn?.Invoke();
            }
            catch (RequestFailedException exception)
            {
                Debug.Log($"UGSAuthManager-SignInAnonymouslyAsync-exception:{exception}");
                AuthenticaitonEvents.OnFailedSignedIn?.Invoke();
            }
        }

        private async Task<bool> InitializeUnityServiceAsync(string profileName)
        {
            try
            {
                Debug.Log($"UGSManager-InitializeUnityServiceAsync-State:{UnityServices.State}, profileName:{profileName}");
                if (UnityServices.State == ServicesInitializationState.Initialized)
                    return true;

                if(profileName == null)
                {
                    await UnityServices.InitializeAsync();
                }
                else
                {
                    InitializationOptions initializationOptions = new();
                    initializationOptions.SetProfile(profileName);
                    await UnityServices.InitializeAsync(initializationOptions);
                }

                Debug.Log($"UGSManager-InitializeUnityServiceAsync-State:{UnityServices.State}");
                if (UnityServices.State == ServicesInitializationState.Initialized)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                Debug.Log($"UGSManager-InitializeUnityServiceAsync-ex:{ex}");
                return false;
            }
        }
        
        private void SubscribeToUGSEvents()
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
                return;
            AuthenticationService.Instance.SignedIn += OnSignedIn;
            AuthenticationService.Instance.SignInFailed += OnSignInFailed;
        }

        private void UnsubscribeFromUGSEvents()
        {
            
            if (UnityServices.State != ServicesInitializationState.Initialized)
                return;
            AuthenticationService.Instance.SignedIn -= OnSignedIn;
            AuthenticationService.Instance.SignInFailed -= OnSignInFailed;
        }


        #region Events

        private void OnDestroy()
        {
            AuthenticaitonEvents.SignInAnonymously -= AuthenticaitonEvents_SignInAnonymously;
            UnsubscribeFromUGSEvents();
        }

        private void AuthenticaitonEvents_SignInAnonymously(string username)
        {
            SignInAnonymouslyAsync(username);
        }

        #region AuthenticationService Events

        private void OnSignedIn()
        {
            m_MyPlayerId = AuthenticationService.Instance.PlayerId;
            Debug.Log($"UGSAuthManager-OnSignedIn-MyPlayerId:{MyPlayerId}");
            PlayerPrefsManager.Singleton.SetString(PlayerPrefsManager.PLAYER_USERNAME_KEY, m_MyUsername);
            AuthenticaitonEvents.OnCompletedSignedIn?.Invoke();
        }

        private void OnSignInFailed(RequestFailedException requestFailedException)
        {
            Debug.Log($"UGSAuthManager-OnSignInFailed:{requestFailedException}");
            AuthenticaitonEvents.OnFailedSignedIn?.Invoke();
        }

        #endregion

        #endregion
    }
}