using System.Globalization;
using Unity.Netcode;
using UnityEngine;


namespace MultiplayerGamePrototype.UGS.Utilities
{
    public class NetcodeCharacterController : NetworkBehaviour
    {
        private CharacterController m_CharacterController;

        private void Awake()
        {
            m_CharacterController = GetComponent<CharacterController>();
            if (m_CharacterController != null)
            {
                // Start every netcode cloned instance with CharacterController disabled
                m_CharacterController.enabled = false;
            }
        }

        public override void OnNetworkSpawn()
        {
            // Owner enables when spawned
            if (IsOwner && m_CharacterController != null)
            {
                m_CharacterController.enabled = true;
            }

            base.OnNetworkSpawn();
        }
    }
}