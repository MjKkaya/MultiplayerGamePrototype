using MultiplayerGamePrototype.Gameplay;
using MultiplayerGamePrototype.Gameplay.Weapons;
using StarterAssets;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;


namespace MultiplayerGamePrototype.Players
{
    public class NOPlayerStunBombController : NetworkBehaviour
    {
        //"Time required to wait before being able to fire the bomd
        private static float BOMB_TIMEOUT = 5f;

        [SerializeField] Transform m_BombDeployPosition;
        [SerializeField] private StarterAssetsInputs m_StarterAssetsInputs;

        private bool isBombDeployed = false;
        private float m_bombWaitingTimeoutDelta;

        private GameObject m_mainCamera;
        private NetworkObject m_BombNetworkObject;


        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            m_bombWaitingTimeoutDelta = 0.0f;

            if (m_mainCamera == null)
                m_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }


        private void DeployTheBomb()
        {
            Debug.Log($"{name}-DeployTheBomb");
            m_StarterAssetsInputs.bomb = false;
            isBombDeployed = true;
            m_bombWaitingTimeoutDelta = BOMB_TIMEOUT;
            DeployTheBombServerRPC();
        }

        private void FireTheBomb()
        {
            Debug.Log($"{name}-FireTheBomb");
            m_StarterAssetsInputs.bomb = false;
            isBombDeployed = false;
            m_bombWaitingTimeoutDelta = BOMB_TIMEOUT;
            FireTheBombServerRPC();
        }


        #region RPC

        [ServerRpc]
        public void DeployTheBombServerRPC()
        {
            Debug.Log($"{name}-DeployTheBombServerRPC!");
            //// Replicating that same new instance to all connected clients
            m_BombNetworkObject = GameplayManager.Singleton.StunBombSinglePool.SpawnInstance(m_BombDeployPosition.position);
        }

        [ServerRpc]
        public void FireTheBombServerRPC()
        {
            Debug.Log($"{name}-FireTheBombServerRPC!");
            if (m_BombNetworkObject == null)
                return;

            ulong[] effectedClientIds = m_BombNetworkObject.GetComponent<NOStunBomb>().CalculateEffectedPlayers();
            m_BombNetworkObject.Despawn();
            FireTheBombClientRPC();

            ClientRpcParams clientRpcParams = new ClientRpcParams{
                Send = new ClientRpcSendParams{
                    TargetClientIds = effectedClientIds
                }
            };

            ImmobilizPlayerClientRPC(clientRpcParams);
        }

        /// <summary>
        /// Reaches all players.
        /// </summary>
        [ClientRpc]
        public void FireTheBombClientRPC()
        {
            Debug.Log($"{name}-FireTheBombClientRPC!");
        }

        /// <summary>
        /// Reaches just effected players.
        /// </summary>
        [ClientRpc]
        public void ImmobilizPlayerClientRPC(ClientRpcParams clientRpcParams = default)
        {
            Debug.Log($"{name}-ImmobilizPlayerClientRPC");
            GameplayManager.Singleton.PlayerImmobilized();
        }

        #endregion


        #region Events

        private void Update()
        {
            if (!IsOwner)
                return;

            // One button control Deploy and Fire the bomb. Each waiting time is 5 second after any action.
            if (m_StarterAssetsInputs.bomb){
                if (m_bombWaitingTimeoutDelta <= 0)
                {
                    if (!isBombDeployed)
                        DeployTheBomb();
                    else
                        FireTheBomb();
                }
                else
                {
                    m_StarterAssetsInputs.bomb = false;
                    Debug.Log($"{name}-Wait for the bomb time:{m_bombWaitingTimeoutDelta}");
                }
            }    

            if (m_bombWaitingTimeoutDelta > 0)
                m_bombWaitingTimeoutDelta -= Time.deltaTime;
        }

        #endregion
    }
}