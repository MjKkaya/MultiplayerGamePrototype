using MultiplayerGamePrototype.Game;
using MultiplayerGamePrototype.Game.Weapons;
using StarterAssets;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;


namespace MultiplayerGamePrototype.Players
{
    public class NOPlayerStunBombController : NetworkBehaviour
    {
        //"Time required to wait before being able to fire the bomd
        private static float BOMB_WAITING_TIMEOUT = 5f;

        [SerializeField] Transform m_BombPrefab;
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

            // get a reference to our main camera
            if (m_mainCamera == null)
                m_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }


        private void DeployTheBomb()
        {
            Debug.Log($"{name}-DeployTheBomb");
            m_StarterAssetsInputs.bomb = false;
            isBombDeployed = true;
            m_bombWaitingTimeoutDelta = BOMB_WAITING_TIMEOUT;
            DeployTheBombServerRPC();
        }

        private void FireTheBomb()
        {
            Debug.Log($"{name}-FireTheBomb");
            m_StarterAssetsInputs.bomb = false;
            isBombDeployed = false;
            m_bombWaitingTimeoutDelta = BOMB_WAITING_TIMEOUT;
            FireTheBombServerRPC();
        }


        [ServerRpc]
        public void DeployTheBombServerRPC()
        {
            Debug.Log($"{name}-DeployTheBombServerRPC!");
            Transform newGameObject = Object.Instantiate(m_BombPrefab, m_BombDeployPosition);

            // Replicating that same new instance to all connected clients
            m_BombNetworkObject = newGameObject.GetComponent<NetworkObject>();
            m_BombNetworkObject.Spawn();
        }


        [ServerRpc]
        public void FireTheBombServerRPC()
        {
            Debug.Log($"{name}-FireTheBombServerRPC!");
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

        [ClientRpc]
        public void FireTheBombClientRPC()
        {
            Debug.Log($"{name}-FireTheBombClientRPC!");
        }

        [ClientRpc]
        public void ImmobilizPlayerClientRPC(ClientRpcParams clientRpcParams = default)
        {
            Debug.Log($"{name}-ImmobilizPlayerClientRPC");
            GameManager.Singleton.PlayerImmobilized();
        }


        private void Update()
        {
            if (!IsOwner)
                return;

            // One button control Deploy and Fire the bomb. Each waiting time is 5 second after any action.
            if (m_StarterAssetsInputs.bomb == true){
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

            if(Input.GetKeyDown(KeyCode.C))
                m_BombNetworkObject.GetComponent<NOStunBomb>().CalculateEffectedPlayers();
        }
    }
}