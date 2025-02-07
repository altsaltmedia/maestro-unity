using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro
{
    [Serializable]
    public class UserDataKeyReference : ReferenceBase
    {
        [SerializeField]
        [OnValueChanged(nameof(UpdateReferenceName))]
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 5)]
        private UserDataKey _variable;

        public override ScriptableObject GetVariable()
        {
            base.GetVariable();
            return _variable;
        }

        public void SetVariable(UserDataKey value)
        {
            _variable = value;
        }
        
        
        public UserDataKeyReference()
        { }

        public UserDataKeyReference(UserDataKey value)
        {
            _variable = value;
        }
        
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