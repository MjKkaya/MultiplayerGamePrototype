using MultiplayerGamePrototype.Core;
using MultiplayerGamePrototype.Game.NOSpawnControllers;
using MultiplayerGamePrototype.Players;
using MultiplayerGamePrototype.ScriptableObjects;
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UIElements;


namespace MultiplayerGamePrototype.Game
{
    public class GameManager : ManagerSingleton<GameManager>
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

        public PlayerSpawnController PlayerSpawnController;
        public TargetObjectsSpawnController TargetObjectsSpawnController;


        public override void Init()
        {
            base.Init();
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
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
                int randomCount = UnityEngine.Random.Range(SOGameData.Singleton.MinimumNumberSpawnTargetObject, SOGameData.Singleton.MinimumNumberSpawnTargetObject * 2);
                TargetObjectsSpawnController.SpawnTargetObjects(randomCount);
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


        private void OnClientConnectedCallback(ulong connectedClientId)
        {
            if(NetworkManager.Singleton.IsServer)
                PlayerSpawnController.SpawnPlayerObject(connectedClientId);
        }

        private void OnDestroy()
        {
            if(NetworkManager.Singleton != null)
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
        }

        #endregion
    }
}