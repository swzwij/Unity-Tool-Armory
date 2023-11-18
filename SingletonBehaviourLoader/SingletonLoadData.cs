using System;
using UnityEngine;

namespace swzwij.Singletons
{
    /// <summary>
    /// ScriptableObject holding data for initializing and managing singletons.
    /// </summary>
    [CreateAssetMenu(fileName = "SingletonLoadData", menuName = "SingletonBehaviour/SingletonLoadData")]
    public class SingletonLoadData : ScriptableObject
    {
        /// <summary>
        /// Data structure containing information about singletons and their associated scenes.
        /// </summary>
        [SerializeField]
        private Data[] _loadDatas;

        /// <summary>
        /// Array of Data structures representing loaded singleton data.
        /// </summary>
        public Data[] LoadDatas => _loadDatas;

        /// <summary>
        /// Struct defining the data for a singleton and its associated scene.
        /// </summary>
        [Serializable]
        public struct Data
        {
            /// <summary>
            /// Reference to the GameObject singleton prefab.
            /// </summary>
            [SerializeField]
            private GameObject _singleton;

            /// <summary>
            /// Name of the scene where the singleton should be instantiated.
            /// </summary>
            [SerializeField]
            private string _sceneName;

            /// <summary>
            /// Gets the name of the scene for this data entry.
            /// </summary>
            public readonly string SceneName => _sceneName;

            /// <summary>
            /// Gets the GameObject singleton associated with this data entry.
            /// </summary>
            public readonly GameObject Singleton => _singleton;
        }
    }
}
