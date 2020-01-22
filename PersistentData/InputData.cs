using System;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro
{
    [Serializable]
    public class InputData : ScriptableObject
    {
        [SerializeField]
        private InputGroupCollection _inputGroupCollection = new InputGroupCollection();

        private InputGroupCollection inputGroupCollection {
            get {
                
                if (_inputGroupCollection == null) {
                    _inputGroupCollection = new InputGroupCollection();
                }

                return _inputGroupCollection;
            }
        }

        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void RefreshDependencies()
        {
            foreach (KeyValuePair<InputGroupKey, InputGroup> inputGroupItem in inputGroupCollection) {
                inputGroupItem.Value.RefreshDependencies(inputGroupItem.Key);
            }
        }

        public InputGroup GetInputGroup(InputGroupKey inputGroupKey)
        {
            if (inputGroupCollection.ContainsKey(inputGroupKey)) {
                return inputGroupCollection[inputGroupKey];
            }

            inputGroupCollection.Add(inputGroupKey, new InputGroup(this, inputGroupKey));
            return inputGroupCollection[inputGroupKey];
        }

        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public InputGroupCollection SetDefaults()
        {
            foreach (KeyValuePair<InputGroupKey, InputGroup> inputGroupItem in inputGroupCollection) {
                inputGroupItem.Value.SetDefaults(this, inputGroupItem.Key);
            }

            return inputGroupCollection;
        }

        [Serializable]
        public class InputGroupCollection : SerializableDictionaryBase<InputGroupKey, InputGroup> { }
    }
}