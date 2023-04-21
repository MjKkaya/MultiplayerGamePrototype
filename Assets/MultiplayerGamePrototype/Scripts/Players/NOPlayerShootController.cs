using MultiplayerGamePrototype.Game;
using MultiplayerGamePrototype.Game.Targets;
using MultiplayerGamePrototype.UGS.DataControllers;
using MultiplayerGamePrototype.ScriptableObjects;
using StarterAssets;
using Unity.Netcode;
using UnityEngine;
using MultiplayerGamePrototype.UGS.Managers;

namespace MultiplayerGamePrototype.Players
{
    [RequireComponent(typeof(NOPlayer))]
    public class NOPlayerShootController : NetworkBehaviour
    {
        //"Time required to pass before being able to shot again. Set to 0f to instantly shot again"
        private static readonly float SHOT_TIMEOUT = 0.25f;
        private static readonly float SCREEN_SIZE_TIMEOUT = 5f;


        [SerializeField] Transform m_CameraTransform;
        [SerializeField] private StarterAssetsInputs m_StarterAssetsInputs;
        [SerializeField] private LayerMask m_TargetMask;

        private float m_shotTimeout;
        private float m_screenSizeTimeout;
        private Vector2 screenCenterPoint;
        private NOPlayer m_NOPlayer;


        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            m_NOPlayer = GetComponent<NOPlayer>();
            m_shotTimeout = SHOT_TIMEOUT;
            SetScreenCenterPoint();
        }


        private void Shot()
        {
            Debug.Log($"{name}-Shot");
            //todo: make sound
            m_shotTimeout = SHOT_TIMEOUT;
            m_StarterAssetsInputs.shot = false;
            ShootBulletServerRpc();
        }

        private void SetScreenCenterPoint()
        {
            screenCenterPoint = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            m_screenSizeTimeout = SCREEN_SIZE_TIMEOUT;
            //Debug.Log($"{name}-SetScreenCenterPoint:{screenCenterPoint}");
        }

        /// <summary>
        /// Just Host can run this method!
        /// </summary>
        private async void GivePointForHitTheTarget()
        {
            string playerId = m_NOPlayer.PlayerId;
            bool isCorrectBullet = UGSLobbyDataController.IsPlayerBulletTypeEqualsToLobbyBulletType(playerId);
            Debug.Log($"{name}-GivePointForHitTheTarget-PlayerId:{playerId}-isCorrectBullet:{isCorrectBullet}");
            int score = isCorrectBullet ? SOGameData.Singleton.HittingTargetScore : -SOGameData.Singleton.HittingTargetScore;
            await UGSLobbyManager.Singleton.UpdateLobbyDataAsync(UGSLobbyDataController.IncreasePlayerScoreStat(playerId, score));
        }

        #region RPC

        [ServerRpc]
        private void ShootBulletServerRpc()
        {
            Ray ray = new(m_CameraTransform.position, m_CameraTransform.TransformDirection(Vector3.forward));
            if (Physics.Raycast(ray, out RaycastHit raycastHit, GameManager.MainCamera.farClipPlane * 0.5f, m_TargetMask))
            {
                Debug.Log($"{name}-raycastHit:{raycastHit.transform.name}");
                if(raycastHit.transform.TryGetComponent(out NOSimpleTarget target))
                {
                    GivePointForHitTheTarget();
                    target.Hit();
                }
            }
        }

        #endregion


        #region Events

        private void Update()
        {
            Debug.DrawRay(m_CameraTransform.position, m_CameraTransform.TransformDirection(Vector3.forward) * GameManager.MainCamera.farClipPlane, Color.green);

            if (!IsOwner)
                return;

            if (m_shotTimeout > 0.0f)
                m_shotTimeout -= Time.deltaTime;

            if (m_StarterAssetsInputs.shot)
            {
                if (m_shotTimeout <= 0)
                    Shot();
                else
                    m_StarterAssetsInputs.shot = false;
            }

            if (m_screenSizeTimeout > 0.0f)
                m_screenSizeTimeout -= Time.deltaTime;
            else
                SetScreenCenterPoint();
        }
        #endregion
    }
}
