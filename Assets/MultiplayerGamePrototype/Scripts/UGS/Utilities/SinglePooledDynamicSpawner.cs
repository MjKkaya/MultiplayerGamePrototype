using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;


namespace MultiplayerGamePrototype.UGS.Utilities
{
    public class SinglePooledDynamicSpawner : NetworkBehaviour, INetworkPrefabInstanceHandler
    {
        [SerializeField]
        private GameObject PrefabToSpawn;

        private GameObject m_PrefabInstance;
        private NetworkObject m_SpawnedNetworkObject;


        private void Start()
        {
            Debug.Log("SinglePooledDynamicSpawner-Start");
            // Instantiate our instance when we start (for both clients and server)
            m_PrefabInstance = Instantiate(PrefabToSpawn);

            // Get the NetworkObject component assigned to the Prefab instance
            m_SpawnedNetworkObject = m_PrefabInstance.GetComponent<NetworkObject>();

            // Set it to be inactive
            m_PrefabInstance.SetActive(false);
        }


        /// <summary>
        /// Just Server/Host runs the method.
        /// </summary>
        /// <param name="spawnPosition">The spawnPosition to spawn the object at.</param>
        /// <returns></returns>
        public NetworkObject SpawnInstance(Vector3 spawnPosition)
        {
            Debug.Log($"SinglePooledDynamicSpawner-SpawnInstance-IsServer:{IsServer}");
            if (IsServer && m_PrefabInstance != null && m_SpawnedNetworkObject != null && !m_SpawnedNetworkObject.IsSpawned)
            {
                m_PrefabInstance.SetActive(true);
                m_PrefabInstance.transform.position = spawnPosition;
                m_SpawnedNetworkObject.Spawn();
                return m_SpawnedNetworkObject;
            }
            else
                return null;
        }


        #region Implementations

        /// <summary>
        /// Invoked only on clients and not server or host
        /// INetworkPrefabInstanceHandler.Instantiate implementation
        /// Called when Netcode for GameObjects need an instance to be spawned
        /// </summary>
        NetworkObject INetworkPrefabInstanceHandler.Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation)
        {
            Debug.Log($"SinglePooledDynamicSpawner-Instantiate-ownerClientId:{ownerClientId}, position:{position}, rotation:{rotation}");
            m_PrefabInstance.SetActive(true);
            m_PrefabInstance.transform.position = position;
            m_PrefabInstance.transform.rotation = rotation;
            return m_SpawnedNetworkObject;
        }

        /// <summary>
        /// Client and Server side
        /// This method is triggered when run "m_SpawnedNetworkObject.Despawn();"
        /// INetworkPrefabInstanceHandler.Destroy implementation
        /// </summary>
        void INetworkPrefabInstanceHandler.Destroy(NetworkObject networkObject)
        {
            Debug.Log("SinglePooledDynamicSpawner-Destroy");
            if(m_PrefabInstance != null)
                m_PrefabInstance.SetActive(false);
        }

        #endregion


        public override void OnNetworkSpawn()
        {
            Debug.Log("SinglePooledDynamicSpawner-OnNetworkSpawn");
            // We register our network Prefab and this NetworkBehaviour that implements the
            // INetworkPrefabInstanceHandler interface with the Prefab handler
            NetworkManager.PrefabHandler.AddHandler(PrefabToSpawn, this);
        }

        public override void OnNetworkDespawn()
        {
            Debug.Log("SinglePooledDynamicSpawner-OnNetworkDespawn");
            if (m_SpawnedNetworkObject != null && m_SpawnedNetworkObject.IsSpawned)
            {
                m_SpawnedNetworkObject.Despawn();
            }
            base.OnNetworkDespawn();
        }

        public override void OnDestroy()
        {
            Debug.Log("SinglePooledDynamicSpawner-OnDestroy");
            // This example destroys the
            if (m_PrefabInstance != null)
            {
                // Always deregister the prefab
                if(PrefabToSpawn != null && NetworkManager != null && NetworkManager.PrefabHandler != null)
                    NetworkManager.PrefabHandler.RemoveHandler(PrefabToSpawn);
                Destroy(m_PrefabInstance);
            }
            base.OnDestroy();
        }
    }
}