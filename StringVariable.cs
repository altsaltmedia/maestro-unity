using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro
{
    [CreateAssetMenu(menuName = "AltSalt/Variables/String Variable")]
    public class StringVariable : ModifiableEditorVariable
    {
        protected override string title => nameof(StringVariable);
        
        [SerializeField]
        private string _value;

        public string value
        {
            get => _value;
            set => _value = value;
        }
        
        [SerializeField]
        [ShowIf(nameof(hasDefault))]
        private string _defaultValue;

        public string defaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }
        
        public void SetValue(string value)
        {
            this.value = value;
        }

        public void SetValue(StringVariable value)
        {
            this.value = value.value;
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