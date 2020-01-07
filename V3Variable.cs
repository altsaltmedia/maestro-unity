using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro
{
    [CreateAssetMenu(menuName = "AltSalt/Variables/Vector3 Variable")]
    public class V3Variable : VariableBase
    {
#if UNITY_EDITOR
        [Multiline]
        [SerializeField]
        [Header("Vector3 Variable")]
        string DeveloperDescription = "";
#endif
        [SerializeField]
        public Vector3 _value;

        public Vector3 value
        {
            get => _value;
            set => _value = value;
        }
        
        [SerializeField]
        [ShowIf(nameof(hasDefault))]
        private Vector3 _defaultValue;

        public Vector3 defaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }

        public void SetValue(Vector3 value)
        {
            this.value = value;
        }

        public void SetValue(V3Variable value)
        {
            this.value = value.value;
        }

        public void ApplyChange(Vector3 amount)
        {
            value += amount;
        }

        public void ApplyChange(V3Variable amount)
        {
            value += amount.value;
        }
        
        public override void SetDefaultValue()
        {
            if (hasDefault)  {
                value = defaultValue;
            } else  {
                Debug.LogWarning("Method SetDefaultValue() called on " + this.name + ", but var does not have a default value assigned.");
            }
        }
    }
}