using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro
{
    [Serializable]
    public class TextFamilyReference : ReferenceBase
    {
        [SerializeField]
        [OnValueChanged(nameof(UpdateReferenceName))]
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 5)]
        private TextFamily _variable;

        public TextFamily GetVariable(Object callingObject)
        {
#if UNITY_EDITOR
            this.parentObject = callingObject;
            if (searchAttempted == false && _variable == null && string.IsNullOrEmpty(referenceName) == false) {
                searchAttempted = true;
                LogMissingReferenceMessage(GetType().Name);
                _variable = Utils.GetScriptableObject(referenceName) as TextFamily;
                if (_variable != null) {
                    LogFoundReferenceMessage(GetType().Name, _variable);
                }
            }
#endif
            return _variable;
        }
        
        public void SetVariable(TextFamily value)
        {
            _variable = value;
        }
        
        public TextFamilyReference()
        { }

        public TextFamilyReference(TextFamily value)
        {
            _variable = value;
        }

        protected override void UpdateReferenceName()
        {
            if (GetVariable(parentObject) != null) {
                searchAttempted = false;
                referenceName = GetVariable(parentObject).name;
            }
//			else {
//				referenceName = "";
//			}
        }
    }
}