using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro
{
    [Serializable]
    public class StringReference : VariableReference
    {
        [FormerlySerializedAs("ConstantValue")]
        [SerializeField]
        [ShowIf(nameof(useConstant))]
        [HideIf(nameof(hideConstantOptions))]
        private string _constantValue;

        private string constantValue
        {
            get => _constantValue;
            set => _constantValue = value;
        }

        [FormerlySerializedAs("Variable")]
        [SerializeField]
        [OnValueChanged(nameof(UpdateReferenceName))]
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 5)]
        [HideIf(nameof(useConstant))]
        private StringVariable _variable;

        public override ScriptableObject GetVariable()
        {
            base.GetVariable();
            return _variable;
        }

        public void SetVariable(StringVariable value) => _variable = value;
        
        public StringReference()
        { }

        public StringReference(string value)
        {
            useConstant = true;
            constantValue = value;
        }

        public string GetValue()
        {
            return useConstant ? constantValue : (GetVariable() as StringVariable).value;
        }
        
        public StringVariable SetValue(GameObject callingObject, string targetValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            StringVariable stringVariable = GetVariable() as StringVariable;
            stringVariable.StoreCaller(callingObject);
            stringVariable.SetValue(targetValue);
            return stringVariable;
        }
        
        public StringVariable SetValue(GameObject callingObject, StringVariable targetValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            StringVariable stringVariable = GetVariable() as StringVariable;
            stringVariable.StoreCaller(callingObject);
            stringVariable.SetValue(targetValue.value);
            return stringVariable;
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