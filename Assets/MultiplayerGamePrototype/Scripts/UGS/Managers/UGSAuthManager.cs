using MultiplayerGamePrototype.Core;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Events;


namespace MultiplayerGamePrototype.UGS.Managers
{
    public class UGSAuthManager : ManagerSingleton<UGSAuthManager>
    {
        public static UnityAction ActionOnCompletedSignIn;
        public static string MyPlayerId;


        public override void Init()
        {
            base.Init();
            Debug.Log("UGSAuthManager-Init");
            UGSManager.ActionOnCompletedInitialize += OnCompletedInitialize;
        }

        public async void SignInAnonymouslyAsync()
        {
            Debug.Log("UGSAuthManager-SignInAnonymouslyAsync");
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            catch (AuthenticationException ex)
            {
               
                Debug.Log($"UGSAuthManager-SignInAnonymouslyAsync-ex:{ex}");
            }
            catch (RequestFailedException exception)
            {
                Debug.Log($"UGSAuthManager-SignInAnonymouslyAsync-exception:{exception}");
            }
        }


        private void SetupEvents(bool isActive)
        {
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

        private void OnCompletedInitialize()
        {
            SetupEvents(true);
            SignInAnonymouslyAsync();
        }

        private void OnDestroy()
        {
            UGSManager.ActionOnCompletedInitialize -= OnCompletedInitialize;
            SetupEvents(false);
        }


        #region AuthenticationService Events

        private void OnSignedIn()
        {
            MyPlayerId = AuthenticationService.Instance.PlayerId;
            Debug.Log($"UGSAuthManager-OnSignedIn-MyPlayerId:{MyPlayerId}");
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