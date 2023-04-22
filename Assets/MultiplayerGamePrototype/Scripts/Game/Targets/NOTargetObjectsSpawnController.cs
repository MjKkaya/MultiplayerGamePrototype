using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


namespace MultiplayerGamePrototype.Game.Targets
{
    public class NOTargetObjectsSpawnController : MonoBehaviour
    {
        [SerializeField]
        private Transform m_TragetObjectPrefab;

        //The area is rectangular
        [SerializeField]
        private Transform m_SpawnArea;

        List<NetworkObject> m_SpawnedTargetObjects = new();


        public bool IsSpawnedObjectListEmpty{
            get{
                return m_SpawnedTargetObjects.Count == 0;
            }
        }

        public void SpawnTargetObjects(int count)
        {
            m_SpawnedTargetObjects.Clear();
            NetworkObject networkObject;
            Vector3 randomPos;

            float yPos = m_TragetObjectPrefab.localScale.y;
            float xDimension = (m_SpawnArea.localScale.x - m_TragetObjectPrefab.localScale.x) * 0.5f;

            //Used m_SpawnArea.localScale.y because m_SpawnArea rotated 
            float zDimension = (m_SpawnArea.localScale.y - m_TragetObjectPrefab.localScale.y) * 0.5f;

            float maxExtraIteration = count;
            for (int i = 0; i < count; i++)
            {
                randomPos = transform.position + new Vector3(Random.Range(-xDimension, xDimension), yPos, Random.Range(-zDimension, zDimension));
                Debug.Log($"NOTargetObjectsSpawnController-SpawnTargetObjects-randomPos:{randomPos}");
                if(IsBoxCollision(randomPos, m_TragetObjectPrefab.localScale))
                {
                    maxExtraIteration--;
                    if(maxExtraIteration == 0)
                    {
                        break;
                    }
                    else
                    {
                        i--;
                        continue;
                    }
                }
                randomPos = new Vector3(randomPos.x, yPos * 4, randomPos.z);
                networkObject = Object.Instantiate(m_TragetObjectPrefab, randomPos, Quaternion.identity).GetComponent<NetworkObject>();
                networkObject.Spawn();
                m_SpawnedTargetObjects.Add(networkObject);
            }
        }


        public void DespawnTargetObject(NetworkObject spawnedObject)
        {
            m_SpawnedTargetObjects.Remove(spawnedObject);
            spawnedObject.Despawn();
            GameManager.Singleton.CheckAndSpawnTargetObjects();
        }


        private bool IsBoxCollision(Vector3 centerPoint, Vector3 boxSize)
        {
            int hitCount = Physics.OverlapBoxNonAlloc(centerPoint, boxSize * 0.5f, new Collider[1]);
            Debug.Log($"NOTargetObjectsSpawnController-IsBoxCollision-hitCount:{hitCount}");
            return hitCount > 0;
        }
    }
}