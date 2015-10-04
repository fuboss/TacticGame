using System.Linq;
using UnityEngine;

namespace Assets.Code.Tools
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance
        {
            get
            {
                if (_istance != null) return _istance;

                var allInstances = FindObjectsOfType<T>();
                if (allInstances.Length == 1)
                    _istance = allInstances.First();
                else 
                    Debug.LogError(string.Format("There are {0} instances of "+ typeof(T).Name + " in the scene", allInstances.Length > 1? "to many": "no any"));
                return _istance;
            }
        }

        protected virtual void Awake()
        {
            Init();
        }

        protected virtual void Init()
        {
            
        }

        private static T _istance;
    }
}
