using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt
{
    [CreateAssetMenu(menuName = "AltSalt/Vector2 Variable")]
    public class V2Variable : VariableBase
    {
#if UNITY_EDITOR
        [Multiline]
        [SerializeField]
        [Header("Vector2 Variable")]
        string DeveloperDescription = "";
#endif
        [SerializeField]
        private Vector2 _value;

        public Vector2 value
        {
            get => _value;
            set => _value = value;
        }
        
        [SerializeField]
        [ShowIf(nameof(hasDefault))]
        private Vector2 _defaultValue;

        public Vector2 defaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }

        public void SetValue(Vector2 value)
        {
            this.value = value;
        }

        public void SetValue(V2Variable value)
        {
            this.value = value.value;
        }

        public void ApplyChange(Vector2 amount)
        {
            value += amount;
        }

        public void ApplyChange(V2Variable amount)
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