using MultiplayerGamePrototype.Core;
using MultiplayerGamePrototype.Gameplay.NOSpawnControllers;
using MultiplayerGamePrototype.Players;
using MultiplayerGamePrototype.ScriptableObjects;
using MultiplayerGamePrototype.UI.Panels.Gameplay;
using MultiplayerGamePrototype.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UIElements;


namespace MultiplayerGamePrototype.Gameplay
{
    public class GameplayManager : SingletonMono<GameplayManager>
    {
        public static Action ActionOnImmobilizedPlayer;

        [SerializeField]
        private Camera m_MainCamera;
        public static Camera MainCamera{
            get{
                return Singleton.m_MainCamera;
            }
        }

        [SerializeField] private NOPlayerFPSController m_FPSController;
        public NOPlayerFPSController FPSController{
            get{
                return m_FPSController;
            }
        }

        [SerializeField] private SOGameData m_SOGameData;
        [SerializeField] private UIGameplayPanelsController m_UIGameplayPanelsController;

        public PlayerSpawnController PlayerSpawnController;
        public TargetObjectsSpawnController TargetObjectsSpawnController;


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
            //NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            LoadingSceneManager.ActionOnLoadClientGameplaySceneComplete += OnLoadClientGameplaySceneComplete;

            //Host must to create player object  for all players manually.
            //if (NetworkManager.Singleton.IsServer)
            //    SpawnPlayerObjectForAllConnectedPlayers();

                
            //... donT use here.
            //    Set clients when trigger "LoadComplete" or "LoadEventCompleted" or 
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


        private void SpawnPlayerObjectForAllConnectedPlayers()
        {
            Debug.Log($"GameplayManager-SpawnPlayerObjectForAllConnectedPlayers-Count:{NetworkManager.Singleton.ConnectedClientsIds.Count}");
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                PlayerSpawnController.SpawnPlayerObject(clientId);
            }
        }

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

        //private void OnClientConnectedCallback(ulong connectedClientId)
        //{
        //    if(NetworkManager.Singleton.IsServer)
        //        PlayerSpawnController.SpawnPlayerObject(connectedClientId);
        //}

        private void OnLoadClientGameplaySceneComplete(ulong clientId)
        {
            PlayerSpawnController.SpawnPlayerObject(clientId);
        }


        private void OnDestroy()
        {
            Debug.Log("GameManager-OnDestroy");
            LoadingSceneManager.ActionOnLoadClientGameplaySceneComplete -= OnLoadClientGameplaySceneComplete;
        }

        #endregion
    }
}