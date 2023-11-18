using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace swzwij.Singletons
{
    public static class SingletonBehaviourLoader
    {
        private static string _singletonPrefabFolder = "Singletons";

        private static GameObject _singletonParentObject;

        private static SingletonLoadData _singletonLoadData;

        private static bool _hasInitialized;

        private static Dictionary<GameObject, SingletonLoadData.LoadData> _loadDatas = new();

        public static string SingletonPrefabFolder
        {
            get => _singletonPrefabFolder;
            set => _singletonPrefabFolder = value;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            SceneManager.sceneLoaded += OnSceneLoad;
            InitializeLoadData();
        }

        private static void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"{scene.name} Loaded");
            LoadSingletons(scene);
            _hasInitialized = true;
        }

        private static void LoadSingletons(Scene scene)
        {
            if(!_hasInitialized)
                _singletonParentObject = InstantiateSingletonParent();

            GameObject[] singletons = RetrieveSingletonsFromFolder(_singletonPrefabFolder);

            foreach (GameObject singleton in singletons)
                InstantiateSingleton(singleton, scene);
        }

        private static GameObject InstantiateSingletonParent()
        {
            const string SINGLETON_PARENT_NAME = "Singleton Instances";

            Debug.Log("Created parent");

            GameObject singletonParent = Object.Instantiate(new GameObject(SINGLETON_PARENT_NAME));
            singletonParent.name = SINGLETON_PARENT_NAME;
            Object.DontDestroyOnLoad(singletonParent);

            return singletonParent;
        }

        private static void InstantiateSingleton(GameObject singleton, Scene scene)
        {
            if(!_loadDatas.ContainsKey(singleton) && !_hasInitialized)
            {
                CreateSingletonInstance(singleton, true);
                return;
            }

            if (!_loadDatas.ContainsKey(singleton))
                return;

            if(_loadDatas[singleton].SceneName == string.Empty && !_hasInitialized)
            {
                CreateSingletonInstance(singleton, true);
                return;
            }

            if (_loadDatas[singleton].SceneName == scene.name)
            {
                CreateSingletonInstance(singleton, false);
                return;
            }
        }

        private static void CreateSingletonInstance(GameObject singleton, bool dontDetroyOnLoad)
        {
            GameObject singletonInstance = Object.Instantiate(singleton);
            singletonInstance.name = singleton.name;

            if (!dontDetroyOnLoad)
                return;

            Object.DontDestroyOnLoad(singletonInstance);
            singletonInstance.transform.parent = _singletonParentObject.transform;
        }

        private static GameObject[] RetrieveSingletonsFromFolder(string folderPath)
        {
            string[] prefabNames = GetPrefabNamesInFolder(folderPath);

            GameObject[] singletons = new GameObject[prefabNames.Length];

            for (int i = 0; i < prefabNames.Length; i++)
                singletons[i] = Resources.Load<GameObject>(folderPath + "/" + prefabNames[i]);

            return singletons;
        }

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

        private static SingletonLoadData RetrieveSingletonLoadData(string folderPath)
        {
            string[] prefabPaths = Directory.GetFiles("Assets/Resources/" + folderPath, "*.asset");
            string prefabName = Path.GetFileNameWithoutExtension(prefabPaths[0]);
            var singletonLoadData = Resources.Load<SingletonLoadData>(folderPath + "/" + prefabName);
            return singletonLoadData;
        }
            
        private static void InitializeLoadData()
        {
            _singletonLoadData = RetrieveSingletonLoadData(_singletonPrefabFolder);
            Debug.Log(_singletonLoadData);
            SingletonLoadData.LoadData[] singletonLoadDatas = _singletonLoadData.LoadDatas;

            foreach (var singletonData in singletonLoadDatas)
                _loadDatas.Add(singletonData.Singleton, singletonData);
        }
    }
}