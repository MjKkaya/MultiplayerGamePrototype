using UnityEngine;


namespace MultiplayerGamePrototype.ScriptableObjects
{
    [CreateAssetMenu(fileName = "GameData", menuName = "MultiplayerGamePrototype/Data/GameData")]
    public class SOGameData : ScriptableObject
    {
        [SerializeField]
        [Tooltip("When the bomb is fired, surrounding players remain immobilized for the duration.\nValue type is seconds")]
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

        [SerializeField]
        private int m_HittingTargetScore;
        public int HittingTargetScore{
            get{
                return m_HittingTargetScore;
            }
        }

        [SerializeField]
        private int m_MinimumNumberSpawnTargetObject;
        public int MinimumNumberSpawnTargetObject{
            get{
                return m_MinimumNumberSpawnTargetObject;
            }
        }
    }
} 