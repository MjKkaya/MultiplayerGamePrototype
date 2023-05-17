using MultiplayerGamePrototype.UGS.Managers;
using MultiplayerGamePrototype.Gameplay;
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
        [SerializeField] private FirstPersonController m_FirstPersonController;


        private void Awake()
        {
            Debug.Log($"{name}-Awake");
        }

        private void Start()
        {
            Debug.Log($"{name}-Start");
        }

        public override void OnNetworkSpawn()
        {
            Debug.Log($"{name}-OnNetworkSpawn-IsServer:{IsServer}, pos:{transform.position}");
            if (IsServer && !IsOwner)
                transform.position = GameplayManager.Singleton.PlayerSpawnController.GetAvaiablePosition();
            base.OnNetworkSpawn();
            Debug.Log($"{name}-OnNetworkSpawn-OwnerClientId:{OwnerClientId}, pos:{transform.position}");
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
            m_FirstPersonController.enabled = true;
            m_PlayerInput.enabled = true;
            GameplayManager.Singleton.SetInputs(m_TransformPlayerCameraRoot, m_PlayerInput, m_StarterAssetsInputs);
            if (IsServer)
                GameplayManager.Singleton.HostPlayerSpawned();
        }

        private void PlayerIdOnValueChanged(FixedString64Bytes oldValue, FixedString64Bytes newValue)
        {
            Debug.Log($"{name}-PlayerIdOnValueChanged-oldValue:{oldValue}, newValue:{newValue}, current:{m_PlayerId.Value}");
        }
    }
}