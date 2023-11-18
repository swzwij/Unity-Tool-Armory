using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace swzwij.Singletons
{
    /// <summary>
    /// 
    /// </summary>
    public static class SingletonBehaviourLoader
    {
        private const string SINGLETON_PREFAB_FOLDER = "Singletons";

        /// <summary>
        /// The general singelton parent object.
        /// </summary>
        private static GameObject _singletonParentObject;

        /// <summary>
        /// Whether the general singeltons have been initialized.
        /// </summary>
        private static bool _hasInitializedSingletons;

        /// <summary>
        /// All the general singletons which go into the dontdestroyonload.
        /// </summary>
        private static readonly List<GameObject> _generalSingletons = new();

        /// <summary>
        /// All the scene specific singletons.
        /// </summary>
        private static readonly Dictionary<string, List<GameObject>> _sceneSingletons = new();

        /// <summary>
        /// Initialize all the sinletons and there data on subsystem registration.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            InitializeLoadData();
        }

        /// <summary>
        /// load scene specific singletons on scene load and initialize general singletons if not done alreadty.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if(!_hasInitializedSingletons)
            {
                _singletonParentObject = InstantiateSingletonParent();
                LoadGeneralSingletons();
                _hasInitializedSingletons = true;
            }

            LoadSceneSingletons(scene);
        }

        /// <summary>
        /// load all the general singletons.
        /// </summary>
        private static void LoadGeneralSingletons()
        {
            foreach (GameObject singleton in _generalSingletons)
                CreateSingletonInstance(singleton, true);
        }

        /// <summary>
        /// load scene specific sinegelons.
        /// </summary>
        /// <param name="scene"></param>
        private static void LoadSceneSingletons(Scene scene)
        {
            if (!_sceneSingletons.ContainsKey(scene.name))
                return;

            foreach (GameObject singleton in _sceneSingletons[scene.name])
                CreateSingletonInstance(singleton, false);
        }

        /// <summary>
        /// instantie singleton parent.
        /// </summary>
        /// <returns></returns>
        private static GameObject InstantiateSingletonParent()
        {
            const string SINGLETON_PARENT_NAME = "Singleton Instances";

            GameObject singletonParent = Object.Instantiate(new GameObject(SINGLETON_PARENT_NAME));
            singletonParent.name = SINGLETON_PARENT_NAME;
            Object.DontDestroyOnLoad(singletonParent);

            return singletonParent;
        }

        /// <summary>
        /// Create a singleton instance.
        /// </summary>
        /// <param name="singleton"></param>
        /// <param name="dontDetroyOnLoad"></param>
        /// <returns></returns>
        private static GameObject CreateSingletonInstance(GameObject singleton, bool dontDetroyOnLoad)
        {
            GameObject singletonInstance = Object.Instantiate(singleton);
            singletonInstance.name = singleton.name;

            if (!dontDetroyOnLoad)
                return singletonInstance;

            Object.DontDestroyOnLoad(singletonInstance);
            singletonInstance.transform.parent = _singletonParentObject.transform;

            return singletonInstance;
        }

        /// <summary>
        /// Initailzie all the singleton data from the sciprable object and the folder of prefabs.
        /// </summary>
        private static void InitializeLoadData()
        {
            SingletonLoadData _singletonLoadData = RetrieveSingletonLoadData(SINGLETON_PREFAB_FOLDER);
            SingletonLoadData.Data[] loadData = _singletonLoadData.LoadDatas;
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
        /// get all singletons from folder.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        private static GameObject[] RetrieveSingletonsFromFolder(string folderPath)
        {
            string[] prefabNames = GetPrefabNamesInFolder(folderPath);

            GameObject[] singletons = new GameObject[prefabNames.Length];

            for (int i = 0; i < prefabNames.Length; i++)
                singletons[i] = Resources.Load<GameObject>(folderPath + "/" + prefabNames[i]);

            return singletons;
        }

        /// <summary>
        /// get singleton load data sciptable object.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        private static SingletonLoadData RetrieveSingletonLoadData(string folderPath)
        {
            string[] prefabPaths = Directory.GetFiles("Assets/Resources/" + folderPath, "*.asset");
            string prefabName = Path.GetFileNameWithoutExtension(prefabPaths[0]);
            SingletonLoadData singletonLoadData = Resources.Load<SingletonLoadData>(folderPath + "/" + prefabName);
            return singletonLoadData;
        }

        /// <summary>
        /// get prefabs from a folder of the given path.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
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