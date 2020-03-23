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

        public void SetValue(FloatVariable targetVariable)
        {
            slider.value = targetVariable.value;
        }
    }
}