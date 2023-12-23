using UnityEngine;
using UnityEngine.UI;

namespace AltSalt.Maestro.Layout
{
    [RequireComponent(typeof(Slider))]
    public class SliderExtensions : MonoBehaviour
    {
        [SerializeField]
        [Multiline]
        private string _description;

        private Slider _slider;

        private Slider slider
        {
            get
            {
                if (_slider == null) {
                    _slider = GetComponentInChildren<Slider>();
                }

                return _slider;
            }
            set => _slider = value;
        }

        public void SetSliderValueToVariable(FloatVariable targetVariable)
        {
            slider.value = targetVariable.value;
        }

        public void SetVariableToSliderValue(FloatVariable targetVariable)
        {
            targetVariable.StoreCaller(this.gameObject);
            targetVariable.SetValue(slider.value);
        }
    }
}