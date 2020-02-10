using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AltSalt.Maestro
{
    public static class UnityEventUtils
    {
        
#if UNITY_EDITOR        
        public static UnityEventParameter[] GetUnityEventParameters(SerializedProperty eventList)
        {
            SerializedProperty unityEventCallList = eventList
                .FindPropertyRelative($"m_PersistentCalls.m_Calls");

            return ParseUnityEventParameters(unityEventCallList);
        }
        
        public static UnityEventParameter[] GetUnityEventParameters(SerializedObject eventSourceObject, string eventListName)
        {
            SerializedProperty unityEventCallList = eventSourceObject
                .FindProperty($"{eventListName}.m_PersistentCalls.m_Calls");

            return ParseUnityEventParameters(unityEventCallList);
        }

        public static UnityEventParameter[] GetUnityEventParameters(SerializedProperty eventListParentProperty, string eventListName)
        {
            SerializedProperty unityEventCallList = eventListParentProperty
                .FindPropertyRelative($"{eventListName}.m_PersistentCalls.m_Calls");

            return ParseUnityEventParameters(unityEventCallList);
        }

        private static UnityEventParameter[] ParseUnityEventParameters(SerializedProperty unityEventCallList)
        {
            List<UnityEventParameter> parameters = new List<UnityEventParameter>();
            
            for (int i = 0; i < unityEventCallList.arraySize; i++) {
                int mode = unityEventCallList.GetArrayElementAtIndex(i).FindPropertyRelative("m_Mode").intValue;
                SerializedProperty argumentList = unityEventCallList.GetArrayElementAtIndex(i).FindPropertyRelative("m_Arguments");
                
                switch (mode) {
                    case 2:
                    {
                        var value = argumentList.FindPropertyRelative("m_ObjectArgument").objectReferenceValue;
                        parameters.Add(value != null
                            ? new UnityEventParameter(value.GetType(), value, value.name)
                            : new UnityEventParameter(typeof(Object), null, "null"));
                        break;
                    }

                    case 3:
                    {
                        var value = argumentList.FindPropertyRelative("m_IntArgument").intValue;
                        parameters.Add(new UnityEventParameter(typeof(int), value, value.ToString()));
                        break;
                    }

                    case 4:
                    {
                        var value = argumentList.FindPropertyRelative("m_FloatArgument").floatValue;
                        parameters.Add(new UnityEventParameter(typeof(float), value, value.ToString("F")));
                        break;
                    }

                    case 5:
                    {
                        var value = argumentList.FindPropertyRelative("m_StringArgument").stringValue;
                        parameters.Add(new UnityEventParameter(typeof(string), value, value));
                        break;
                    }

                    case 6:
                    {
                        var value = argumentList.FindPropertyRelative("m_BoolArgument").boolValue;
                        parameters.Add(new UnityEventParameter(typeof(bool), value, value.ToString()));
                        break;
                    }
                    
                    default:
                        parameters.Add(new UnityEventParameter(null, null, ""));
                        break;
                }
            }

            return parameters.ToArray();
        }

        public static void MigrateUnityEventList(string originalCallListName, string targetCallListName, 
            SerializedObject sourceObject)
        {
            sourceObject.FindProperty("_migrated").boolValue = true;
            SerializedProperty originalCallList = sourceObject
                .FindProperty($"{originalCallListName}.m_PersistentCalls.m_Calls");
            SerializedProperty newCallList = sourceObject
                .FindProperty($"{targetCallListName}.m_PersistentCalls.m_Calls");

            CopyEventProperties(originalCallList, newCallList);
            
            sourceObject.ApplyModifiedProperties();
            sourceObject.Update();
        }

        public static void MigrateUnityEventList(string originalCallListName, string targetCallListName,
            SerializedProperty serializedProperty)
        {
            serializedProperty.FindPropertyRelative("_migrated").boolValue = true;
            SerializedProperty originalCallList = serializedProperty
                .FindPropertyRelative($"{originalCallListName}.m_PersistentCalls.m_Calls");
                
            SerializedProperty newCallList = serializedProperty
                .FindPropertyRelative($"{targetCallListName}.m_PersistentCalls.m_Calls");
            
            CopyEventProperties(originalCallList, newCallList);
            
            serializedProperty.serializedObject.ApplyModifiedProperties();
        }
        
        private static SerializedProperty CopyEventProperties(SerializedProperty originalCallList, SerializedProperty newCallList)
        {
            newCallList.ClearArray();
            
            for (int i = 0; i < originalCallList.arraySize; i++) {
                
                newCallList.InsertArrayElementAtIndex(i);
                
                var originalCallArrayElement = originalCallList.GetArrayElementAtIndex(i);
                var newCallArrayElement = newCallList.GetArrayElementAtIndex(i);

                newCallArrayElement.FindPropertyRelative("m_Target").objectReferenceValue =
                    originalCallArrayElement.FindPropertyRelative("m_Target").objectReferenceValue;

                newCallArrayElement.FindPropertyRelative("m_MethodName").stringValue =
                    originalCallArrayElement.FindPropertyRelative("m_MethodName").stringValue;
                
                newCallArrayElement.FindPropertyRelative("m_Mode").intValue =
                    originalCallArrayElement.FindPropertyRelative("m_Mode").intValue;
                
                newCallArrayElement.FindPropertyRelative("m_CallState").intValue =
                    originalCallArrayElement.FindPropertyRelative("m_CallState").intValue;

                SerializedProperty originalArgumentList = originalCallArrayElement.FindPropertyRelative("m_Arguments");
                SerializedProperty newArgumentList = newCallArrayElement.FindPropertyRelative("m_Arguments");
                
                newArgumentList.FindPropertyRelative("m_ObjectArgument").objectReferenceValue =
                    originalArgumentList.FindPropertyRelative("m_ObjectArgument").objectReferenceValue;
                
                newArgumentList.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName").stringValue =
                    originalArgumentList.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName").stringValue;

                newArgumentList.FindPropertyRelative("m_IntArgument").intValue =
                    originalArgumentList.FindPropertyRelative("m_IntArgument").intValue;
                
                newArgumentList.FindPropertyRelative("m_FloatArgument").floatValue =
                    originalArgumentList.FindPropertyRelative("m_FloatArgument").floatValue;

                newArgumentList.FindPropertyRelative("m_StringArgument").stringValue =
                    originalArgumentList.FindPropertyRelative("m_StringArgument").stringValue;

                newArgumentList.FindPropertyRelative("m_BoolArgument").boolValue =
                    originalArgumentList.FindPropertyRelative("m_BoolArgument").boolValue;
            }

            return newCallList;
        }

        public static bool UnityEventValuesChanged(GameObjectGenericAction genericAction, UnityEventParameter[] parameterNames,
            List<UnityEventData> cachedEventData, out List<UnityEventData> eventData)
        {
            eventData = GetUnityEventData(genericAction, parameterNames);
            var addedItems = eventData.Except(cachedEventData);
            return addedItems.Any();
        }

        public static string ParseUnityEventDescription(List<UnityEventData> eventData)
        {
            string eventDescription = "";

            for (int i = 0; i < eventData.Count; i++) {
                eventDescription += eventData[i].targetName;
                if (String.IsNullOrEmpty(eventData[i].methodName) == false) {
                    eventDescription += $" > {eventData[i].methodName}";
                    eventDescription += $" ({eventData[i].parameter.valueName})";
                }

                if (i < eventData.Count - 1) {
                    eventDescription += "\n";
                }
            }
            
            return eventDescription;
        }
        
        public static List<UnityEventData> GetUnityEventData(UnityEventBase unityEvent, UnityEventParameter[] parameters = null)
        {
            List<UnityEventData> eventData = new List<UnityEventData>();
            
            for (int i = 0; i < unityEvent.GetPersistentEventCount(); i++) {
                if (unityEvent.GetPersistentTarget(i) != null) {
                    var data = new UnityEventData(unityEvent.GetPersistentTarget(i).GetInstanceID(),
                        unityEvent.GetPersistentTarget(i).name, unityEvent.GetPersistentMethodName(i));
                    if (parameters != null) {
                        data.parameter = parameters[i];
                    } 
                    eventData.Add(data);
                }
            }

            return eventData;
        }
#endif
    }
}