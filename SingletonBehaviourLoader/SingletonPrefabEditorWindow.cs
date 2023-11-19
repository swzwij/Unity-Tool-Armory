using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace swzwij.Singletons.Editor
{
    public class SingletonPrefabEditorWindow : EditorWindow
    {
        private string _singletonLoadDataPath = "Assets/Resources/Singletons";
        private SingletonLoadData _singletonLoadDataObject;
        private string[] _prefabNames;
        private Dictionary<string, SceneAsset> _prefabSceneDictionary = new Dictionary<string, SceneAsset>();
        private Vector2 _scrollPosition = Vector2.zero;

        [MenuItem("Window/Singleton Behaviour Loader Editor")]
        public static void ShowWindow() => GetWindow<SingletonPrefabEditorWindow>("Singleton Loader Editor");

        private void OnGUI()
        {
            GUILayout.Label("Singleton Load Data Settings", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Singleton Load Data Path:", GUILayout.Width(150));
                _singletonLoadDataPath = EditorGUILayout.TextField(_singletonLoadDataPath);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Singleton Load Data Object:", GUILayout.Width(180));
                _singletonLoadDataObject = (SingletonLoadData)EditorGUILayout.ObjectField(_singletonLoadDataObject, typeof(SingletonLoadData), false);
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Create New Singleton Load Data"))
                CreateNewSingletonLoadData();

            GUILayout.Space(10);

            GUILayout.Label("Prefab List", EditorStyles.boldLabel);

            GetPrefabNames();

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

            foreach (string prefabName in _prefabNames)
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(prefabName, GUILayout.Width(150));

                    _prefabSceneDictionary.TryGetValue(prefabName, out SceneAsset sceneAsset);
                    sceneAsset = (SceneAsset)EditorGUILayout.ObjectField(sceneAsset, typeof(SceneAsset), false);

                    if (sceneAsset != null)
                        _prefabSceneDictionary[prefabName] = sceneAsset;

                    if (GUILayout.Button("Set Scene"))
                        SetSceneForPrefab(prefabName);

                    if (GUILayout.Button("Remove Scene"))
                        RemoveSceneForPrefab(prefabName);
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        private void GetPrefabNames()
        {
            string[] prefabPaths = Directory.GetFiles(_singletonLoadDataPath, "*.prefab");
            List<string> names = new();

            foreach (string path in prefabPaths)
            {
                string prefabName = Path.GetFileNameWithoutExtension(path);
                names.Add(prefabName);

                if (!_prefabSceneDictionary.ContainsKey(prefabName))
                    _prefabSceneDictionary.Add(prefabName, null);
            }

            _prefabNames = names.ToArray();
        }

        private void CreateNewSingletonLoadData()
        {
            _singletonLoadDataObject = CreateInstance<SingletonLoadData>();
            AssetDatabase.CreateAsset(_singletonLoadDataObject, $"{_singletonLoadDataPath}/NewSingletonLoadData.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

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