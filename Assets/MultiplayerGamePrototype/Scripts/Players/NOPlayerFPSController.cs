using Cinemachine;
using MultiplayerGamePrototype.ScriptableObjects;
using MultiplayerGamePrototype.Gameplay;
using StarterAssets;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


namespace MultiplayerGamePrototype.Players
{
    public class NOPlayerFPSController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera m_CinemachineVirtualCamera;
        [SerializeField] private UICanvasControllerInput m_UICanvasControllerInput;
        [SerializeField] private MobileDisableAutoSwitchControls m_MobileDisableAutoSwitchControls;
        [SerializeField] private SOGameData m_SOGameData;

        private Coroutine m_ImmobilizedCoroutine;
        private PlayerInput m_PlayerInput;

        /// <summary>
        ///FPS chracter is a network object,  because of this we have to assign some components after that instance created.
        /// </summary>
        /// <param name="transform">FPS character camera control</param>
        /// <param name="starterAssetsInputs">For FPS charceter inputs</param>
        public void SetPlayer(Transform followingObject, PlayerInput playerInput, StarterAssetsInputs starterAssetsInputs)
        {
            m_CinemachineVirtualCamera.Follow = followingObject;
            m_UICanvasControllerInput.starterAssetsInputs = starterAssetsInputs;
            m_PlayerInput = playerInput;
#if ENABLE_INPUT_SYSTEM && (UNITY_IOS || UNITY_ANDROID)
            m_MobileDisableAutoSwitchControls.playerInput = playerInput;
#endif
            m_UICanvasControllerInput.gameObject.SetActive(true);
        }

        IEnumerator ImmobilizedCoroutine()
         {
            m_PlayerInput.enabled = false;
            yield return new WaitForSeconds(m_SOGameData.PlyaerImmobilizedTime);
            m_PlayerInput.enabled = true;
 //#if ENABLE_INPUT_SYSTEM && (UNITY_IOS || UNITY_ANDROID)
 //            m_MobileDisableAutoSwitchControls.playerInput.enabled = false;
 //            m_MobileDisableAutoSwitchControls.playerInput.enabled = true;
 //#else
 //            yield return null;
 //#endif
         }
        
        #region Events


        private void ImmobilizedPlayer()
        {
            if (m_ImmobilizedCoroutine != null)
                StopCoroutine(m_ImmobilizedCoroutine);
            m_ImmobilizedCoroutine = StartCoroutine(ImmobilizedCoroutine());
        }

        private void Awake()
        {
            GameplayManager.ActionOnImmobilizedPlayer += ImmobilizedPlayer;
        }

        private void OnDestroy()
        {
            GameplayManager.ActionOnImmobilizedPlayer -= ImmobilizedPlayer;
        }

        #endregion

    }
}