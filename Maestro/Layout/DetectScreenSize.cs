using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro {
    
	#if UNITY_EDITOR
	[ExecuteInEditMode]
	#endif
    public class DetectScreenSize : MonoBehaviour {

        [Required]
        [SerializeField]
        AppSettings appSettings;

        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        FloatReference deviceAspectRatio;

        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        FloatReference deviceWidth;

        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        FloatReference deviceHeight;

        [Required]
        [SerializeField]
        SimpleEventTrigger screenResized;

        float internalHeightValue;

		void Start()
		{
            SaveScreenValues();
		}

        void SaveScreenValues()
        {
            deviceWidth.Variable.SetValue(Screen.width);
            deviceHeight.Variable.SetValue(Screen.height);
            deviceAspectRatio.Variable.SetValue((float)Screen.height / Screen.width);
        }

        #if UNITY_EDITOR

            void OnGUI()
            {
                if (ScreenResized() == true) {
                    SaveScreenValues();
                    screenResized.RaiseEvent(this.gameObject);
                }
            }

            public bool ScreenResized()
            {
                if (Mathf.Approximately(deviceWidth.Value, Screen.width) == false ||
                    Mathf.Approximately(deviceHeight.Value, Screen.height) == false) {
                    return true;
                }
                else {
                    return false;
                }
            }
        #endif

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
	}
	
}