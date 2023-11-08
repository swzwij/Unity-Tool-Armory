using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace swzwij.APIManager
{
    /// <summary>
    /// Manager class for handling API requests and responses.
    /// </summary>
    public class APIManager : MonoBehaviour
    {
        #region Singleton Behaviour

        /// <summary>
        /// Singleton instance of the APIManager.
        /// </summary>
        private static APIManager _instance;

        /// <summary>
        /// Accessor for the Singleton instance of the APIManager.
        /// If the instance does not exist, it creates one and ensures it persists across scenes.
        /// </summary>
        public static APIManager Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                _instance = FindObjectOfType<APIManager>();

                if (_instance != null)
                    return _instance;

                GameObject singletonObject = new(typeof(APIManager).Name); ;
                _instance = singletonObject.AddComponent<APIManager>();
                DontDestroyOnLoad(singletonObject);

                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// Sends a GET request to the specified API endpoint.
        /// </summary>
        /// <typeparam name="T">Type of the expected response data.</typeparam>
        /// <param name="request">API request configuration.</param>
        /// <param name="onComplete">Callback invoked upon a successful response.</param>
        /// <param name="onFailure">Callback invoked when the request fails or encounters an error.</param>
        public void GetCall<T>(APIRequest request, Action<T> onComplete = null, Action<APIStatus> onFailure = null) =>
            StartCoroutine(WebRequest(request, onComplete, onFailure));

        /// <summary>
        /// Coroutine for sending a web request and handling the response.
        /// </summary>
        /// <typeparam name="T">Type of the expected response data.</typeparam>
        /// <param name="request">API request configuration.</param>
        /// <param name="onComplete">Callback invoked upon a successful response.</param>
        /// <param name="onFailure">Callback invoked when the request fails or encounters an error.</param>
        private IEnumerator WebRequest<T>(APIRequest request, Action<T> onComplete, Action<APIStatus> onFailure)
        {
            UnityWebRequest webRequest = new(request.URL)
            {
                downloadHandler = new DownloadHandlerBuffer()
            };

            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                onFailure?.Invoke(new(webRequest));
                yield break;
            }

            T response = JsonUtility.FromJson<T>(webRequest.downloadHandler.text);
            onComplete?.Invoke(response);
        }
    }
}