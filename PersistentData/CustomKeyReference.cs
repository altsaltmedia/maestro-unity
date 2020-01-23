using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro
{
    [Serializable]
    public class CustomKeyReference : ReferenceBase
    {
        [SerializeField]
        [OnValueChanged(nameof(UpdateReferenceName))]
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 5)]
        private CustomKey _variable;

        public override ScriptableObject GetVariable()
        {
            base.GetVariable();
            return _variable;
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
        
        public void SetVariable(CustomKey value)
        {
            _variable = value;
        }

        public CustomKeyReference()
        { }

        public CustomKeyReference(CustomKey value)
        {
            _variable = value;
        }
    }
}