using MultiplayerGamePrototype.Core;
using MultiplayerGamePrototype.Gameplay.NOSpawnControllers;
using MultiplayerGamePrototype.Players;
using MultiplayerGamePrototype.ScriptableObjects;
using MultiplayerGamePrototype.UI.Panels.Gameplay;
using MultiplayerGamePrototype.Utilities;
using MultiplayerGamePrototype.UGS.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UIElements;
using StarterAssets;
using UnityEngine.InputSystem;
using MultiplayerGamePrototype.UGS.Managers;


namespace MultiplayerGamePrototype.Gameplay
{
    public class GameplayManager : SingletonMono<GameplayManager>
    {
        public static event Action ActionOnImmobilizedPlayer;

        [SerializeField]
        private Camera m_MainCamera;
        public static Camera MainCamera{
            get{
                return Singleton.m_MainCamera;
            }
        }


        [SerializeField] private SinglePooledDynamicSpawner m_StunBombSinglePool;
        public SinglePooledDynamicSpawner StunBombSinglePool{
            get{
                return m_StunBombSinglePool;
            }
        }

        public PlayerSpawnController PlayerSpawnController;
        public TargetObjectsSpawnController TargetObjectsSpawnController;

        [SerializeField] private SOGameData m_SOGameData;
        [SerializeField] private UIGameplayPanelsController m_UIGameplayPanelsController;
        [SerializeField] private NOPlayerFPSController m_FPSController;

        private PlayerInput m_PlayerInput;
        private StarterAssetsInputAction m_StarterAssetsInputAction;


        public override void Awake()
        {
            base.Awake();
            Debug.Log("GameplayManager-Awake");
            Init();
            m_UIGameplayPanelsController.Init();
        }

        public void Init()
        {
            Debug.Log("GameplayManager-Init");
            LoadingSceneManager.ActionOnLoadClientGameplaySceneComplete += OnLoadClientGameplaySceneComplete;
        }

        public void HostPlayerSpawned()
        {
            CheckAndSpawnTargetObjects();
        }

        public void PlayerImmobilized()
        {

            ActionOnImmobilizedPlayer?.Invoke();
        }
       
        public void CheckAndSpawnTargetObjects()
        {
            if(TargetObjectsSpawnController.IsSpawnedObjectListEmpty)
            {
                int randomCount = UnityEngine.Random.Range(m_SOGameData.MinimumNumberSpawnTargetObject, m_SOGameData.MinimumNumberSpawnTargetObject * 2);
                TargetObjectsSpawnController.SpawnTargetObjects(randomCount);
            }
        }

        // Shutdown the network session and load the menu scene
        public void LeaveTheGame()
        {
            Debug.Log("GameplayManager-LeaveTheGame");
            UGSLobbyManager.Singleton.RemovePlayerAsync(UGSAuthManager.MyPlayerId);
            UGSNetworkManager.Singleton.Shutdown();
        }


        private void SpawnPlayerObjectForAllConnectedPlayers()
        {
            Debug.Log($"GameplayManager-SpawnPlayerObjectForAllConnectedPlayers-Count:{NetworkManager.Singleton.ConnectedClientsIds.Count}");
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                PlayerSpawnController.SpawnPlayerObject(clientId);
            }
        }


        #region Input Controls

        public void SetInputs(Transform followingObject, PlayerInput playerInput, StarterAssetsInputs starterAssetsInputs)
        {
            m_StarterAssetsInputAction = new();
            m_StarterAssetsInputAction.Player.TooglePause.performed += OnTooglePausePlayer;
            m_StarterAssetsInputAction.UIMenu.TooglePause.performed += OnTooglePauseUIMenu;
            m_StarterAssetsInputAction.Player.Enable();

            m_PlayerInput = playerInput;
            m_PlayerInput.SwitchCurrentActionMap("Player");
            m_FPSController.SetPlayer(followingObject, playerInput, starterAssetsInputs);
        }

        private void OnTooglePausePlayer(InputAction.CallbackContext callbackContext)
        {
            Debug.Log("GameplayManager-OnTooglePausePlayer");
            if(m_PlayerInput.enabled)
                m_PlayerInput.SwitchCurrentActionMap("UIMenu");
            m_StarterAssetsInputAction.Player.Disable();
            m_StarterAssetsInputAction.UIMenu.Enable();
        }

        private void OnTooglePauseUIMenu(InputAction.CallbackContext callbackContext)
        {
            Debug.Log("GameplayManager-OnTooglePauseUIMenu");
            if(m_PlayerInput.enabled)
                m_PlayerInput.SwitchCurrentActionMap("Player");
            m_StarterAssetsInputAction.UIMenu.Disable();
            m_StarterAssetsInputAction.Player.Enable();
        }

        #endregion


        #region Available Positions

        public List<Vector3> GetAvailablePosition(Vector3 objectSize, Vector3 areaSize, int count)
        {
            List<Vector3> positionList = new();

            float yPos = objectSize.y;
            float xDimension = (areaSize.x - objectSize.x) * 0.5f;
            float zDimension = (areaSize.z - objectSize.z) * 0.5f;

            float maxExtraIteration = count * 10;
            Vector3 randomPos;

            for (int i = 0; i < count; i++)
            {
                randomPos = transform.position + new Vector3(UnityEngine.Random.Range(-xDimension, xDimension), yPos, UnityEngine.Random.Range(-zDimension, zDimension));
                Debug.Log($"GameManager-GetAvailablePosition-randomPos:{randomPos}");
                if (IsBoxCollision(randomPos, objectSize))
                {
                    maxExtraIteration--;
                    if (maxExtraIteration == 0)
                    {
                        break;
                    }
                    else
                    {
                        i--;
                        continue;
                    }
                }

                randomPos = new Vector3(randomPos.x, yPos, randomPos.z);
                positionList.Add(randomPos);
            }

            Debug.Log($"GameManager-GetAvailablePosition-positionList:{positionList.Count}");
            return positionList;
        }

        private bool IsBoxCollision(Vector3 centerPoint, Vector3 objectSize)
        {
            int hitCount = Physics.OverlapBoxNonAlloc(centerPoint, objectSize * 0.5f, new Collider[1]);
            Debug.Log($"GameManager-IsBoxCollision-hitCount:{hitCount}");
            return hitCount > 0;
        }

        #endregion


        #region Events

        private void OnLoadClientGameplaySceneComplete(ulong clientId)
        {
            PlayerSpawnController.SpawnPlayerObject(clientId);
        }

        private void OnApplicationQuit()
        {
            Debug.Log("GameManager-OnApplicationQuit");
            LeaveTheGame();
        }

        private void OnDestroy()
        {
            Debug.Log("GameManager-OnDestroy");
            LoadingSceneManager.ActionOnLoadClientGameplaySceneComplete -= OnLoadClientGameplaySceneComplete;
            m_StarterAssetsInputAction.Dispose();
        }

        #endregion
    }
}