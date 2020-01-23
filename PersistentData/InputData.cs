using System;
using System.Collections.Generic;
using System.Reflection;
using RotaryHeart.Lib.SerializableDictionary;
using Sirenix.OdinInspector;
using UnityEditor;
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

        private void OnEnable()
        {
//            foreach (KeyValuePair<InputGroupKey,InputGroup> inputGroup in inputGroupCollection) {
//                FieldInfo[] fields = typeof(InputGroup).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
//                for (int i = 0; i < fields.Length; i++) {
//                    var fieldValue = fields[i].GetValue(inputGroup.Value);
//                    if (fieldValue is ReferenceBase variableReference) {
//                        variableReference.isSystemReference = true;
//                    }
//                }
//            }
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
            EditorUtility.SetDirty(this);
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