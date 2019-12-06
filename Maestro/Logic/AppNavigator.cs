/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DoozyUI;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace AltSalt.Maestro.Logic
{
    public class AppNavigator : MonoBehaviour {
        
        // Scene transition variables
        [Required]
        public AppSettings appSettings;
        
        [ValidateInput("IsPopulated")]
        public StringReference activeScene;

        [SerializeField]
        [Required]
        SimpleEventTrigger fadeOutTriggered;

        [SerializeField]
        [Required]
        SimpleEventTrigger sceneLoadCompleted;

        [SerializeField]
        [Required]
        SimpleEventTrigger timescaleChanged;

        private string _sceneName;

        private string sceneName
        {
            get => _sceneName;
            set => _sceneName = value;
        }

        LoadSceneMode loadMode;

        Dictionary<string, AsyncOperationHandle<SceneInstance>> sceneInstances = new Dictionary<string, AsyncOperationHandle<SceneInstance>>();

        // The first scene will always be a single load, done immediately w/o a call to
        // make a fade out first, so we have a special case for it here
        public void LoadInitialScene(EventPayload eventPayload) {
            sceneName = eventPayload.GetStringValue(DataType.stringType);
            activeScene.Variable.SetValue(sceneName);
            StartCoroutine(AsyncLoad(sceneName, LoadSceneMode.Single));
        }
        

        // Otherwise, single scene loads necessitate doing a fade out before doing the load,
        // and additive scenes are loaded immediately without a call to the fader
        public void TriggerSceneLoad(EventPayload eventPayload) {
            
            sceneName = eventPayload.GetStringValue(DataType.stringType);
            activeScene.Variable.SetValue(sceneName);
            loadMode = eventPayload.GetBoolValue(DataType.boolType) == true ? LoadSceneMode.Additive : LoadSceneMode.Single;

            if(loadMode == LoadSceneMode.Single) {
                fadeOutTriggered.RaiseEvent(this.gameObject);
            } else {
                StartCoroutine(AsyncLoad(sceneName, loadMode));
            }
        }

        public void FadeOutSceneLoadCallback()
        {
            StartCoroutine(AsyncLoad(sceneName, loadMode));
        }
        
        IEnumerator AsyncLoad (string xSceneName, LoadSceneMode xLoadMode)
        {
            //AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(xSceneName, xLoadMode);

            AsyncOperationHandle asyncLoad = Addressables.LoadSceneAsync(xSceneName, xLoadMode);

            asyncLoad.Completed += SceneLoadCallback;

            // If we're in single load mode, scenes are unloaded automatically.
            // If additive, we need to store a reference to the scene to unload it later.
            if (xLoadMode == LoadSceneMode.Additive) {
                sceneInstances.Add(xSceneName, asyncLoad.Convert<SceneInstance>());
            }
            
            while (!asyncLoad.IsDone) {
                yield return null;
            }
        }
        
        void SceneLoadCallback(AsyncOperationHandle asyncOperation)
        {
            sceneLoadCompleted.RaiseEvent(this.gameObject);
        }

        public void TriggerUnloadScene(EventPayload eventPayload)
        {
            string unloadSceneName = eventPayload.GetStringValue(DataType.stringType);
            StartCoroutine(AsyncUnload(unloadSceneName));
        }

        IEnumerator AsyncUnload(string xSceneName)
        {
            //AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(xSceneName);

            AsyncOperationHandle<SceneInstance> unloadInstance = sceneInstances[xSceneName];

            if (sceneInstances.ContainsKey(xSceneName)) {
                sceneInstances.Remove(xSceneName);
            }

            AsyncOperationHandle asyncLoad = Addressables.UnloadSceneAsync(unloadInstance);

            while (!asyncLoad.IsDone) {
                yield return null;
            }
        }

        public void FadeOutOverlay(string parameter) {
            Debug.Log (parameter);
            UIManager.HideUiElement ("Fader", "Utils");
        }

        public void UpdateTimescale()
        {
            Time.timeScale = appSettings.timescale;
        }

        private static bool IsPopulated(StringReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}
