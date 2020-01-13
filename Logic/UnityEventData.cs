using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Events;

namespace AltSalt.Maestro.Logic
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

        public static List<UnityEventData> GetUnityEventData(UnityEvent unityEvent, string[] parameterNames = null)
        {
            List<UnityEventData> eventData = new List<UnityEventData>();
            
            for (int i = 0; i < unityEvent.GetPersistentEventCount(); i++) {
                if (unityEvent.GetPersistentTarget(i) != null) {
                    var data = new UnityEventData(unityEvent.GetPersistentTarget(i).GetInstanceID(),
                        unityEvent.GetPersistentTarget(i).name, unityEvent.GetPersistentMethodName(i));
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