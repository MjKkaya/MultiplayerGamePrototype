using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;


namespace MultiplayerGamePrototype.UI.Panels.GamePanels
{
    public class UIPlayerScoreStatItem : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI m_UsernameText;
        [SerializeField] TextMeshProUGUI m_ScoreText;

        private string m_PlayerId;


        public void Init(string playerId, string username, int score)
        {
            m_PlayerId = playerId;
            m_UsernameText.text = username;
            UpdateScore(score);
        }

        public void UpdateScore(int score)
        {
            m_ScoreText.text = score.ToString();
        }
    }
}