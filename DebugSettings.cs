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
            set => _responsiveLayoutActive = value;
        }
        
        [SerializeField]
        private bool _modifyTextActive = true;

        public bool modifyTextActive
        {
            get => _modifyTextActive;
            set => _modifyTextActive = value;
        }

        [SerializeField]
        private bool _modifyLayoutActive = true;

        public bool modifyLayoutActive
        {
            get => _modifyLayoutActive;
            set => _modifyLayoutActive = value;
        }
        
        [SerializeField]
        private bool _saveDataActive = true;
        
        public bool saveDataActive
        {
            get => _saveDataActive;
            set => _saveDataActive = value;
        }
        
        [SerializeField]
        private bool _useAddressables = false;

        public bool useAddressables
        {
            get => _useAddressables;
            set => _useAddressables = value;
        }
        
        [SerializeField]
        private bool _logEventCallersAndListeners = false;
        
        public bool logEventCallersAndListeners
        {
            get => _logEventCallersAndListeners;
            set => _logEventCallersAndListeners = value;
        }
        
        [SerializeField]
        private bool _logResponsiveElementActions = false;

        public bool logResponsiveElementActions
        {
            get => _logResponsiveElementActions;
            set => _logResponsiveElementActions = value;
        }
        
        [SerializeField]
        private bool _logConditionResponses = false;

        public bool logConditionResponses
        {
            get => _logConditionResponses;
            set => _logConditionResponses = value;
        }

        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void SetDefaults()
        {
            responsiveLayoutActive = true;
            modifyTextActive = true;
            modifyLayoutActive = true;
            saveDataActive = true;
            useAddressables = false;
            logEventCallersAndListeners = false;
            logResponsiveElementActions = false;
            logConditionResponses = false;
        }
    }
}