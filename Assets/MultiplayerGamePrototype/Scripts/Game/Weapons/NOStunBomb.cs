using MultiplayerGamePrototype.ScriptableObjects;
using MultiplayerGamePrototype.Players;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


namespace MultiplayerGamePrototype.Game.Weapons
{
    public class NOStunBomb : NetworkBehaviour
    {
        public ulong[] CalculateEffectedPlayers()
        {
            Debug.Log("CalculateEffectedPlayers");
            List<ulong> effectedClientIdList = new();
            Collider[] colliders = Physics.OverlapSphere(transform.position, SOGameData.Singleton.StunBombEffectAreaRadius);
            int count = colliders.Length;
            Debug.Log($"CalculateEffectedPlayers-count:{count}");
            Collider Collider;
            for (int i = 0; i < count; i++)
            {
                Collider = colliders[i];
                if (Collider.CompareTag("Player"))
                {
                    Debug.Log($"NOStunBomb-Name:{Collider.transform.name}, id:{Collider.transform.GetComponent<NOPlayer>().OwnerClientId}");
                    effectedClientIdList.Add(Collider.transform.GetComponent<NOPlayer>().OwnerClientId);
                }
            }

            return effectedClientIdList.ToArray();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, SOGameData.Singleton.StunBombEffectAreaRadius);
        }
    }
}
