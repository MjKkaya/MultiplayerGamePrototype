using MultiplayerGamePrototype.Gameplay;
using MultiplayerGamePrototype.Gameplay.Targets;
using MultiplayerGamePrototype.UGS.DataControllers;
using MultiplayerGamePrototype.ScriptableObjects;
using StarterAssets;
using Unity.Netcode;
using UnityEngine;
using MultiplayerGamePrototype.UGS.Managers;
using MultiplayerGamePrototype.Events;

namespace MultiplayerGamePrototype.Players
{
    [RequireComponent(typeof(NOPlayer))]
    public class NOPlayerShootController : NetworkBehaviour
    {
        //"Time required to pass before being able to shot again. Set to 0f to instantly shot again"
        private static readonly float SHOT_TIMEOUT = 0.25f;

        [SerializeField] private SOGameData m_SOGameData;
        [SerializeField] Transform m_CameraTransform;
        [SerializeField] private StarterAssetsInputs m_StarterAssetsInputs;
        [SerializeField] private LayerMask m_TargetMask;

        private float m_shotTimeout;
        private NOPlayer m_NOPlayer;


        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            m_NOPlayer = GetComponent<NOPlayer>();
            m_shotTimeout = SHOT_TIMEOUT;
        }


        private void Shot()
        {
            Debug.Log($"{name}-Shot");
            //todo: add sound
            m_shotTimeout = SHOT_TIMEOUT;
            m_StarterAssetsInputs.shot = false;
            ShootBulletServerRpc();
        }


        /// <summary>
        /// Host adds/decreases score for hitted target.
        /// </summary>
        private async void GivePointToPlayerForHitTarget()
        {
            string playerId = m_NOPlayer.PlayerId;
            bool isCorrectBullet = UGSLobbyDataController.IsPlayerBulletTypeEqualsToLobbyBulletType(playerId);
            Debug.Log($"{name}-GivePointToPlayerForHitTarget-PlayerId:{playerId}-isCorrectBullet:{isCorrectBullet}");
            int score = isCorrectBullet ? m_SOGameData.HittingTargetScore : -m_SOGameData.HittingTargetScore;
            int newScore = UGSLobbyDataController.CalculatePlayerScoreData(playerId, score);
            await UGSLobbyManager.Singleton.UpdateLobbyDataAsync(UGSLobbyDataController.UpdatePlayerScoreData(playerId, newScore));
            LobbyEvents.OnChangedPlayerScore?.Invoke(playerId, newScore.ToString());
        }

        #region RPC

        [ServerRpc]
        private void ShootBulletServerRpc()
        {
            Ray ray = new(m_CameraTransform.position, m_CameraTransform.TransformDirection(Vector3.forward));
            Debug.Log($"{name}-ShootBulletServerRpc-position:{m_CameraTransform.position}, m_TargetMask:{m_TargetMask}");
            if (Physics.Raycast(ray, out RaycastHit raycastHit, GameplayManager.MainCamera.farClipPlane * 0.5f, m_TargetMask))
            {
                Debug.Log($"{name}-ShootBulletServerRpc-raycastHit:{raycastHit.transform.name}");
                if(raycastHit.transform.TryGetComponent(out NOSimpleTarget target))
                {
                    target.Hit();
                    GivePointToPlayerForHitTarget();
                }
            }
        }

        #endregion


        #region Events

        private void Update()
        {
            if(UGSNetworkManager.Singleton.IsHost)
                Debug.DrawRay(m_CameraTransform.position, m_CameraTransform.TransformDirection(Vector3.forward) * GameplayManager.MainCamera.farClipPlane, Color.green);

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
        }
        #endregion
    }
}
