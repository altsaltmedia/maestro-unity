using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro
{   
    [CreateAssetMenu(menuName = "Maestro/Variables/Axis Variable")]
	public class Axis : SimpleSignal
    {
        protected override string title => nameof(Axis);

        [SerializeField]
        private AxisType _axisType;

        public AxisType axisType
        {
            get => _axisType;
            private set => _axisType = value;
        }

        [SerializeField]
        private bool _active;

        public bool active
        {
            get => _active;
            private set => _active = value;
        }

        [SerializeField]
        private bool _inverted;

        public bool inverted
        {
            get => _inverted;
            private set => _inverted = value;
        }

        // This should ONLY be called when being created as part of an input group
        public Axis SetAxisType(InputData inputData, AxisType targetAxisType)
        {
            Debug.Log("Setting axis type via call from " + inputData.name);
            
            axisType = targetAxisType;

            return this;
        }

        public void SetStatus(bool targetValue)
        {
            if (CallerRegistered() == false) return;

            active = targetValue;
            
            SignalChange();
        }

        public void SetInverted(bool targetValue)
        {
            if (CallerRegistered() == false) return;

            inverted = targetValue;
            
            SignalChange();
        }
        
    }

}