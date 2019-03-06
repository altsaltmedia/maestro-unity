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

        [Required]
        public LocalizationManager m_localizationManager;

        string sceneName;
        LoadSceneMode loadMode;
        
        void Start() {
            if(m_localizationManager == null) {
                Debug.LogError("Please create and attach an AltSalt localization manager to AppNavigator.");
            }
        }
        
        public void TriggerSceneLoad(EventPayload eventPayload) {
            
            sceneName = eventPayload.GetStringValue(EventPayloadType.stringPayload.ToString());
            loadMode = eventPayload.GetBoolValue(EventPayloadType.boolPayload.ToString()) == true ? LoadSceneMode.Additive : LoadSceneMode.Single;

            if(loadMode == LoadSceneMode.Single) {
                fadeOutTriggered.Raise();
            } else {
                StartCoroutine(AsyncLoad(sceneName, loadMode));
            }
            
            if (!m_localizationManager.texts.ContainsKey(activeScene.Value) && appSettings.localizationActive) {
                m_localizationManager.LoadLocalizedText(activeScene.Value + "/" + activeScene.Value + "_en.xml", activeScene.Value);
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
            string unloadSceneName = eventPayload.GetStringValue(EventPayloadType.stringPayload.ToString());
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
        
        private static bool IsPopulated(StringReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}