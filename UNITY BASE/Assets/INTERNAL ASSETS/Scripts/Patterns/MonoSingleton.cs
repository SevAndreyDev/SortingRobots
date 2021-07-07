using UnityEngine;

namespace EnglishKids.SortingRobots
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance = null;

        //==================================================
        // Fields
        //==================================================

        private bool _wasInitiated = false;

        //==================================================
        // Properties
        //==================================================

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(T)) as T;

                    if (_instance == null)
                    {
                        _instance = new GameObject(typeof(T).ToString()).AddComponent<T>();

                        if (_instance == null)
                            Debug.LogError(string.Format("Problem during the creation of {0}", typeof(T).ToString()));
                        else
                            _instance.Initialize();
                    }
                    else
                        _instance.Initialize();
                }
                                
                return _instance;
            }
        }

        //==================================================
        // Methods
        //==================================================

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                Initialize();
            }
        }

        private void Initialize()
        {
            if (!_wasInitiated)
            {
                Init();
                _wasInitiated = true;
            }
        }

        protected virtual void Init() { }
    }
}