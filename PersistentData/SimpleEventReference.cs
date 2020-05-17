using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro
{
    [Serializable]
    public class SimpleEventReference : ReferenceBase
    {
        [Required("Field not populated - make sure this is intentional", InfoMessageType.Warning)]
        [SerializeField]
        [FormerlySerializedAs("_simpleEvent")]
        [FormerlySerializedAs("simpleEvent")]
        [OnValueChanged(nameof(UpdateReferenceName))]
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 5)]
        protected SimpleEvent _variable;

        private bool _isRequired;
        
        public bool isRequired
        {
            get => _isRequired;
            set => _isRequired = value;
        } 
        
        public override ScriptableObject GetVariable()
        {
            base.GetVariable();
            return _variable;
        }

        public void SetVariable(SimpleEvent value) => _variable = value;
        
#if UNITY_EDITOR
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