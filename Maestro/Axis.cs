using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro
{   
    [CreateAssetMenu(menuName = "AltSalt/Touch/Axis")]
	public class Axis : ScriptableObject {

        [SerializeField]
        private AxisType _axisType;

        public AxisType axisType
        {
            get => _axisType;
            set => _axisType = value;
        }

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

        public void SetStatus(bool targetStatus)
        {
            active = targetStatus;
        }
	}

}