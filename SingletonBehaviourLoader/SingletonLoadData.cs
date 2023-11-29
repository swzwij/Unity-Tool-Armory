using System;
using System.Collections.Generic;
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
        private List<Data> _loadDatas = new();

        /// <summary>
        /// Array of Data structures representing loaded singleton data.
        /// </summary>
        public List<Data> LoadDatas => _loadDatas;

        public void SetSceneForSingleton(GameObject singleton, string sceneName)
        {
            for (int i = 0; i < _loadDatas.Count; i++)
            {
                if (_loadDatas[i].Singleton == singleton)
                {
                    _loadDatas[i] = new Data(_loadDatas[i].Singleton, sceneName);
                    return;
                }
            }

            _loadDatas.Add(new Data(singleton, sceneName));
        }

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

            public Data(GameObject singleton, string sceneName)
            {
                _singleton = singleton;
                _sceneName = sceneName;
            }

            /// <summary>
            /// Gets the name of the scene for this data entry.
            /// </summary>
            public string SceneName 
            { 
                get => _sceneName;
                set => _sceneName = value;
            }

            /// <summary>
            /// Gets the GameObject singleton associated with this data entry.
            /// </summary>
            public readonly GameObject Singleton => _singleton;
        }
    }
}
