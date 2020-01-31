using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro
{
    [Serializable]
    public class ComplexEventReference : ReferenceBase
    {
        [SerializeField]
        [FormerlySerializedAs("_complexEvent"),Required]
        [FormerlySerializedAs("complexEvent")]
        [OnValueChanged(nameof(UpdateReferenceName))]
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 5)]
        protected ComplexEvent _variable;
        
        public override ScriptableObject GetVariable()
        {
            base.GetVariable();
            return _variable;
        }

        public void SetVariable(ComplexEvent value) => _variable = value;
        
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