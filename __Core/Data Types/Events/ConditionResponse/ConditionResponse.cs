using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [Serializable]
    [ExecuteInEditMode]
    public abstract class ConditionResponse
    {
        protected string conditionEventTitle;

        [Required]
        [SerializeField]
        AppSettings appSettings;

        [PropertySpace]
        [ValidateInput("IsPopulated")]
        [SerializeField]
        [PropertyOrder(8)]
        protected UnityEvent response;

        public abstract void SyncValues();

        public abstract bool CheckCondition();

        public void TriggerResponse(GameObject caller, bool triggerOnStart)
        {
            GetAppSettings();
            if (CheckCondition() == true) {
                if(appSettings.logConditionResponses == true) {
                    LogConditionResponse(caller, triggerOnStart);
                }
                response.Invoke();
            }
        }

        void GetAppSettings()
        {
            if(appSettings == null) {
                appSettings = Utils.GetAppSettings();
            }
        }

        void LogConditionResponse(GameObject callerObject, bool triggerOnStart)
        {
            SyncValues();
            Debug.Log(string.Format("[condition response] [{0}] [{1}] Following condition met on start {2} : ", callerObject.scene.name, callerObject.name, triggerOnStart.ToString().ToUpper()), callerObject);
            Debug.Log(string.Format("[condition response] [{0}] [event] {1} ", callerObject.scene.name, conditionEventTitle), callerObject);
            Debug.Log(string.Format("[condition response] [{0}] {1} triggered the following :", callerObject.scene.name, callerObject.name), callerObject);
            for (int i=0; i<response.GetPersistentEventCount(); i++) {
                object targetObject = response.GetPersistentTarget(i);
                string objectScene = GetObjectScene(targetObject);
                string objectName = GetObjectName(targetObject);
                Debug.Log(string.Format("[condition response] [{0}] [{1}] {2}", objectScene, objectName, response.GetPersistentMethodName(i)), response.GetPersistentTarget(i));
            }
            Debug.Log("[condition response] ---------");
        }

        string GetObjectScene(object sourceObject)
        {
            if (sourceObject is GameObject) {
                return ((GameObject)sourceObject).scene.name;
            } else if (sourceObject is ScriptableObject) {
                return "Asset";
            } else if (sourceObject is MonoBehaviour) {
                return ((MonoBehaviour)sourceObject).gameObject.scene.name;
            } else {
                return "Scene Unknown";
            }
        }

        string GetObjectName(object sourceObject)
        {
            if (sourceObject is GameObject) {
                return ((GameObject)sourceObject).name;
            } else if (sourceObject is ScriptableObject) {
                return ((ScriptableObject)sourceObject).name;
            } else if (sourceObject is MonoBehaviour){
                return ((MonoBehaviour)sourceObject).name;
            } else {
                return sourceObject.ToString();
            }
        }

        public UnityEvent GetResponse()
        {
            return response;
        }

        private static bool IsPopulated(UnityEvent attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }

}