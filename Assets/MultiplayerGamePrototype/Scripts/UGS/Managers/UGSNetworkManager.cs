using MultiplayerGamePrototype.Utilities;
using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;


namespace MultiplayerGamePrototype.UGS.Managers
{
    [RequireComponent(typeof(NetworkManager))]
    public class UGSNetworkManager : SingletonMonoPersistent<UGSNetworkManager>
    {
        //todo: is this class neccesary?
        public static Action ActionOnInitilized;
        public static Action ActionOnSceneManagerInitilized;

        private NetworkManager m_NetworkManager;


        public override void Awake()
        {
            base.Awake();
            m_NetworkManager = GetComponent<NetworkManager>();
            StartCoroutine(CheckSingletonInitialization());
        }

        IEnumerator CheckSingletonInitialization()
        {
            do
            {
                yield return null;
                Debug.Log("UGSNetworkManager-CheckSingletonInitialization-while");
            } while (NetworkManager.Singleton == null);

            Debug.Log("UGSNetworkManager-CheckSingletonInitialization-END");
            ActionOnInitilized?.Invoke();

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        }

        IEnumerator CheckSceneManagerInitialization()
        {
            Debug.Log("UGSNetworkManager-CheckSceneManagerInitialization");
            do
            {
                yield return null;
                Debug.Log("UGSNetworkManager-CheckSceneManagerInitialization-while");
            } while (NetworkManager.Singleton.SceneManager == null);

            ActionOnSceneManagerInitilized?.Invoke();
            Debug.Log("UGSNetworkManager-CheckSceneManagerInitialization-END!");
        }


        #region Events

        private void OnClientConnectedCallback(ulong connectedClientId)
        {
            Debug.Log($"UGSNetworkManager-OnClientConnectedCallback-connectedClientId:{connectedClientId}");
            StartCoroutine(CheckSceneManagerInitialization());
        }

        private void OnDestroy()
        {
            if(NetworkManager.Singleton != null)
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
        }

        #endregion
    }
}