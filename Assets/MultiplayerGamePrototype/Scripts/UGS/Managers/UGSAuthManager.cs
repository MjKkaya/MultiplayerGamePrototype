using MultiplayerGamePrototype.Core;
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
        public static Action ActionOnCompletedSignIn;

        private static string m_MyPlayerId;
        public static string MyPlayerId => m_MyPlayerId;

        private static string m_MyUsername;
        public static string MyUsername => m_MyUsername;


        public async Task<bool> SignInAnonymouslyAsync(string username)
        {
            m_MyUsername = username;
            bool isInitialized = await InitializeUnityServiceAsync(username);

            Debug.Log($"UGSAuthManager-SignInAnonymouslyAsync-isInitialized:{isInitialized}");
            if (!isInitialized)
                return false;

            try
            {
                Debug.Log($"UGSAuthManager-SignInAnonymouslyAsync-IsSignedIn:{AuthenticationService.Instance.IsSignedIn}");
                if(AuthenticationService.Instance.IsSignedIn)
                {
                    OnSignedIn();
                    return true;
                }

                SetupEvents(true);
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                return true;
            }
            catch (AuthenticationException ex)
            {
                Debug.Log($"UGSAuthManager-SignInAnonymouslyAsync-ex:{ex}");
                return false;
            }
            catch (RequestFailedException exception)
            {
                Debug.Log($"UGSAuthManager-SignInAnonymouslyAsync-exception:{exception}");
                return false;
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
                    //This restrict is doing at username's input field!
                    //Regex rgx = new Regex("[^a-zA-Z0-9 - _]");
                    //profileName = rgx.Replace(profileName, "");
                    initializationOptions.SetProfile(profileName);
                    await UnityServices.InitializeAsync(initializationOptions);
                }

                Debug.Log($"UGSManager-InitializeUnityServiceAsync-State:{UnityServices.State}");
                if (UnityServices.State == ServicesInitializationState.Initialized)
                    return true;
                else
                    return false;
            }
            catch (System.Exception ex)
            {
                Debug.Log($"UGSManager-InitializeUnityServiceAsync-ex:{ex}");
                return false;
            }
        }
        
        private void SetupEvents(bool isActive)
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
                return;

            if (isActive)
            {
                AuthenticationService.Instance.SignedIn += OnSignedIn;
                AuthenticationService.Instance.SignInFailed += OnSignInFailed;
            }
            else
            {
                AuthenticationService.Instance.SignedIn -= OnSignedIn;
                AuthenticationService.Instance.SignInFailed -= OnSignInFailed;
            }
        }


        #region Events

        private void OnDestroy()
        {
            SetupEvents(false);
        }


        #region AuthenticationService Events

        private void OnSignedIn()
        {
            m_MyPlayerId = AuthenticationService.Instance.PlayerId;
            Debug.Log($"UGSAuthManager-OnSignedIn-MyPlayerId:{MyPlayerId}");
            PlayerPrefsManager.Singleton.SetString(PlayerPrefsManager.PLAYER_USERNAME_KEY, m_MyUsername);
            ActionOnCompletedSignIn?.Invoke();
        }

        private void OnSignInFailed(RequestFailedException requestFailedException)
        {
            Debug.Log($"UGSAuthManager-OnSignInFailed:{requestFailedException}");
        }

        #endregion

        #endregion
    }
}