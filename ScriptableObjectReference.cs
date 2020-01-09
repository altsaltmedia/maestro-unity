using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro
{
    [Serializable]
    public class ScriptableObjectReference : ReferenceBase
    {
        [SerializeField]
        [OnValueChanged(nameof(UpdateReferenceName))]
        private ScriptableObject _variable;

        public ScriptableObject variable
        {
            get
            {
#if UNITY_EDITOR
                if (hasSearchedForAsset == false && _variable == null && string.IsNullOrEmpty(referenceName) == false) {
                    hasSearchedForAsset = true;
                    LogMissingReferenceMessage(GetType().Name);
                    _variable = Utils.GetScriptableObject(referenceName) as IntVariable;
                }
#endif
                return _variable;
            }
            set => _variable = value;
        }
        
        protected override void UpdateReferenceName()
        {
            if (_variable != null) {
                hasSearchedForAsset = false;
                referenceName = _variable.name;
            }
//            else {
//                referenceName = "";
//            }
        }
    }
}