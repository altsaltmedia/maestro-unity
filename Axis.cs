using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro
{   
    [CreateAssetMenu(menuName = "AltSalt/Touch/Axis")]
	public class Axis : SimpleSignal
    {
        protected override string title => nameof(Axis);

        [SerializeField]
        private AxisType _axisType;

        public AxisType axisType => _axisType;

        [SerializeField]
        private bool _active;

        public bool active
        {
            get => _active;
            set => _active = value;
        }

        [SerializeField]
        private bool _inverted;

        public bool inverted
        {
            get => _inverted;
            set => _inverted = value;
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