using System;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

namespace AltSalt.Maestro
{
    [Serializable]
    public class InputData : ScriptableObject
    {
        [SerializeField]
        private InputGroupCollection _inputGroupCollection = new InputGroupCollection();

        private InputGroupCollection inputGroupCollection => _inputGroupCollection;
        
        public InputGroup GetInputGroup(CustomKey inputGroupKey)
        {
            if (inputGroupCollection.ContainsKey(inputGroupKey)) {
                return inputGroupCollection[inputGroupKey];
            }
            else {
                inputGroupCollection.Add(inputGroupKey, new InputGroup(inputGroupKey));
                return inputGroupCollection[inputGroupKey];
            }
        }

        [Serializable]
        public class InputGroupCollection : SerializableDictionaryBase<CustomKey, InputGroup> { }
    }
}