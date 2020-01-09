using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Animation
{
    public class PostProcessing : MonoBehaviour
    {
        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private BoolReference _isFlicked = new BoolReference();

        private bool isFlicked => _isFlicked.GetValue(this.gameObject);

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private BoolReference _isReversing = new BoolReference();

        private bool isReversing => _isReversing.GetValue(this.gameObject);

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private FloatReference _currentTimescale = new FloatReference();

        private FloatReference currentTimescale => _currentTimescale;

        [SerializeField]
        [Required]
        private SimpleEventTrigger _timescaleChanged = new SimpleEventTrigger();

        private SimpleEventTrigger timescaleChanged => _timescaleChanged;

        [SerializeField]
        [RangeAttribute(0, 5)]
        private float _duration;

        public float duration
        {
            get => _duration;
            set => _duration = value;
        }

        [SerializeField]
        [RangeAttribute(0, 1)]
        private float[] _strength = { .15f, .2f, .3f, .4f} ;

        public float[] strength => _strength;

        private float[] _timescales = { 1f, 1.5f, 2f, 2.5f};

        public float[] timescales => _timescales;

        private int timescaleIndex = 5;

        private bool _previousSwipeNegative = false;

        private bool previousSwipeNegative
        {
            get => _previousSwipeNegative;
            set => _previousSwipeNegative = value;
        }

        public void UpdateTimescale()
        {
            if(isFlicked == false) {
                return;
            }

            if(isReversing == previousSwipeNegative) {
                timescaleIndex++;
            } else {
                timescaleIndex = 1;
            }

            if(timescaleIndex >= timescales.Length) {
                timescaleIndex = timescales.Length - 1;
            }

            currentTimescale.variable.SetValue(isReversing == false ? timescales[timescaleIndex] : timescales[timescaleIndex] * -1);

            Time.timeScale = Mathf.Abs(currentTimescale.value);
            timescaleChanged.RaiseEvent(this.gameObject);
            ExecuteEffect();

            previousSwipeNegative = isReversing;
        }

        public void ResetTimescale()
        {
            timescaleIndex = 0;
            currentTimescale.variable.SetValue(timescales[timescaleIndex]);
            Time.timeScale = currentTimescale.value;
        }

        public void ExecuteEffect ()
        {
            //CameraPlay.Radial(0.5f, 0.5f, duration, strength[timescaleIndex]);
        }

        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}