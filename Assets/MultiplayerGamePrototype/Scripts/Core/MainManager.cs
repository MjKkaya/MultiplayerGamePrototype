using UnityEngine;


namespace MultiplayerGamePrototype.Core
{
    #region Managers

    public abstract class ManagerBase : MonoBehaviour
    {
        public abstract void Init();
    }
    public class ManagerSingleton<T> : ManagerBase where T : ManagerSingleton<T>
    {
        private static T m_Singleton;
        public static T Singleton
        {
            get
            {
                return m_Singleton;
            }
            set
            {
                if (m_Singleton != null)
                    Destroy(m_Singleton.gameObject);

                m_Singleton = value;
            }
        }

        public override void Init()
        {
            Singleton = this as T;
        }
    }

    #endregion


    #region ScriptableObjects

    public abstract class ScriptableObjectBase: ScriptableObject
    {
        public abstract void Init();
    }
    public class ScriptableObjectSingleton<T> : ScriptableObjectBase where T : ScriptableObjectSingleton<T>
    {
        private static T m_Singleton;
        public static T Singleton
        {
            get
            {
                return m_Singleton;
            }
            set
            {
                if (m_Singleton == null)
                    m_Singleton = value;
            }
        }

        public override void Init()
        {
            Singleton = this as T;
        }
    }

    #endregion


    public class MainManager : ManagerSingleton<MainManager>
    {
        [SerializeField] private ManagerBase[] m_Managers;

        [SerializeField] private ScriptableObjectBase[] m_ScriptableObjectBase;


        private void Awake()
        {
            Init();

            foreach (ManagerBase subManager in m_Managers)
            {
                subManager.Init();
            }

            foreach (ScriptableObjectBase subSO in m_ScriptableObjectBase)
            {
                subSO.Init();
            }
        }
    }
}