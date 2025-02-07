﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AltSalt.Maestro.Logic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro.Logic
{
    [Serializable]
    public abstract class ConditionResponse : IRegisterConditionResponse
    {
        public static bool debugMode => ActionTrigger.debugMode;
        
        public static bool manualOverride => ActionTrigger.manualOverride;

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

        [SerializeField]
        [ReadOnly]
        [ShowIf(nameof(debugMode))]
        private Object _parentObject;

        protected Object parentObject
        {
            get => _parentObject;
            set => _parentObject = value;
        }
        
        [SerializeField]
        [ShowIf(nameof(debugMode))]
        [EnableIf(nameof(manualOverride))]
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
            set => _eventDescription = value;
        }
        
        [PropertySpace(20)]
        
        [SerializeField]
        [PropertyOrder(9)]
        [ValidateInput(nameof(IsPopulated))]
        [HideReferenceObjectPicker]
        [HideIf(nameof(hideGenericAction))]
        protected GameObjectGenericAction _action = new GameObjectGenericAction();

        public GameObjectGenericAction action
        {
            get => _action;
            set => _action = value;
        }

        private bool _hideGenericAction;

        public bool hideGenericAction
        {
            get => _hideGenericAction;
            set => _hideGenericAction = value;
        }
        
        private bool _initialized;

        private bool initialized
        {
            get => _initialized;
            set => _initialized = value;
        }

        protected ConditionResponse CheckPopulateReferences()
        {
#if UNITY_EDITOR
            if (initialized == false) {
                initialized = true;
                PopulateReferences();
            }
#endif
            return this;
        }

        public virtual ConditionResponse PopulateReferences()
        {
            return this;
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

        public abstract void SyncConditionHeading(Object callingObject);

        public void SyncUnityEventHeading(SerializedProperty serializedConditionResponse)
        {
            UnityEventParameter[] parameters = UnityEventUtils.GetUnityEventParameters(serializedConditionResponse, nameof(_action));
            if (UnityEventUtils.UnityEventValuesChanged(action, parameters, cachedEventData, out var eventData)) {
                if (eventData.Count > 0) {
                    eventDescription = UnityEventUtils.ParseUnityEventDescription(eventData);
                    cachedEventData = eventData;
                }
            }
        }
#endif
        protected ConditionResponse(UnityEngine.Object parentObject,
            string serializedPropertyPath)
        {
            this.parentObject = parentObject;
            this.serializedPropertyPath = serializedPropertyPath;
        }

        public abstract bool CheckCondition(Object callingObject);

        public void TriggerResponse(GameObject callingObject)
        {
            if (CheckCondition(callingObject) == true) {
                if(AppSettings.logConditionResponses == true) {
                    LogConditionResponse(callingObject);
                }
                action.Invoke(callingObject);
            }
        }

        private void LogConditionResponse(GameObject callerObject)
        {
            Debug.Log(string.Format("[condition response] [{0}] [{1}] Following condition met : ", callerObject.scene.name, callerObject.name), callerObject);
            Debug.Log(string.Format("[condition response] [{0}] [event] {1} ", callerObject.scene.name, conditionEventTitle), callerObject);
            Debug.Log(string.Format("[condition response] [{0}] {1} triggered the following :", callerObject.scene.name, callerObject.name), callerObject);
            for (int i=0; i<action.GetPersistentEventCount(); i++) {
                object targetObject = action.GetPersistentTarget(i);
                string objectScene = GetObjectScene(targetObject);
                string objectName = GetObjectName(targetObject);
                Debug.Log(string.Format("[condition response] [{0}] [{1}] {2}", objectScene, objectName, action.GetPersistentMethodName(i)), action.GetPersistentTarget(i));
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