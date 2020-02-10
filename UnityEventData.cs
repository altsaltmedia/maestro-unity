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

        private string _methodName;

        public string methodName
        {
            get => _methodName;
            private set => _methodName = value;
        }

        private UnityEventParameter _parameter;

        public UnityEventParameter parameter
        {
            get => _parameter;
            set => _parameter = value;
        }

        public UnityEventData(int instanceID, string targetName, string methodName)
        {
            this.instanceID = instanceID;
            this.targetName = targetName;
            this.methodName = methodName;
        }
    }
}