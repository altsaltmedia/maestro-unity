/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace AltSalt.Maestro.Logic
{
    [ExecuteInEditMode]
    public class SceneController : MonoBehaviour {
        
        // Scene transition variables
        [Required]
        [SerializeField]
        [ReadOnly]
        private AppSettingsReference _appSettings = new AppSettingsReference();

        private AppSettings appSettings => _appSettings.GetVariable() as AppSettings;
        
        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private CustomKeyReference _loadSceneImmediatelyKey = new CustomKeyReference();

        private CustomKey loadSceneImmediatelyKey => _loadSceneImmediatelyKey.GetVariable() as CustomKey;
        
        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private CustomKeyReference _loadSceneAdditiveKey = new CustomKeyReference();

        private CustomKey loadSceneAdditiveKey => _loadSceneAdditiveKey.GetVariable() as CustomKey;
        
        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private ComplexEventManualTrigger _triggerFadeToBlack = new ComplexEventManualTrigger();

        private ComplexEventManualTrigger triggerFadeToBlack => _triggerFadeToBlack;
        
        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private SimpleEventReference _fadeToBlackCompleted = new SimpleEventReference();

        private SimpleEventReference fadeToBlackCompleted => _fadeToBlackCompleted;
        
        private float sceneLoadingProgress
        {
            set => appSettings.SetSceneLoadingProgress(this.gameObject, value);
        }

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private SimpleEventReference _singleSceneLoadCompleted = new SimpleEventReference();

        private SimpleEventReference singleSceneLoadCompleted => _singleSceneLoadCompleted;
        
        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private SimpleEventReference _additiveSceneLoadCompleted = new SimpleEventReference();

        private SimpleEventReference additiveSceneLoadCompleted => _additiveSceneLoadCompleted;

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private CustomKeyReference _eventCallbackKey = new CustomKeyReference();

        private CustomKey eventCallbackKey => _eventCallbackKey.GetVariable() as CustomKey;

        private SimpleEvent _sceneLoadCompletedCallback;

        private SimpleEvent sceneLoadCompletedCallback
        {
            get => _sceneLoadCompletedCallback;
            set => _sceneLoadCompletedCallback = value;
        }
        
        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private SimpleEventReference _sceneUnloadCompleted = new SimpleEventReference();

        private SimpleEventReference sceneUnloadCompleted => _sceneUnloadCompleted;

        private string _targetScene;

        private string targetScene
        {
            get => _targetScene;
            set => _targetScene = value;
        }

        private LoadSceneMode _loadMode;

        private LoadSceneMode loadMode
        {
            get => _loadMode;
            set => _loadMode = value;
        }

        [ShowInInspector]
        private Dictionary<string, AsyncOperationHandle<SceneInstance>> _addressableSceneInstances
            = new Dictionary<string, AsyncOperationHandle<SceneInstance>>();

        private Dictionary<string, AsyncOperationHandle<SceneInstance>> addressableSceneInstances 
            => _addressableSceneInstances;

        private void OnEnable()
        {
#if UNITY_EDITOR
            _appSettings.PopulateVariable(this, nameof(_appSettings));
            _triggerFadeToBlack.PopulateVariable(this, nameof(_triggerFadeToBlack));
            _fadeToBlackCompleted.PopulateVariable(this, nameof(_fadeToBlackCompleted));
            _loadSceneAdditiveKey.PopulateVariable(this, nameof(_loadSceneAdditiveKey));
            _singleSceneLoadCompleted.PopulateVariable(this, nameof(_singleSceneLoadCompleted));
            _additiveSceneLoadCompleted.PopulateVariable(this, nameof(_additiveSceneLoadCompleted));
            _eventCallbackKey.PopulateVariable(this, nameof(_eventCallbackKey));
#endif            
        }

        // Otherwise, single scene loads necessitate doing a fade out before doing the load,
        // and additive scenes are loaded immediately without a call to the fader
        public void TriggerSceneLoad(ComplexPayload complexPayload)
        {
            if (Application.isPlaying == false) return;

            sceneLoadingProgress = 0;
            
            this.targetScene = complexPayload.GetStringValue(DataType.stringType);;
            this.appSettings.SetActiveScene(this.gameObject,this.targetScene);

            bool loadSceneImmediately = complexPayload.GetBoolValue(loadSceneImmediatelyKey);

            if (loadSceneImmediately == true) {
                PerformSimpleSceneLoad(complexPayload);
                return;
            }
            
            bool loadSceneAdditive = complexPayload.GetBoolValue(loadSceneAdditiveKey);

            if (loadSceneAdditive == true) {
                PerformAdditiveSceneLoad(complexPayload);   
            }
            else {
                PerformComplexSceneLoad(complexPayload);
            }
            
        }

        private Coroutine PerformSimpleSceneLoad(ComplexPayload complexPayload)
        {
            this.loadMode = LoadSceneMode.Single;
            
            var eventCallback = complexPayload.GetScriptableObjectValue(this.eventCallbackKey) as SimpleEvent;
            if (eventCallback != null) {
                this.sceneLoadCompletedCallback = eventCallback;
            }
            else {
                this.sceneLoadCompletedCallback = singleSceneLoadCompleted.GetVariable() as SimpleEvent;
            }

            if (this.appSettings.useAddressables == true) {
                return StartCoroutine(AddressablesAsyncSceneLoad(this.targetScene, this.loadMode, AddressablesAsyncLoadCompleted));
            }

            return StartCoroutine(StandardAsyncSceneLoad(this.targetScene, this.loadMode, StandardAsyncLoadCompleted));
        }

        private Coroutine PerformAdditiveSceneLoad(ComplexPayload complexPayload)
        {
            this.loadMode = LoadSceneMode.Additive;
            
            var eventCallback = complexPayload.GetScriptableObjectValue(this.eventCallbackKey) as SimpleEvent;
            if (eventCallback != null) {
                this.sceneLoadCompletedCallback = eventCallback;
            }
            else {
                this.sceneLoadCompletedCallback = additiveSceneLoadCompleted.GetVariable() as SimpleEvent;
            }

            if (appSettings.useAddressables == true) {
                return StartCoroutine(AddressablesAsyncSceneLoad(targetScene, loadMode, AddressablesAsyncLoadCompleted));
            }

            return StartCoroutine(StandardAsyncSceneLoad(targetScene, loadMode, StandardAsyncLoadCompleted));
        }

        /// <summary>
        /// Work in conjunction with the fader to fade to black, perform a scene
        /// load, perform a fade in, and when the animation is complete, execute a
        /// simple event callback
        /// </summary>
        /// <param name="complexPayload"></param>
        private void PerformComplexSceneLoad(ComplexPayload complexPayload)
        {
            // Set our load mode for later use
            this.loadMode = LoadSceneMode.Single;
            
            // Store the callback so we can retrieve it once the fadeToBlack animation is complete
            // (Note: the callback may be null at this point - we'll check later when it's time to use it)
            this.sceneLoadCompletedCallback = complexPayload.GetScriptableObjectValue(eventCallbackKey) as SimpleEvent;
            
            // Modify the complex payload, inserting a callback that we can
            // use to respond when the fadeToBlack animation is complete
            complexPayload.Set(eventCallbackKey, fadeToBlackCompleted.GetVariable());

            // Pass on the rest of the payload parameters to the fader
            this.triggerFadeToBlack.RaiseEvent(this.gameObject, complexPayload);
        }

        /// <summary>
        /// Once the fade to black is completed, repopulate our
        /// complex payload with our final callback, perform the scene load,
        /// and once the scene load is complete, send a fade in request
        /// along with our complex payload parameters
        /// </summary>
        public void FadeToBlackCompletedCallback()
        {
            // Now that we're faded to black, determine which event
            // we'll send to the fader as our final callback
            SimpleEvent eventCallback;
            if (this.sceneLoadCompletedCallback != null) {
                eventCallback = this.sceneLoadCompletedCallback;
            }
            else {
                eventCallback = singleSceneLoadCompleted.GetVariable() as SimpleEvent;
            }

            // Trigger the scene load, followed by a fade in action
            // to be executed when the scene load is completed
            if (appSettings.useAddressables == true) {
                StartCoroutine(AddressablesAsyncSceneLoad(targetScene, loadMode, (AsyncOperationHandle asyncOperation) =>
                    {
                        eventCallback.StoreCaller(this.gameObject);
                        eventCallback.SignalChange();
                    }));
            }
            else {
                StartCoroutine(StandardAsyncSceneLoad(targetScene, loadMode, (AsyncOperation asyncOperation) =>
                {
                    eventCallback.StoreCaller(this.gameObject);
                        eventCallback.SignalChange();
                    }));
            }
        }

        private IEnumerator AddressablesAsyncSceneLoad (string xSceneName, LoadSceneMode xLoadMode, Action<AsyncOperationHandle> callback)
        {
            AsyncOperationHandle asyncLoad = Addressables.LoadSceneAsync(xSceneName, xLoadMode);
            asyncLoad.Completed += callback;

            // When loading additive scenes using addressables, we need to
            // store a reference to the scene so we can unload it later.
            if (xLoadMode == LoadSceneMode.Additive && addressableSceneInstances.ContainsKey(xSceneName) == false) {
                addressableSceneInstances.Add(xSceneName, asyncLoad.Convert<SceneInstance>());
            }
            
            while (!asyncLoad.IsDone) {
                sceneLoadingProgress = Mathf.Clamp01(asyncLoad.PercentComplete / .9f);
                yield return null;
            }
        }
        
        private IEnumerator StandardAsyncSceneLoad (string xSceneName, LoadSceneMode xLoadMode, Action<AsyncOperation> callback)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(xSceneName, xLoadMode);
            asyncLoad.completed += callback;

            while (!asyncLoad.isDone) {
                sceneLoadingProgress = Mathf.Clamp01(asyncLoad.progress / .9f);
                yield return null;
            }
        }

        private void AddressablesAsyncLoadCompleted(AsyncOperationHandle asyncOperationHandle)
        {
            this.sceneLoadCompletedCallback.StoreCaller(this.gameObject);
            this.sceneLoadCompletedCallback.SignalChange();
        }

        private void StandardAsyncLoadCompleted(AsyncOperation asyncOperation)
        {
            this.sceneLoadCompletedCallback.StoreCaller(this.gameObject); 
            this.sceneLoadCompletedCallback.SignalChange();
        }

        public void TriggerUnloadScene(ComplexPayload complexPayload)
        {
            string unloadSceneName = complexPayload.GetStringValue(DataType.stringType);
            
            if (SceneManager.GetSceneByName(unloadSceneName).IsValid() == false )
            {
                return;
            }

            var payloadEvent = complexPayload.GetScriptableObjectValue(this.eventCallbackKey) as SimpleEvent;
            SimpleEvent eventCallback;
            if (payloadEvent != null) {
                eventCallback = payloadEvent;
            }
            else {
                eventCallback = sceneUnloadCompleted.GetVariable() as SimpleEvent;
            }

            if (appSettings.useAddressables == true) {
                StartCoroutine(AddressablesAsyncSceneUnload(unloadSceneName, (AsyncOperationHandle asyncOperation) =>
                {
                    eventCallback.StoreCaller(this.gameObject);
                    eventCallback.SignalChange();
                }));
            }
            else {
                StartCoroutine(StandardAsyncSceneUnload(unloadSceneName, (AsyncOperation asyncOperation) =>
                {
                    eventCallback.StoreCaller(this.gameObject);
                    eventCallback.SignalChange();
                }));
            }
        }

        private IEnumerator AddressablesAsyncSceneUnload(string xSceneName, Action<AsyncOperationHandle> callback)
        {
            AsyncOperationHandle<SceneInstance> unloadInstance = addressableSceneInstances[xSceneName];

            if (addressableSceneInstances.ContainsKey(xSceneName) == true) {
                addressableSceneInstances.Remove(xSceneName);
            }

            AsyncOperationHandle asyncLoad = Addressables.UnloadSceneAsync(unloadInstance);
            asyncLoad.Completed += callback;

            while (!asyncLoad.IsDone) {
                yield return null;
            }
        }
        
        private IEnumerator StandardAsyncSceneUnload(string xSceneName, Action<AsyncOperation> callback)
        {
            AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(xSceneName);
            asyncLoad.completed += callback;
            
            while (!asyncLoad.isDone) {
                yield return null;
            }
        }

        public void ResetSceneLoadingProgress()
        {
            sceneLoadingProgress = 0;
        }

        public void QuitApplication()
        {
            Application.Quit();
        }

        private static bool IsPopulated(ComplexEventManualTrigger attribute)
        {
            return Utils.IsPopulated(attribute);
        }
        
        private static bool IsPopulated(SimpleEventReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
        
        private static bool IsPopulated(CustomKeyReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}
