using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Logic
{
    [ExecuteInEditMode]
    public class FloatVariableUtils : MonoBehaviour
    {
        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private FloatVariable _outputFloatVariable;

        private FloatVariable outputFloatVariable
        {
            get => _outputFloatVariable;
            set => _outputFloatVariable = value;
        }
        
        [SerializeField]
        private float _maxValue;

        public float maxValue
        {
            get => _maxValue;
            set => _maxValue = value;
        }

        [SerializeField]
        private float _minValue;

        public float minValue
        {
            get => _minValue;
            set => _minValue = value;
        }
        
        public void SetToRandomRange()
        {
            if (MaxMinValid() == false) return;

            outputFloatVariable.SetValue(Random.Range(minValue, maxValue));
        }
        
        public void Normalize()
        {
            if (MaxMinValid() == false || OutputVariablePopulated() == false) return;
            
            outputFloatVariable.SetValue(Utils.NormalizeFloat(outputFloatVariable.value, minValue, maxValue));
        }
        
        public void SetToNormalizedX(V2Variable targetV2Variable)
        {
            if (MaxMinValid() == false || OutputVariablePopulated() == false) return;

            outputFloatVariable.SetValue(Utils.NormalizeFloat(targetV2Variable.value.x, minValue, maxValue));
        }
        
        public void SetToNormalizedY(V2Variable targetV2Variable)
        {
            if (MaxMinValid() == false || OutputVariablePopulated() == false) return;

            outputFloatVariable.SetValue(Utils.NormalizeFloat(targetV2Variable.value.y, minValue, maxValue));
        }

        private bool MaxMinValid()
        {
            if (maxValue <= minValue) {
                Debug.Log("Invalid max and min values", this);
                return false;
            }

            return true;
        }

        private bool OutputVariablePopulated()
        {
            if (outputFloatVariable == null) {
                Debug.Log("Unable to normalize - you must specify an output float variable, or choose a different method", this);
                return false;
            }

            return true;
        }

        private static bool IsPopulated(FloatVariable attribute)
        {
            if (attribute == null) {
                return false;
            }

            return true;
        }
    }
}