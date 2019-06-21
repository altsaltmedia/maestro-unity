using UnityEngine;

namespace AltSalt
{
    [CreateAssetMenu(menuName = "AltSalt/String Variable")]
    public class StringVariable : VariableBase
    {
#if UNITY_EDITOR
        [Multiline]
        [SerializeField]
        [Header("String Variable")]
        string DeveloperDescription = "";
#endif
        public string Value;

		public void SetValue(string value)
        {
            Value = value;
        }

        public void SetValue(StringVariable value)
        {
            Value = value.Value;
        }
    }
}