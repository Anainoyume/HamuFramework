using UnityEngine;

namespace Utils
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance;
        private static readonly object _lock = new();

        public static T Instance {
            get {
                // 这里是粗粒度判空, 不执行多余逻辑, 防止 lock 影响性能
                if (_instance == null) {
                    InitializeInstance();
                }
                return _instance;
            }
        }
        
        private static void InitializeInstance() {
            lock (_lock) {
                // 双检锁, 以免其他锁唤醒后重新初始化一遍单例
                if (_instance != null) return;
                
                _instance = FindAnyObjectByType<T>();
                if (_instance == null) {
                    var go = new GameObject(typeof(T).Name);
                    _instance = go.AddComponent<T>();
                }
                DontDestroyOnLoad(_instance.gameObject);
                _instance.OnInitialize();
            }
        }

        // 如果没有在 Awake 之前提前调用 Getter, 走正常初始化流程
        private void Awake() {
            if (_instance == null) {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
                OnInitialize();
            }
            else if (_instance != this) {
                Destroy(gameObject);
            }
        }

        private void OnDestroy() {
            if (_instance != this) return;
            OnBeforeDestroy();
            _instance = null;
        }

        private void OnApplicationQuit() {
            if (_instance != this) return;
            OnBeforeApplicationQuit();
            _instance = null;
        }
        
        /// <summary>
        /// 在单例初始化完成之后调用
        /// </summary>
        protected virtual void OnInitialize() {}
        
        /// <summary>
        /// 在单例 OnDestroy() 时, instance 还未被置空之前调用
        /// </summary>
        protected virtual void OnBeforeDestroy() {}
        
        /// <summary>
        /// 在单例 OnApplicationQuit() 时, instance 还未被置空之前调用
        /// </summary>
        protected virtual void OnBeforeApplicationQuit() {}
    }
}