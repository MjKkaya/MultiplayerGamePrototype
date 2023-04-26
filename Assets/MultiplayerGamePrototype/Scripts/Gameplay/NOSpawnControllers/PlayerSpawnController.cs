using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


namespace MultiplayerGamePrototype.Gameplay.NOSpawnControllers
{
    public class PlayerSpawnController : MonoBehaviour
    {
        private static readonly Vector3 DEFAULT_POSITION = new (0.0f, 2.0f, 0.0f); 

        [SerializeField] private Transform m_PlayerPrefab;

        //The area is rectangular
        [SerializeField]
        private Transform m_SpawnArea;

        private Vector3 areaSize;


        private void Start()
        {
            areaSize = new Vector3(m_SpawnArea.localScale.x, 0.0f, m_SpawnArea.localScale.y);
        }

        public void SpawnPlayerObject(ulong joinedClientId)
        {
            Debug.Log($"PlayerSpawnController-SpawnPlayerObject-joinedClientId:{joinedClientId}");
            //List<Vector3> positions = GameManager.Singleton.GetAvailablePosition(m_PlayerPrefab.localScale, areaSize, 1);
            //Vector3 position = positions.Count == 0 ? DEFAULT_POSITION : positions[0];
            Transform newGameObject;
            if (joinedClientId == NetworkManager.ServerClientId)
                newGameObject = Instantiate(m_PlayerPrefab, GetAvaiablePosition(), Quaternion.identity);
            else
                newGameObject = Instantiate(m_PlayerPrefab);
            NetworkObject newGameObjectNetworkObject = newGameObject.GetComponent<NetworkObject>();
            newGameObjectNetworkObject.SpawnAsPlayerObject(joinedClientId, true);
        }

        public Vector3 GetAvaiablePosition()
        {
            List<Vector3> positions = GameplayManager.Singleton.GetAvailablePosition(m_PlayerPrefab.localScale, areaSize, 1);
            Vector3 position = positions.Count == 0 ? DEFAULT_POSITION : positions[0];
            position = new Vector3(position.x, position.y * 2, position.z);
            Debug.Log($"PlayerSpawnController-GetAvaiablePosition-position:{position}");
            return position;
        }
    }
}