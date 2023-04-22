using Unity.Netcode;
using UnityEngine;


namespace MultiplayerGamePrototype.Game.Targets
{
    public class NOSimpleTarget : NetworkBehaviour
    {
        public void Hit()
        {
            Debug.Log($"{name}-Hit");
            HitClientRPC();
        }


        [ClientRpc]
        public void HitClientRPC()
        {
            Debug.Log($"{name}-HitClientRPC");
            //todo: add particle effect
            if(IsServer)
            {
                GameManager.Singleton.TargetObjectsSpawnController.DespawnTargetObject(NetworkObject);
            }
        }

        //public override void OnNetworkDespawn()
        //{
        //    base.OnNetworkDespawn();
        //    if(IsServer)
        //    {
        //        Debug.Log($"{name}-OnNetworkDespawn");
        //        GameManager.Singleton.NOTargetObjectsSpawnController.();
        //    }
        //}
    }
}