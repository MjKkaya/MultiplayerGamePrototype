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

        [SerializeField] private Transform _transformPlayerCameraRoot;
        [SerializeField] private StarterAssetsInputs _starterAssetsInputs;
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private FirstPersonController _firstPersonController;
        [SerializeField] private ThirdPersonController _thirdPersonController;
        [SerializeField] private bool _isFirstPersonControllerActive;


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
            _firstPersonController.enabled = _isFirstPersonControllerActive;
            _thirdPersonController.enabled = !_isFirstPersonControllerActive;
            _playerInput.enabled = true;
            GameplayManager.Singleton.SetInputs(_transformPlayerCameraRoot, _playerInput, _starterAssetsInputs, _isFirstPersonControllerActive);
            if (IsServer)
                GameplayManager.Singleton.HostPlayerSpawned();
        }

        private void PlayerIdOnValueChanged(FixedString64Bytes oldValue, FixedString64Bytes newValue)
        {
            Debug.Log($"{name}-PlayerIdOnValueChanged-oldValue:{oldValue}, newValue:{newValue}, current:{m_PlayerId.Value}");
        }
    }
}