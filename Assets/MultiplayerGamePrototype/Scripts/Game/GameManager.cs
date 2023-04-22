using MultiplayerGamePrototype.Core;
using MultiplayerGamePrototype.UGS.Managers;
using MultiplayerGamePrototype.Game.Targets;
using MultiplayerGamePrototype.Players;
using MultiplayerGamePrototype.ScriptableObjects;
using System;
using UnityEngine;
using Unity.Netcode;

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

        public NOTargetObjectsSpawnController NOTargetObjectsSpawnController;


        public override void Init()
        {
            base.Init();
            UGSRelayManager.ActionOnJoinedRelayServer += OnJoinedRelayServer;
        }

        public void HostSpawned()
        {
            CheckAndSpawnTargetObjects();
        }

        public void PlayerImmobilized()
        {
            ActionOnImmobilizedPlayer?.Invoke();
        }
       
        public void CheckAndSpawnTargetObjects()
        {
            if(NOTargetObjectsSpawnController.IsSpawnedObjectListEmpty)
            {
                int randomCount = UnityEngine.Random.Range(SOGameData.Singleton.MinimumNumberSpawnTargetObject, SOGameData.Singleton.MinimumNumberSpawnTargetObject * 2);
                NOTargetObjectsSpawnController.SpawnTargetObjects(SOGameData.Singleton.MinimumNumberSpawnTargetObject);
            }
        }


        public void SpawnPlayerObjectThanChangeOwnerShip(ulong joinedClientId)
        {
            Debug.Log($"GameManager-SpawnPlayerObjectThanChangeOwnerShip-joinedClientId:{joinedClientId}");
        }

        #region Available Positions


        #endregion


        #region Events

        private void OnJoinedRelayServer()
        {
            SpawnPlayerObjectThanChangeOwnerShip(NetworkManager.Singleton.LocalClientId);
        }

        private void OnDestroy()
        {
            UGSRelayManager.ActionOnJoinedRelayServer -= OnJoinedRelayServer;
        }

        #endregion
    }
}