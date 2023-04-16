using UnityEngine;


namespace MultiplayerGamePrototype.Core
{
    public abstract class ManagerBase : MonoBehaviour
    {
        public abstract void Init();
    }


    public class ManagerSingleton<T> : ManagerBase where T : ManagerSingleton<T>
    {
        private static T m_Instance;
        public static T Instance
        {
            get
            {
                return m_Instance;
            }
            set
            {
                if (m_Instance != null)
                    Destroy(m_Instance.gameObject);

                m_Instance = value;
            }
        }

        public override void Init()
        {
            Instance = this as T;
        }
    }


    public class MainManager : ManagerSingleton<MainManager>
    {
        [SerializeField] private ManagerBase[] m_Managers;


        private void Awake()
        {
            Init();

            foreach (ManagerBase subManager in m_Managers)
            {
                subManager.Init();
            }
        }
    }
}