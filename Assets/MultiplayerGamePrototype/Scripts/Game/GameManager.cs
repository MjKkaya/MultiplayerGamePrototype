using MultiplayerGamePrototype.Players;
using MultiplayerGamePrototype.Core;
using System;
using UnityEngine;


namespace MultiplayerGamePrototype.Game
{
    public class GameManager : ManagerSingleton<GameManager>
    {
        public static Action ActionOnImmobilizedPlayer;


        [SerializeField] private NOPlayerFPSController m_FPSController;
        public NOPlayerFPSController FPSController{
            get{
                return m_FPSController;
            }
        }


        public override void Init()
        {
            base.Init();
            //m_FPSController = GetComponent<FPSController>();
        }

        public void PlayerImmobilized()
        {
            ActionOnImmobilizedPlayer?.Invoke();
        }
    }
}