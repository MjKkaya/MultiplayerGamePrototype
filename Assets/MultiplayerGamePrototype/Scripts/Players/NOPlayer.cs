using MultiplayerGamePrototype.UGS.Managers;
using MultiplayerGamePrototype.Game;
using StarterAssets;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Collections;


namespace MultiplayerGamePrototype.Players
{ 
    public class NOPlayer : NetworkBehaviour
    {
        public string PlayerId{
            get{
                return m_PlayerId.Value.ToString();
            }

        }

        private NetworkVariable<FixedString64Bytes> m_PlayerId = new NetworkVariable<FixedString64Bytes>();

        [SerializeField] private Transform m_TransformPlayerCameraRoot;
        [SerializeField] private StarterAssetsInputs m_StarterAssetsInputs;
        [SerializeField] private PlayerInput m_PlayerInput;


        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsOwner)
                InitFPSControls();
        }

        private void InitFPSControls()
        {
            m_PlayerId = new NetworkVariable<FixedString64Bytes>(UGSAuthManager.MyPlayerId, readPerm: NetworkVariableReadPermission.Owner, writePerm:NetworkVariableWritePermission.Owner);
            string username = UGSAuthManager.MyUsername;
            name = $"Player-{username}";
            Debug.Log($"{name}-InitFPSControls");
            m_PlayerInput.enabled = true;
            GameManager.Singleton.FPSController.SetFPSPlayer(m_TransformPlayerCameraRoot, m_PlayerInput, m_StarterAssetsInputs);
        }
    }
}