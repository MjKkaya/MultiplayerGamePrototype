using MultiplayerGamePrototype.Core;
using UnityEngine;


namespace MultiplayerGamePrototype.ScriptableObjects
{
    [CreateAssetMenu(fileName = "GameData", menuName = "MultiplayerGamePrototype/Data/GameData")]
    public class SOGameData : ScriptableObjectSingleton<SOGameData>
    {
        [SerializeField]
        [Tooltip("When the bomb is fired, surrounding players remain immobilized for the duration.\nValue type is seconds")]
        [Range(0, 30)]
        private float m_PlyaerImmobilizedTime;
        public float PlyaerImmobilizedTime{
            get{
                return m_PlyaerImmobilizedTime;
            }
        }

        [SerializeField]
        [Tooltip("Used at the radius value of the sphere collider of the stun bomb.\nValue type is Unity unit")]
        [Range(0, 100)]
        private float m_StunBombEffectAreaRadius;
        public float StunBombEffectAreaRadius{
            get{
                return m_StunBombEffectAreaRadius;
            }
        }
    }
} 