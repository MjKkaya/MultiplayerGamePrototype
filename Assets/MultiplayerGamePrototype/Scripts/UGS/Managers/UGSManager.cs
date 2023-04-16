using MultiplayerGamePrototype.Core;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Events;


namespace MultiplayerGamePrototype.UGS.Managers
{
    public class UGSManager : ManagerSingleton<UGSManager>
    {
        public static UnityAction ActionOnCompletedInitialize;
        public static UnityAction ActionOnFailedInitialize;

        [SerializeField] private ManagerBase[] m_UGSSubManagers;


        public override void Init()
        {
            base.Init();
            foreach (ManagerBase subManager in m_UGSSubManagers)
            {
                subManager.Init();
            }
            InitializeUnityServiceAsync();
        }

        private async void InitializeUnityServiceAsync()
        {
            try
            {
                await UnityServices.InitializeAsync();
                Debug.Log($"UGSManager-InitializeUnityServiceAsync-State:{UnityServices.State}");
                if (UnityServices.State == ServicesInitializationState.Initialized)
                    ActionOnCompletedInitialize?.Invoke();
                else
                    ActionOnFailedInitialize?.Invoke();
            }
            catch (System.Exception ex)
            {
                Debug.Log($"UGSManager-InitializeUnityServiceAsync-ex:{ex}");
                ActionOnFailedInitialize?.Invoke();
            }
        }
    }
}