using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Events;

namespace AltSalt.Maestro
{
    public class UnityEventData
    {
        private int _instanceID;

        private int instanceID
        {
            set => _instanceID = value;
        }

        private string _targetName;

        public string targetName
        {
            get => _targetName;
            private set => _targetName = value;
        }

        private string _methdodName;

        public string methdodName
        {
            get => _methdodName;
            private set => _methdodName = value;
        }

        private string _parameterName = "";

        public string parameterName
        {
            get => _parameterName;
            private set => _parameterName = value;
        }

        private UnityEventData(int instanceID, string targetName, string methdodName)
        {
            this.instanceID = instanceID;
            this.targetName = targetName;
            this.methdodName = methdodName;
        }

        public static List<UnityEventData> GetUnityEventData(GameObjectGenericAction genericAction, string[] parameterNames = null)
        {
            List<UnityEventData> eventData = new List<UnityEventData>();
            
            for (int i = 0; i < genericAction.GetPersistentEventCount(); i++) {
                if (genericAction.GetPersistentTarget(i) != null) {
                    var data = new UnityEventData(genericAction.GetPersistentTarget(i).GetInstanceID(),
                        genericAction.GetPersistentTarget(i).name, genericAction.GetPersistentMethodName(i));
                    if (parameterNames != null) {
                        data.parameterName = parameterNames[i];
                    } 
                    eventData.Add(data);
                }
            }

            return eventData;
        }
    }
}