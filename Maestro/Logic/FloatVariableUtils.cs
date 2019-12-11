using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Logic
{
    [ExecuteInEditMode]
    public class FloatVariableUtils : MonoBehaviour
    {
        [SerializeField]
        [ValidateInput(nameof(IsPopulated), "Optional: Specify an output variable to place calculations", InfoMessageType.Warning)]
        private FloatVariable _outputFloatVariable;

        private FloatVariable outputFloatVariable
        {
            get => _outputFloatVariable;
            set => _outputFloatVariable = value;
        }

        [SerializeField]
        private bool _normalize;

        private bool normalize
        {
            get => _normalize;
            set => _normalize = value;
        }

        [SerializeField]
        private float _minValue;

        private float minValue
        {
            get => _minValue;
            set => _minValue = value;
        }

        [SerializeField]
        private float _maxValue;

        private float maxValue
        {
            get => _maxValue;
            set => _maxValue = value;
        }

        public void Normalize()
        {
            if (NormalizeValuesPopulated() == false || OutputVariablePopulated() == false) return;
            
            outputFloatVariable.SetValue(Utils.NormalizeFloat(outputFloatVariable.value, minValue, maxValue));
        }

        public void GetDistance(float floatValue)
        {
            if (OutputVariablePopulated() == false) return;
            
            outputFloatVariable.SetValue(Mathf.Abs(outputFloatVariable.value - floatValue));
        }
        
        public void GetDistance(FloatVariable floatValue)
        {
            if (OutputVariablePopulated() == false) return;
            
            outputFloatVariable.SetValue(Mathf.Abs(outputFloatVariable.value - floatValue.value));
        }

        public void Multiply(float floatValue)
        {
            if(OutputVariablePopulated() == false) return;
            
            outputFloatVariable.Multiply(floatValue);
        }
        
        public void Multiply(FloatVariable floatValue)
        {
            if(OutputVariablePopulated() == false) return;
            
            outputFloatVariable.Multiply(floatValue.value);
        }
        
        public void ClampMax(float floatValue)
        {
            if(OutputVariablePopulated() == false) return;

            if (outputFloatVariable.value > floatValue) {
                outputFloatVariable.value = floatValue;
            }
        }
        
        public void ClampMax(FloatVariable floatValue)
        {
            if(OutputVariablePopulated() == false) return;
            
            if (outputFloatVariable.value > floatValue.value) {
                outputFloatVariable.value = floatValue.value;
            }
        }
        
        public void ClampMin(float floatValue)
        {
            if(OutputVariablePopulated() == false) return;

            if (outputFloatVariable.value < floatValue) {
                outputFloatVariable.value = floatValue;
            }
        }
        
        public void ClampMin(FloatVariable floatValue)
        {
            if(OutputVariablePopulated() == false) return;
            
            if (outputFloatVariable.value < floatValue.value) {
                outputFloatVariable.value = floatValue.value;
            }
        }

        public void SetEqualToNormalizedX(V2Variable targetV2Variable)
        {
            if (NormalizeValuesPopulated() == false || OutputVariablePopulated() == false) return;

            outputFloatVariable.SetValue(Utils.NormalizeFloat(targetV2Variable.value.x, minValue, maxValue));
        }
        
        public void SetEqualToNormalizedY(V2Variable targetV2Variable)
        {
            if (NormalizeValuesPopulated() == false || OutputVariablePopulated() == false) return;

            outputFloatVariable.SetValue(Utils.NormalizeFloat(targetV2Variable.value.y, minValue, maxValue));
        }

        private bool NormalizeValuesPopulated()
        {
            if (normalize == false) {
                Debug.Log("Unable to normalize - you must specify a min and max value", this);
                return false;
            }

            if (maxValue < minValue || Mathf.Approximately(maxValue, minValue)) {
                Debug.Log("Unable to normalize - invalid max and min values", this);
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