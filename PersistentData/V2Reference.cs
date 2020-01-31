using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro
{
    [Serializable]
    public class V2Reference : VariableReference
    {
        [FormerlySerializedAs("ConstantValue")]
        [SerializeField]
        private Vector2 _constantValue;

        public Vector2 constantValue
        {
            get => _constantValue;
            set => _constantValue = value;
        }

        [FormerlySerializedAs("Variable")]
        [SerializeField]
        [OnValueChanged(nameof(UpdateReferenceName))]
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 5)]
        private V2Variable _variable;

        public override ScriptableObject GetVariable() {
            base.GetVariable();
            return _variable;
        }
        
        public void SetVariable(V2Variable value)
        {
            _variable = value;
        }

        public V2Reference()
        { }

        public V2Reference(Vector2 value)
        {
            useConstant = true;
            constantValue = value;
        }

        public Vector2 GetValue()
        {
            return useConstant ? constantValue : (GetVariable() as V2Variable).value;
        }
        
        public V2Variable SetValue(GameObject callingObject, Vector2 targetValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            V2Variable v2Variable = GetVariable() as V2Variable;;
            v2Variable.StoreCaller(callingObject);
            v2Variable.SetValue(targetValue);
            return v2Variable;
        }
        
#if UNITY_EDITOR
        protected override bool ShouldPopulateReference()
        {
            if (useConstant == false && _variable == null) {
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