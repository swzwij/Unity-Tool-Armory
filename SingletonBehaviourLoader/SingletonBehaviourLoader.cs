using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace swzwij.Singletons
{
    /// <summary>
    /// Handles the initialization and management of singleton objects in different scenes.
    /// </summary>
    public static class SingletonBehaviourLoader
    {
        private const string SINGLETON_PREFAB_FOLDER = "Singletons";

        /// <summary>
        /// The parent object to hold all instantiated singletons in the DontDestroyOnLoad.
        /// </summary>
        private static GameObject _singletonParentObject;

        /// <summary>
        /// Tracks if general singletons have been initialized.
        /// </summary>
        private static bool _hasInitializedSingletons;

        /// <summary>
        /// Holds all general singletons to persist across scenes.
        /// </summary>
        private static readonly List<GameObject> _generalSingletons = new();

        /// <summary>
        /// Holds scene specific singletons for each loaded scene.
        /// </summary>
        private static readonly Dictionary<string, List<GameObject>> _sceneSingletons = new();

        /// <summary>
        /// Initializes the singletons and their data during subsystem registration.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            InitializeLoadData();
        }

        /// <summary>
        /// Loads scene specific singletons on scene load and initializes general singletons if not initialized.
        /// </summary>
        /// <param name="scene">The loaded scene.</param>
        /// <param name="mode">The loading mode of the scene.</param>
        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!_hasInitializedSingletons)
            {
                _singletonParentObject = InstantiateSingletonParent();
                LoadGeneralSingletons();
                _hasInitializedSingletons = true;
            }

            LoadSceneSingletons(scene);
        }

        /// <summary>
        /// Loads all the general singletons.
        /// </summary>
        private static void LoadGeneralSingletons()
        {
            foreach (GameObject singleton in _generalSingletons)
                CreateSingletonInstance(singleton, true);
        }

        /// <summary>
        /// Loads scene specific singletons.
        /// </summary>
        /// <param name="scene">The scene to load the singletons into.</param>
        private static void LoadSceneSingletons(Scene scene)
        {
            if (!_sceneSingletons.ContainsKey(scene.name))
                return;

            foreach (GameObject singleton in _sceneSingletons[scene.name])
                CreateSingletonInstance(singleton, false);
        }

        /// <summary>
        /// Instantiates the singleton parent GameObject and adds it into the DontDestroyOnLoad.
        /// </summary>
        /// <returns>The instantiated singleton parent GameObject.</returns>
        private static GameObject InstantiateSingletonParent()
        {
            const string SINGLETON_PARENT_NAME = "Singleton Instances";

            GameObject singletonParent = Object.Instantiate(new GameObject(SINGLETON_PARENT_NAME));
            singletonParent.name = SINGLETON_PARENT_NAME;
            Object.DontDestroyOnLoad(singletonParent);

            return singletonParent;
        }

        /// <summary>
        /// Creates a singleton instance GameObject.
        /// </summary>
        /// <param name="singleton">The GameObject to instantiate as a singleton.</param>
        /// <param name="dontDestroyOnLoad">Determines if the GameObject should persist across scenes.</param>
        /// <returns>The created singleton instance GameObject.</returns>
        private static GameObject CreateSingletonInstance(GameObject singleton, bool dontDestroyOnLoad)
        {
            GameObject singletonInstance = Object.Instantiate(singleton);
            singletonInstance.name = singleton.name;

            if (!dontDestroyOnLoad)
                return singletonInstance;

            Object.DontDestroyOnLoad(singletonInstance);
            singletonInstance.transform.parent = _singletonParentObject.transform;

            return singletonInstance;
        }

        /// <summary>
        /// Initializes singleton data from the ScriptableObject and the folder of prefabs.
        /// </summary>
        private static void InitializeLoadData()
        {
            SingletonLoadData _singletonLoadData = RetrieveSingletonLoadData(SINGLETON_PREFAB_FOLDER);
            List<SingletonLoadData.Data> loadData = _singletonLoadData.LoadDatas;
            GameObject[] singletons = RetrieveSingletonsFromFolder(SINGLETON_PREFAB_FOLDER);

            foreach (GameObject singleton in singletons)
                _generalSingletons.Add(singleton);

            foreach (SingletonLoadData.Data data in loadData)
            {
                if (string.IsNullOrEmpty(data.SceneName))
                    continue;

                if (!_sceneSingletons.ContainsKey(data.SceneName))
                    _sceneSingletons.Add(data.SceneName, new());

                _sceneSingletons[data.SceneName].Add(data.Singleton);
                _generalSingletons.Remove(data.Singleton);
            }
        }

        /// <summary>
        /// Retrieves all singletons from a folder path.
        /// </summary>
        /// <param name="folderPath">The path of the folder containing the singletons.</param>
        /// <returns>An array of GameObjects representing singletons in the specified folder.</returns>
        private static GameObject[] RetrieveSingletonsFromFolder(string folderPath)
        {
            string[] prefabNames = GetPrefabNamesInFolder(folderPath);

            GameObject[] singletons = new GameObject[prefabNames.Length];

            for (int i = 0; i < prefabNames.Length; i++)
                singletons[i] = Resources.Load<GameObject>(folderPath + "/" + prefabNames[i]);

            return singletons;
        }

        /// <summary>
        /// Retrieves the ScriptableObject containing singleton load data.
        /// </summary>
        /// <param name="folderPath">The path of the folder containing the ScriptableObject.</param>
        /// <returns>The SingletonLoadData ScriptableObject.</returns>
        private static SingletonLoadData RetrieveSingletonLoadData(string folderPath)
        {
            string[] prefabPaths = Directory.GetFiles("Assets/Resources/" + folderPath, "*.asset");
            string prefabName = Path.GetFileNameWithoutExtension(prefabPaths[0]);
            SingletonLoadData singletonLoadData = Resources.Load<SingletonLoadData>(folderPath + "/" + prefabName);
            return singletonLoadData;
        }

        /// <summary>
        /// Retrieves prefab names from a specified folder path.
        /// </summary>
        /// <param name="folderPath">The path of the folder containing the prefabs.</param>
        /// <returns>An array of strings representing prefab names in the specified folder.</returns>
        private static string[] GetPrefabNamesInFolder(string folderPath)
        {
            string[] prefabPaths = Directory.GetFiles("Assets/Resources/" + folderPath, "*.prefab");

            List<string> prefabNames = new();

            foreach (string path in prefabPaths)
            {
                string prefabName = Path.GetFileNameWithoutExtension(path);
                prefabNames.Add(prefabName);
            }

            return prefabNames.ToArray();
        }
    }
}