using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro
{
    [CreateAssetMenu(menuName = "Maestro/Variables/Vector2 Variable")]
    public class V2Variable : ModifiableEditorVariable
    {
        protected override string title => nameof(V2Variable);

        [SerializeField]
        private Vector2 _value;

        public Vector2 value
        {
            get => _value;
            private set => _value = value;
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
            if (CallerRegistered() == false) return;

            this.value = value;

            SignalChange();
        }

        public void SetValue(V2Variable value)
        {
            if (CallerRegistered() == false) return;

            this.value = value.value;

            SignalChange();
        }

        public void ApplyChange(Vector2 amount)
        {
            if (CallerRegistered() == false) return;

            value += amount;

            SignalChange();
        }

        public void ApplyChange(V2Variable amount)
        {
            if (CallerRegistered() == false) return;

            value += amount.value;

            SignalChange();
        }
        
        public override void SetToDefaultValue()
        {
            if (CallerRegistered() == false) return;

            if (hasDefault)  {
                value = defaultValue;
            } else  {
                Debug.LogWarning("Method SetDefaultValue() called on " + this.name + ", but var does not have a default value assigned.");
            }

            SignalChange();
        }
    }
}