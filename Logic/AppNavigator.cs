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
        [SerializeField]
        [ReadOnly]
        private AppSettings _appSettings;
        
        private AppSettings appSettings
        {
            get
            {
                if (_appSettings == null) {
                    _appSettings = Utils.GetAppSettings();
                }

                return _appSettings;
            }
            set => _appSettings = value;
        }
        
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

        [ShowInInspector]
        private Dictionary<string, AsyncOperationHandle<SceneInstance>> _addressableSceneInstances = new Dictionary<string, AsyncOperationHandle<SceneInstance>>();

        private Dictionary<string, AsyncOperationHandle<SceneInstance>> addressableSceneInstances
        {
            get => _addressableSceneInstances;
            set => _addressableSceneInstances = value;
        }

        // The first scene will always be a single load, done immediately w/o a call to
        // make a fade out first, so we have a special case for it here
        public void LoadInitialScene(ComplexPayload complexPayload) {
            sceneName = complexPayload.GetStringValue(DataType.stringType);
            activeScene.GetVariable(this.gameObject).SetValue(sceneName);
            StartCoroutine(AsyncLoad(sceneName, LoadSceneMode.Single));
        }
        

        // Otherwise, single scene loads necessitate doing a fade out before doing the load,
        // and additive scenes are loaded immediately without a call to the fader
        public void TriggerSceneLoad(ComplexPayload complexPayload) {
            
            sceneName = complexPayload.GetStringValue(DataType.stringType);
            activeScene.GetVariable(this.gameObject).SetValue(sceneName);
            loadMode = complexPayload.GetBoolValue(DataType.boolType) == true ? LoadSceneMode.Additive : LoadSceneMode.Single;

            if(loadMode == LoadSceneMode.Single) {
                if (appSettings.useAddressables == true) {
                    addressableSceneInstances.Clear();
                }
                fadeOutTriggered.RaiseEvent(this.gameObject);
            } else {
                StartCoroutine(AsyncLoad(sceneName, loadMode));
            }
        }

        public void FadeOutSceneLoadCallback()
        {
            StartCoroutine(AsyncLoad(sceneName, loadMode));
        }

        private IEnumerator AsyncLoad (string xSceneName, LoadSceneMode xLoadMode)
        {
            if (appSettings.useAddressables == true) {
                
                AsyncOperationHandle asyncLoad = Addressables.LoadSceneAsync(xSceneName, xLoadMode);
                asyncLoad.Completed += AddressablesSceneLoadCallback;

                // If we're in single load mode, scenes are unloaded automatically.
                // If additive, we need to store a reference to the scene to unload it later.
                if (xLoadMode == LoadSceneMode.Additive) {
                    addressableSceneInstances.Add(xSceneName, asyncLoad.Convert<SceneInstance>());
                }
                
                while (!asyncLoad.IsDone) {
                    yield return null;
                }
            }
            else {
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(xSceneName, xLoadMode);
                asyncLoad.completed += SceneLoadCallback;
            
                while (!asyncLoad.isDone) {
                    yield return null;
                }
            }
        }

        private void AddressablesSceneLoadCallback(AsyncOperationHandle asyncOperation)
        {
            sceneLoadCompleted.RaiseEvent(this.gameObject);
        }

        private void SceneLoadCallback(AsyncOperation asyncOperation)
        {
            sceneLoadCompleted.RaiseEvent(this.gameObject);
        }

        public void TriggerUnloadScene(ComplexPayload complexPayload)
        {
            string unloadSceneName = complexPayload.GetStringValue(DataType.stringType);
            StartCoroutine(AsyncUnload(unloadSceneName));
        }

        private IEnumerator AsyncUnload(string xSceneName)
        {
            if (appSettings.useAddressables == true) {
                AsyncOperationHandle<SceneInstance> unloadInstance = addressableSceneInstances[xSceneName];

                if (addressableSceneInstances.ContainsKey(xSceneName) == true) {
                    addressableSceneInstances.Remove(xSceneName);
                }

                AsyncOperationHandle asyncLoad = Addressables.UnloadSceneAsync(unloadInstance);

                while (!asyncLoad.IsDone) {
                    yield return null;
                }
            }
            else {
                AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(xSceneName);
                while (!asyncLoad.isDone) {
                    yield return null;
                }
            }
        }

        public void FadeOutOverlay(string parameter) {
            Debug.Log (parameter);
            UIManager.HideUiElement ("Fader", "Utils");
        }

        public void UpdateTimescale()
        {
            Time.timeScale = appSettings.GetTimescale(this.gameObject);
        }

        private static bool IsPopulated(StringReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}
