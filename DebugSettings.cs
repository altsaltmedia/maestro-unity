using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace AltSalt.Maestro
{
    [CreateAssetMenu(menuName = "AltSalt/Debug Settings")]
    public class DebugSettings : ScriptableObject
    {
        
        [SerializeField]
        private bool _responsiveLayoutActive = true;
        
        public bool responsiveLayoutActive
        {
            get => _responsiveLayoutActive;
            private set => _responsiveLayoutActive = value;
        }
        
        [SerializeField]
        private bool _modifyTextActive = true;

        public bool modifyTextActive
        {
            get => _modifyTextActive;
            private set => _modifyTextActive = value;
        }

        [SerializeField]
        private bool _modifyLayoutActive = true;

        public bool modifyLayoutActive
        {
            get => _modifyLayoutActive;
            private set => _modifyLayoutActive = value;
        }
        
        [SerializeField]
        private bool _saveDataActive = true;
        
        public bool saveDataActive
        {
            get => _saveDataActive;
            private set => _saveDataActive = value;
        }
        
        [SerializeField]
        private bool _useAddressables = false;

        public bool useAddressables
        {
            get => _useAddressables;
            private set => _useAddressables = value;
        }
        
        [SerializeField]
        private bool _logEventCallersAndListeners = false;
        
        public bool logEventCallersAndListeners
        {
            get => _logEventCallersAndListeners;
            private set => _logEventCallersAndListeners = value;
        }
        
        [SerializeField]
        private bool _logResponsiveElementActions = false;

        public bool logResponsiveElementActions
        {
            get => _logResponsiveElementActions;
            private set => _logResponsiveElementActions = value;
        }
        
        [SerializeField]
        private bool _logConditionResponses = false;

        public bool logConditionResponses
        {
            get => _logConditionResponses;
            private set => _logConditionResponses = value;
        }
    }
}