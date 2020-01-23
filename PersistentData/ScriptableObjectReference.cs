using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro
{
    [Serializable]
    public class ScriptableObjectReference : ReferenceBase
    {
        [SerializeField]
        [OnValueChanged(nameof(UpdateReferenceName))]
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 5)]
        private ScriptableObject _variable;

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
        
        public void SetVariable(ScriptableObject value)
        {
            _variable = value;
        }
        
        
        public ScriptableObjectReference()
        { }

        public ScriptableObjectReference(ScriptableObject value)
        {
            _variable = value;
        }

    }
}