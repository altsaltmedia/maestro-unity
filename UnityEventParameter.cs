using System;

namespace AltSalt.Maestro
{
    public class UnityEventParameter
    {
        private Type _type;

        public Type type
        {
            get => _type;
            private set => _type = value;
        }

        public string typeName
        {
            get
            {
                if (_type != null) {
                    return _type.Name;
                }

                return "";
            }
        }

        private System.Object _value;

        public object value
        {
            get => _value;
            private set => _value = value;
        }

        private string _valueName;

        public string valueName
        {
            get => _valueName;
            private set => _valueName = value;
        }

        public UnityEventParameter(Type type, System.Object value, string valueName)
        {
            this.type = type;
            this.value = value;
            this.valueName = valueName;
        }
    }
}