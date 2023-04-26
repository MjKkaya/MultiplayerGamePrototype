using Unity.Netcode;
using UnityEngine;

/*
	Generic classes for the use of singleton
	there are 3 types:
	- MonoBehaviour -> for the use of singleton to normal MonoBehaviours
	- NetworkBehaviour -> for the use of singleton that uses the NetworkBehaviours
	- Persistent -> when we need to make sure the object is not destroyed during the session
*/

namespace MultiplayerGamePrototype.Utilities
{
    public class SingletonMono<T> : MonoBehaviour where T : Component
    {
        public static T Singleton { get; protected set; }

        public virtual void Awake()
        {
            if (Singleton == null)
            {
                Singleton = this as T;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public class SingletonMonoPersistent<T> : MonoBehaviour where T : Component
    {
        public static T Singleton { get; private set; }

        public virtual void Awake()
        {
            if (Singleton == null)
            {
                Singleton = this as T;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public class SingletonNetwork<T> : NetworkBehaviour where T : Component
    {
        public static T Singleton { get; private set; }

        public virtual void Awake()
        {
            if (Singleton == null)
            {
                Singleton = this as T;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public class SingletonNetworkPersistent<T> : NetworkBehaviour where T : Component
    {
        public static T Singleton { get; private set; }

        public virtual void Awake()
        {
            if (Singleton == null)
            {
                Singleton = this as T;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}