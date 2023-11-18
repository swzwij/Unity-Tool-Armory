using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace swzwij.Singletons
{
    /// <summary>
    /// Handles initialization and instantiation of singleton prefabs in the scene.
    /// </summary>
    public static class SingletonBehaviourLoader
    {
        /// <summary>
        /// The folder path where singleton prefabs are located.
        /// </summary>
        private static string _singletonPrefabFolder = "Assets/Singletons";

        /// <summary>
        /// The parent object for instantiated singleton prefabs.
        /// </summary>
        private static GameObject _singletonParentObject;

        /// <summary>
        /// Gets or sets the folder path where singleton prefabs are located.
        /// </summary>
        public static string SingletonPrefabFolder
        {
            get => _singletonPrefabFolder;
            set => _singletonPrefabFolder = value;
        }

        /// <summary>
        /// Initializes the SingletonPrefabInitiator on subsystem registration.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize() => SceneManager.sceneLoaded += OnSceneLoad;

        /// <summary>
        /// Handles actions when a scene is loaded.
        /// </summary>
        /// <param name="scene">The loaded scene.</param>
        /// <param name="mode">The mode of scene loading.</param>
        private static void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"{scene.name} Loaded");
            LoadSingletons();
            SceneManager.sceneLoaded -= OnSceneLoad;
        }

        /// <summary>
        /// Loads and instantiates singleton prefabs.
        /// </summary>
        private static void LoadSingletons()
        {
            _singletonParentObject = InstantiateSingletonParent();

            GameObject[] singletons = RetrieveSingletonsFromFolder(_singletonPrefabFolder);

            foreach (GameObject singleton in singletons)
                InstantiateSingleton(singleton);
        }

        /// <summary>
        /// Instantiates a parent GameObject for singleton instances.
        /// </summary>
        /// <returns>The instantiated parent GameObject.</returns>
        private static GameObject InstantiateSingletonParent()
        {
            const string SINGLETON_PARENT_NAME = "Singleton Instances";

            GameObject singletonParent = Object.Instantiate(new GameObject(SINGLETON_PARENT_NAME));
            singletonParent.name = SINGLETON_PARENT_NAME;
            Object.DontDestroyOnLoad(singletonParent);

            return singletonParent;
        }

        /// <summary>
        /// Instantiates a singleton prefab and sets it under the singleton parent object.
        /// </summary>
        /// <param name="singleton">The singleton prefab to instantiate.</param>
        private static void InstantiateSingleton(GameObject singleton)
        {
            GameObject singletonInstance = Object.Instantiate(singleton);
            singletonInstance.name = singleton.name;
            Object.DontDestroyOnLoad(singletonInstance);
            singletonInstance.transform.parent = _singletonParentObject.transform;
        }

        /// <summary>
        /// Retrieves singleton prefabs from a specified folder path.
        /// </summary>
        /// <param name="folderPath">The folder path to retrieve singleton prefabs from.</param>
        /// <returns>An array of retrieved singleton prefabs.</returns>
        private static GameObject[] RetrieveSingletonsFromFolder(string folderPath)
        {
            string[] singletonPaths = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });
            GameObject[] singletons = new GameObject[singletonPaths.Length];

            for (int i = 0; i < singletonPaths.Length; i++)
            {
                string singletonPath = AssetDatabase.GUIDToAssetPath(singletonPaths[i]);
                singletons[i] = AssetDatabase.LoadAssetAtPath<GameObject>(singletonPath);
            }

            return singletons;
        }
    }
}