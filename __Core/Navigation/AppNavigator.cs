/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System.Collections;
using UnityEngine;
using DoozyUI;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

namespace AltSalt
{
    public class AppNavigator : MonoBehaviour {
        
        // Scene transition variables
        [Required]
        public AppSettings appSettings;
        
        [ValidateInput("IsPopulated")]
        public StringReference activeScene;

        [SerializeField]
        [Required]
        SimpleEvent fadeOutTriggered;

        [SerializeField]
        [Required]
        SimpleEvent sceneLoadCompleted;

        [SerializeField]
        [Required]
        SimpleEvent timescaleChanged;

        string sceneName;
        LoadSceneMode loadMode;

        // The first scene will always be a single load, done immediately w/o a call to
        // make a fade out first, so we have a special case for it here
        public void LoadInitialScene(EventPayload eventPayload) {
            sceneName = eventPayload.GetStringValue(DataType.stringType);
            StartCoroutine(AsyncLoad(sceneName, LoadSceneMode.Single));
        }

        // Otherwise, single scene loads necessitate doing a fade out before doing the load,
        // and additive scenes are loaded immediately without a call to the fader
        public void TriggerSceneLoad(EventPayload eventPayload) {
            
            sceneName = eventPayload.GetStringValue(DataType.stringType);
            loadMode = eventPayload.GetBoolValue(DataType.boolType) == true ? LoadSceneMode.Additive : LoadSceneMode.Single;

            if(loadMode == LoadSceneMode.Single) {
                fadeOutTriggered.Raise();
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
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(xSceneName, xLoadMode);
            asyncLoad.completed += SceneLoadCallback;
            
            while (!asyncLoad.isDone) {
                yield return null;
            }
        }
        
        void SceneLoadCallback(AsyncOperation asyncOperation)
        {
            sceneLoadCompleted.Raise();
        }

        public void TriggerUnloadScene(EventPayload eventPayload)
        {
            string unloadSceneName = eventPayload.GetStringValue(DataType.stringType);
            StartCoroutine(AsyncUnload(unloadSceneName));
        }

        IEnumerator AsyncUnload(string xSceneName)
        {
            AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(xSceneName);
            while (!asyncLoad.isDone) {
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