using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro
{
    [CreateAssetMenu(menuName = "Maestro/Variables/Vector3 Variable")]
    public class V3Variable : ModifiableEditorVariable
    {
        protected override string title => nameof(V3Variable);
        
        [SerializeField]
        public Vector3 _value;

        public Vector3 value
        {
            get => _value;
            private set => _value = value;
        }
        
        [SerializeField]
        [ShowIf(nameof(hasDefault))]
        private Vector3 _defaultValue;

        public Vector3 defaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }
        
        public void SetValue(GameObject callingObject, Vector3 value)
        {
            StoreCaller(callingObject);

            this.value = value;

            SignalChange();
        }

        public void SetValue(Vector3 value)
        {
            if (CallerRegistered() == false) return;

            this.value = value;

            SignalChange();
        }

        public void SetValue(V3Variable value)
        {
            if (CallerRegistered() == false) return;

            this.value = value.value;

            SignalChange();
        }

        public void ApplyChange(Vector3 amount)
        {
            if (CallerRegistered() == false) return;

            value += amount;

            SignalChange();
        }

        public void ApplyChange(V3Variable amount)
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