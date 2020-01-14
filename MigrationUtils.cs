using System.Collections.Generic;
using UnityEditor;

namespace AltSalt.Maestro
{
    public static class MigrationUtils
    {
        public static string[] GetUnityEventParameters(SerializedObject eventSourceObject, string eventListName)
        {
            SerializedProperty unityEventCallList = eventSourceObject
                .FindProperty($"{eventListName}.m_PersistentCalls.m_Calls");

            return ParseUnityEventParameters(unityEventCallList);
        }

        public static string[] GetUnityEventParameters(SerializedProperty eventListParentProperty, string eventListName)
        {
            SerializedProperty unityEventCallList = eventListParentProperty
                .FindPropertyRelative($"{eventListName}.m_PersistentCalls.m_Calls");

            return ParseUnityEventParameters(unityEventCallList);
        }

        private static string[] ParseUnityEventParameters(SerializedProperty unityEventCallList)
        {
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
    }
}