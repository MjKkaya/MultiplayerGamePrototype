using MultiplayerGamePrototype.ScriptableObjects;
using MultiplayerGamePrototype.Gameplay;
using StarterAssets;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;


namespace MultiplayerGamePrototype.Players
{
    public class NOPlayerFPSController : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera _cinemachineCamera;
        [SerializeField] private CinemachineThirdPersonFollow _cinemachineThirdPersonFollow;
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
        public void SetPlayer(Transform followingObject, PlayerInput playerInput, StarterAssetsInputs starterAssetsInputs, bool _isFirstPersonControllerActive)
        {
            _cinemachineCamera.Follow = followingObject;
            m_UICanvasControllerInput.starterAssetsInputs = starterAssetsInputs;
            m_PlayerInput = playerInput;
#if ENABLE_INPUT_SYSTEM && (UNITY_IOS || UNITY_ANDROID)
            m_MobileDisableAutoSwitchControls.playerInput = playerInput;
#endif
            m_UICanvasControllerInput.gameObject.SetActive(true);

            _cinemachineThirdPersonFollow.CameraDistance = _isFirstPersonControllerActive ? 0 : 4;
        }

        IEnumerator ImmobilizedCoroutine()
         {
 #if ENABLE_INPUT_SYSTEM && (UNITY_IOS || UNITY_ANDROID)
            m_UICanvasControllerInput.SetActiveInputs(false);
            // m_MobileDisableAutoSwitchControls.playerInput.enabled = false;
 #endif
            // m_PlayerInput.enabled = false;
            m_PlayerInput.DeactivateInput();

            yield return new WaitForSeconds(m_SOGameData.PlyaerImmobilizedTime);

            // m_PlayerInput.enabled = true;
            m_PlayerInput.ActivateInput();
 #if ENABLE_INPUT_SYSTEM && (UNITY_IOS || UNITY_ANDROID)
            m_UICanvasControllerInput.SetActiveInputs(true);
            // m_MobileDisableAutoSwitchControls.playerInput.enabled = true;
 #endif
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