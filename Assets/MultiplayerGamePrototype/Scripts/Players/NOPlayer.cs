using MultiplayerGamePrototype.Game;
using MultiplayerGamePrototype.UGS.Managers;
using MultiplayerGamePrototype.UGS.DataControllers;
using StarterAssets;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;


namespace MultiplayerGamePrototype.Players
{ 
    public class NOPlayer : NetworkBehaviour
    {
        //"Time required to pass before being able to shot again. Set to 0f to instantly shot again"
        private static float SHOT_TIMEOUT= 0.25f;


        [SerializeField] private Transform m_TransformPlayerCameraRoot;
        [SerializeField] private StarterAssetsInputs m_StarterAssetsInputs;
        [SerializeField] private PlayerInput m_PlayerInput;

        private float m_shotTimeoutDelta;


        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            m_shotTimeoutDelta = SHOT_TIMEOUT;
            if (IsOwner)
                InitFPSControls();
        }

        private void InitFPSControls()
        {
            string username = UGSAuthManager.MyUsername;
            name = $"Player-{username}";
            Debug.Log($"{name}-InitFPSControls");
            m_PlayerInput.enabled = true;
            GameManager.Singleton.FPSController.SetFPSPlayer(m_TransformPlayerCameraRoot, m_PlayerInput, m_StarterAssetsInputs);
        }

        private void Shot()
        {
            Debug.Log($"{name}-Shot");
            m_StarterAssetsInputs.shot = false;
            m_shotTimeoutDelta = SHOT_TIMEOUT;
        }


        #region Events

        private void Update()
        {
            if (!IsOwner)
                return;


            if(m_shotTimeoutDelta > 0.0f)
                m_shotTimeoutDelta -= Time.deltaTime;

            if (m_StarterAssetsInputs.shot)
            {
                if (m_shotTimeoutDelta <= 0)
                    Shot();
                else
                    m_StarterAssetsInputs.shot = false;
            }
        }

        #endregion
    }
}