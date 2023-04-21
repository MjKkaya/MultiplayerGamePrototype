using MultiplayerGamePrototype.ScriptableObjects;
using Unity.Netcode;
using UnityEngine;


namespace MultiplayerGamePrototype.Game.Weapons
{
    public class NOBullet : NetworkBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (IsServer)
            {
                if (other.CompareTag(SOConfigData.Singleton.Tag_Targets))
                {
                    NetworkObject.Despawn();
                }
            }    
        }
    }
}