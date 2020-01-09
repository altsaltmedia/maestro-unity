using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro.Logic.ConditionResponse
{
    [Serializable]
    [ExecuteInEditMode]
    public abstract class ConditionResponseBase
    {
        [SerializeField]
        [HideInInspector]
        protected string _conditionEventTitle;

        protected string conditionEventTitle
        {
            get => _conditionEventTitle;
            set => _conditionEventTitle = value;
        }

        [ShowInInspector]
        [ReadOnly]
        private static AppSettings _appSettings;

        private static AppSettings appSettings
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

        [SerializeField]
        [ReadOnly]
        private UnityEngine.Object _parentObject;

        public Object parentObject
        {
            get => _parentObject;
            set => _parentObject = value;
        }

        [PropertySpace]
        [ValidateInput("IsPopulated")]
        [SerializeField]
        [PropertyOrder(8)]
        protected UnityEvent response;

        public virtual void SyncValues(Object callingObject)
        {
            this.parentObject = callingObject;
        }

        public virtual bool CheckCondition(Object callingObject)
        {
            this.parentObject = callingObject;
            return true;
        }

        public virtual void TriggerResponse(GameObject callingObject, bool triggerOnStart)
        {
            this.parentObject = callingObject;
            if (CheckCondition(callingObject) == true) {
                if(appSettings.logConditionResponses == true) {
                    LogConditionResponse(callingObject, triggerOnStart);
                }
                response.Invoke();
            }
        }

        void LogConditionResponse(GameObject callerObject, bool triggerOnStart)
        {
            SyncValues(parentObject);
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