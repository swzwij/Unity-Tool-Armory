using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace swzwij.Singletons.Editor
{
    /// <summary>
    /// Editor window for managing Singleton Load Data and associating scenes with prefabs.
    /// </summary>
    public class SingletonPrefabEditorWindow : EditorWindow
    {
        /// <summary>
        /// Path where Singleton Load Data is stored.
        /// </summary>
        private string _singletonLoadDataPath = "Assets/Resources/Singletons";

        /// <summary>
        /// The Singleton Load Data object being used.
        /// </summary>
        private SingletonLoadData _singletonLoadDataObject;

        /// <summary>
        /// Array storing names of prefabs.
        /// </summary>
        private string[] _prefabNames;

        /// <summary>
        /// Dictionary mapping prefab names to SceneAssets.
        /// </summary>
        private Dictionary<string, SceneAsset> _prefabSceneDictionary = new();

        /// <summary>
        /// Scroll position for the window.
        /// </summary>
        private Vector2 _scrollPosition = Vector2.zero;

        /// <summary>
        /// Displays the window in Unity's menu bar under 'Window'.
        /// </summary>
        [MenuItem("Window/Singleton Behaviour Loader Editor")]
        public static void ShowWindow() => GetWindow<SingletonPrefabEditorWindow>("Singleton Loader Editor");

        /// <summary>
        /// Displays the editor window GUI.
        /// </summary>
        private void OnGUI()
        {
            DisplaySingletonLoadDataSettings();
            GUILayout.Space(10);
            DisplayPrefabList();
        }

        /// <summary>
        /// Displays settings for Singleton Load Data.
        /// </summary>
        private void DisplaySingletonLoadDataSettings()
        {
            GUILayout.Label("Singleton Load Data Settings", EditorStyles.boldLabel);

            _singletonLoadDataPath = EditorGUILayout.TextField("Singleton Load Data Path:", _singletonLoadDataPath);
            _singletonLoadDataObject = (SingletonLoadData)EditorGUILayout.ObjectField("Singleton Load Data Object:", _singletonLoadDataObject, typeof(SingletonLoadData), false);

            if (GUILayout.Button("Create New Singleton Load Data"))
                CreateNewSingletonLoadData();
        }

        /// <summary>
        /// Displays the list of prefabs with associated scenes.
        /// </summary>
        private void DisplayPrefabList()
        {
            GUILayout.Label("Prefab List", EditorStyles.boldLabel);

            UpdatePrefabNames();
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

            foreach (string prefabName in _prefabNames)
            {
                DisplayPrefabEntry(prefabName);
            }

            GUILayout.EndScrollView();
        }

        /// <summary>
        /// Retrieves names of prefabs in the specified path and initializes the prefab-scene dictionary.
        /// </summary>
        private void UpdatePrefabNames()
        {
            string[] prefabPaths = Directory.GetFiles(_singletonLoadDataPath, "*.prefab");
            List<string> names = new List<string>();

            foreach (string path in prefabPaths)
            {
                string prefabName = Path.GetFileNameWithoutExtension(path);
                names.Add(prefabName);

                if (!_prefabSceneDictionary.ContainsKey(prefabName))
                    _prefabSceneDictionary.Add(prefabName, null);
            }

            _prefabNames = names.ToArray();
        }

        /// <summary>
        /// Displays the entry for a specific prefab with scene association options.
        /// </summary>
        /// <param name="prefabName">Name of the prefab.</param>
        private void DisplayPrefabEntry(string prefabName)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(prefabName, GUILayout.Width(150));

            _prefabSceneDictionary.TryGetValue(prefabName, out SceneAsset sceneAsset);
            sceneAsset = (SceneAsset)EditorGUILayout.ObjectField(sceneAsset, typeof(SceneAsset), false);

            if (sceneAsset != null)
                _prefabSceneDictionary[prefabName] = sceneAsset;

            if (GUILayout.Button("Set Scene"))
                SetSceneForPrefab(prefabName);

            if (GUILayout.Button("Remove Scene"))
                RemoveSceneForPrefab(prefabName);

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Creates a new Singleton Load Data asset.
        /// </summary>
        private void CreateNewSingletonLoadData()
        {
            _singletonLoadDataObject = CreateInstance<SingletonLoadData>();
            AssetDatabase.CreateAsset(_singletonLoadDataObject, $"{_singletonLoadDataPath}/NewSingletonLoadData.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Sets a scene for a specified prefab.
        /// </summary>
        /// <param name="prefabName">Name of the prefab.</param>
        private void SetSceneForPrefab(string prefabName)
        {
            SceneAsset sceneAsset = _prefabSceneDictionary[prefabName];

            if (_singletonLoadDataObject != null)
            {
                GameObject prefabObject = Resources.Load<GameObject>($"Singletons/{prefabName}");

                if (sceneAsset != null)
                {
                    string scenePath = AssetDatabase.GetAssetPath(sceneAsset);
                    string sceneName = Path.GetFileNameWithoutExtension(scenePath);
                    _singletonLoadDataObject.SetSceneForSingleton(prefabObject, sceneName);
                }
                else
                {
                    _singletonLoadDataObject.SetSceneForSingleton(prefabObject, null);
                }

                EditorUtility.SetDirty(_singletonLoadDataObject);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogError("Singleton Load Data Object not assigned. Please assign a Singleton Load Data Object.");
            }
        }

        /// <summary>
        /// Removes scene association for a specified prefab.
        /// </summary>
        /// <param name="prefabName">Name of the prefab.</param>
        private void RemoveSceneForPrefab(string prefabName)
        {
            if (_singletonLoadDataObject != null)
            {
                GameObject prefabObject = Resources.Load<GameObject>($"Singletons/{prefabName}");
                _singletonLoadDataObject.SetSceneForSingleton(prefabObject, null);

                EditorUtility.SetDirty(_singletonLoadDataObject);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogError("Singleton Load Data Object not assigned. Please assign a Singleton Load Data Object.");
            }
        }
    }
}