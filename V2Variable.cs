using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro
{
    [CreateAssetMenu(menuName = "AltSalt/Variables/Vector2 Variable")]
    public class V2Variable : ModifiableEditorVariable
    {
        protected override string title => nameof(V2Variable);

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
        
        public override void SetToDefaultValue()
        {
            if (hasDefault)  {
                value = defaultValue;
            } else  {
                Debug.LogWarning("Method SetDefaultValue() called on " + this.name + ", but var does not have a default value assigned.");
            }
        }
    }
}