using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using UnityEditor;
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

        public string conditionEventTitle
        {
            get => prefix + " (" + _conditionEventTitle + ")";
            set => _conditionEventTitle = value;
        }

        private string _prefix;

        public string prefix
        {
            get => _prefix;
            set => _prefix = value;
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
        private Object _parentObject;

        protected Object parentObject
        {
            get => _parentObject;
            set => _parentObject = value;
        }
        
        [SerializeField]
        [ReadOnly]
        private string _serializedPropertyPath;

        protected string serializedPropertyPath
        {
            get => _serializedPropertyPath;
            set => _serializedPropertyPath = value;
        }
        
        [TitleGroup("$" + nameof(conditionEventTitle))]
        [ShowInInspector]
        [HideLabel]
        [DisplayAsString(false)]
        private string _eventDescription = "No actions defined.";
        
        public string eventDescription
        {
            get
            {
                if (_eventDescription == null) {
                    _eventDescription = "No actions defined.";
                }
                return _eventDescription;
            }
            private set => _eventDescription = value;
        }
        
        [FormerlySerializedAs("response")]
        [PropertySpace]
        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        [PropertyOrder(8)]
        [HideReferenceObjectPicker]
        [ReadOnly]
        protected UnityEvent _response;

        public UnityEvent response => _response;

        [SerializeField]
        [PropertyOrder(9)]
        [ValidateInput(nameof(IsPopulated))]
        [HideReferenceObjectPicker]
        protected GameObjectGenericAction _action = new GameObjectGenericAction();

        public GameObjectGenericAction action
        {
            get => _action;
            set => _action = value;
        }
        
        [SerializeField]
        private bool _migrated = false;

        private bool migrated
        {
            get => _migrated;
            set => _migrated = value;
        }

        private bool _initialized;

        public bool initialized
        {
            get => _initialized;
            set => _initialized = value;
        }

#if UNITY_EDITOR
        private List<UnityEventData> _cachedEventData = new List<UnityEventData>();

        private List<UnityEventData> cachedEventData
        {
            get
            {
                if (_cachedEventData == null) {
                    _cachedEventData = new List<UnityEventData>();
                }
                return _cachedEventData;
            }
            set => _cachedEventData = value;
        }

        public virtual void SyncConditionHeading(Object callingObject)
        {
            
        }

        public void SyncUnityEventHeading(SerializedProperty serializedConditionResponse)
        {
            if (migrated == false) {
                migrated = true;
                UnityEventUtils.MigrateUnityEventList(nameof(_response), nameof(_action), serializedConditionResponse);
            }
            
            string[] parameterNames = UnityEventUtils.GetUnityEventParameters(serializedConditionResponse, nameof(_action));
            if (UnityEventUtils.UnityEventValuesChanged(action, parameterNames, cachedEventData, out var eventData)) {
                if (eventData.Count > 0) {
                    eventDescription = UnityEventUtils.ParseUnityEventDescription(eventData);
                    cachedEventData = eventData;
                }
            }
        }
#endif
        public ConditionResponseBase(UnityEngine.Object parentObject,
            string serializedPropertyPath)
        {
            this.parentObject = parentObject;
            this.serializedPropertyPath = serializedPropertyPath;
        }

        protected ConditionResponseBase CheckPopulateReferences()
        {
            if (initialized == false) {
                initialized = true;
                PopulateReferences();
            }

            return this;
        }

        public virtual ConditionResponseBase PopulateReferences()
        {
            return this;
        }

        public virtual bool CheckCondition(Object callingObject)
        {
            return true;
        }

        public virtual void TriggerResponse(GameObject callingObject, bool triggerOnStart)
        {
            if (CheckCondition(callingObject) == true) {
                if(appSettings.logConditionResponses == true) {
                    LogConditionResponse(callingObject, triggerOnStart);
                }
                response.Invoke();
            }
        }

        private void LogConditionResponse(GameObject callerObject, bool triggerOnStart)
        {
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


        private static string GetObjectScene(object sourceObject)
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

        private static string GetObjectName(object sourceObject)
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

        protected static bool IsPopulated(UnityEvent attribute)
        {
            return Utils.IsPopulated(attribute);
        }
        
        protected static bool IsPopulated(GameObjectGenericAction attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }

}