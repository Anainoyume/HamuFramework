using UnityEngine;

namespace Utils
{
    public class Singleton<T> where T : Singleton<T>, new()
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
                _instance = new T();    // 在构造函数里初始化就行
            }
        }

    }
}