using MultiplayerGamePrototype.Gameplay;
using MultiplayerGamePrototype.ScriptableObjects;
using MultiplayerGamePrototype.UI.Core;
using System.Collections;
using TMPro;
using UnityEngine;


namespace MultiplayerGamePrototype.UI.Panels.Gameplay.Popups
{
    public class UIImmobilizedPlayerPopup : UIBasePopup
    {
        [SerializeField] private SOGameData m_SOGameData;
        [SerializeField] private TextMeshProUGUI m_CountdownText;

        private WaitForSeconds m_WaitForSeconds;
        private Coroutine m_StartCountDown;


        public override void Init()
        {
            Debug.Log("UIImmobilizedPlayerPopup-Init");
            base.Init();
            m_WaitForSeconds = new WaitForSeconds(1.0f);
            GameplayManager.ActionOnImmobilizedPlayer += OnImmobilizedPlayer;
            m_CountdownText.text = string.Empty;
        }

        public override void Show()
        {
            Debug.Log("UIImmobilizedPlayerPopup-Show");
            base.Show();
            if (m_StartCountDown != null)
                StopCoroutine(m_StartCountDown);
            m_StartCountDown = StartCoroutine(StartCountDown());
        }

        private IEnumerator  StartCountDown()
        {
            float waitingTime = m_SOGameData.PlyaerImmobilizedTime;
            do
            {
                m_CountdownText.text = waitingTime.ToString("f0");
                yield return m_WaitForSeconds;
                waitingTime--;
            } while (waitingTime > 0);

            Hide();
        }


        #region Events

        private void OnImmobilizedPlayer()
        {
            Debug.Log("UIImmobilizedPlayerPopup-OnImmobilizedPlayer");
            Show();
        }

        private void OnDestroy()
        {
            Debug.Log("UIImmobilizedPlayerPopup-OnDestroy");
            GameplayManager.ActionOnImmobilizedPlayer -= OnImmobilizedPlayer;
        }

        #endregion
    }
}