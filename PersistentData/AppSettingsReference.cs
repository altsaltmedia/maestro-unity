using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace AltSalt.Maestro
{
    [Serializable]
    public class AppSettingsReference : ReferenceBase
    {
        [SerializeField]
        [OnValueChanged(nameof(UpdateReferenceName))]
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 5)]
        private AppSettings _variable;

        public override ScriptableObject GetVariable()
        {
            base.GetVariable();
            return _variable;
        }
        
        public void SetVariable(AppSettings value)
        {
            _variable = value;
        }
        
#if UNITY_EDITOR
        
        public override ReferenceBase PopulateVariable(UnityEngine.Object parentObject, string serializedPropertyPath)
        {
            var serializedObject = new SerializedObject(parentObject);
            var referenceNameProperty =
                Utils.FindReferenceProperty(serializedObject, $"{serializedPropertyPath}.{nameof(_referenceName)}");
            referenceNameProperty.stringValue = nameof(AppSettings);
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
            
            return base.PopulateVariable(parentObject, serializedPropertyPath);
        }
        
        protected override bool ShouldPopulateReference()
        {
            if (_variable == null) {
                return true;
            }

            return false;
        }

        protected override ScriptableObject ReadVariable()
        {
            return _variable;
        }
#endif

    }
}