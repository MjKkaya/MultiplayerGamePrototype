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

        private NetworkVariable<FixedString64Bytes> m_PlayerId = new NetworkVariable<FixedString64Bytes>(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Owner);

        [SerializeField] private Transform m_TransformPlayerCameraRoot;
        [SerializeField] private StarterAssetsInputs m_StarterAssetsInputs;
        [SerializeField] private PlayerInput m_PlayerInput;


        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Debug.Log($"{name}-OnNetworkSpawn");
            m_PlayerId.OnValueChanged += PlayerIdOnValueChanged;
            if (IsOwner)
                InitFPSControls();
        }

        private void InitFPSControls()
        {
            string username = UGSAuthManager.MyUsername;
            m_PlayerId.Value = UGSAuthManager.MyPlayerId;
            name = $"Player-{username}";
            Debug.Log($"{name}-InitFPSControls");
            m_PlayerInput.enabled = true;
            GameManager.Singleton.FPSController.SetFPSPlayer(m_TransformPlayerCameraRoot, m_PlayerInput, m_StarterAssetsInputs);
            if (IsServer)
                GameManager.Singleton.HostSpawned();
        }

        private void PlayerIdOnValueChanged(FixedString64Bytes oldValue, FixedString64Bytes newValue)
        {
            Debug.Log($"{name}-PlayerIdOnValueChanged-oldValue:{oldValue}, newValue:{newValue}, current:{m_PlayerId.Value}");
        }
    }
}