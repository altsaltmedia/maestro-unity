using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        
        [TitleGroup("$" + nameof(conditionEventTitle))]
        [ShowInInspector]
        [HideLabel]
        [DisplayAsString(false)]
        [PropertySpace]
        private string _eventDescription = "No actions defined.";
        
        public string eventDescription
        {
            get => _eventDescription;
            set => _eventDescription = value;
        }
        
        [FormerlySerializedAs("response")]
        [PropertySpace]
        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        [PropertyOrder(8)]
        [HideReferenceObjectPicker]
        protected UnityEvent _response;

        public UnityEvent response => _response;

#if UNITY_EDITOR
        private List<UnityEventData> _cachedEventData = new List<UnityEventData>();

        private List<UnityEventData> cachedEventData
        {
            get => _cachedEventData;
            set => _cachedEventData = value;
        }

        public virtual void SyncConditionHeading(Object callingObject)
        {
            this.parentObject = callingObject;
        }

        public void SyncUnityEventHeading(SerializedProperty serializedConditionResponse)
        {
            string[] parameterNames = GetParameters(serializedConditionResponse);
            if (UnityEventValuesChanged(response, parameterNames, cachedEventData, out var eventData)) {
                eventDescription = GetEventDescription(eventData);
                cachedEventData = eventData;
            }
        }
#endif

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

        private static string[] GetParameters(SerializedProperty serializedConditionResponse)
        {
            SerializedProperty unityEventCallList = serializedConditionResponse
                .FindPropertyRelative($"{nameof(_response)}.m_PersistentCalls.m_Calls");

            List<string> parameterNames = new List<string>();
            
            for (int i = 0; i < unityEventCallList.arraySize; i++) {
                int mode = unityEventCallList.GetArrayElementAtIndex(i).FindPropertyRelative("m_Mode").intValue;
                SerializedProperty argumentList = unityEventCallList.GetArrayElementAtIndex(i).FindPropertyRelative("m_Arguments");

                switch (mode) {
                    case 2:
                        parameterNames.Add(argumentList.FindPropertyRelative("m_ObjectArgument").objectReferenceValue.name);
                        break;
                    
                    case 3:
                        parameterNames.Add(argumentList.FindPropertyRelative("m_IntArgument").intValue.ToString());
                        break;
                    
                    case 4:
                        parameterNames.Add(argumentList.FindPropertyRelative("m_FloatArgument").floatValue.ToString("F"));
                        break;
                    
                    case 5:
                        parameterNames.Add(argumentList.FindPropertyRelative("m_StringArgument").stringValue);
                        break;
                    
                    case 6:
                        parameterNames.Add(argumentList.FindPropertyRelative("m_BoolArgument").boolValue.ToString());
                        break;
                    
                    default:
                        parameterNames.Add("");
                        break;
                }
            }

            return parameterNames.ToArray();
        }

        private static bool UnityEventValuesChanged(UnityEvent unityEvent, string[] parameterNames,
            List<UnityEventData> cachedEventData, out List<UnityEventData> eventData)
        {
            
            eventData = UnityEventData.GetUnityEventData(unityEvent, parameterNames);
            var addedItems = eventData.Except(cachedEventData);

            return addedItems.Any();
        }

        private static string GetEventDescription(List<UnityEventData> eventData)
        {
            string eventDescription = "";

            for (int i = 0; i < eventData.Count; i++) {
                eventDescription += eventData[i].targetName;
                if (string.IsNullOrEmpty(eventData[i].methdodName) == false) {
                    eventDescription += $" > {eventData[i].methdodName}";
                    eventDescription += $" ({eventData[i].parameterName})";
                }

                if (i < eventData.Count - 1) {
                    eventDescription += "\n";
                }
            }
            
            return eventDescription;
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
    }

}