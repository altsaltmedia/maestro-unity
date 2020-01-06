using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Layout {
    
	#if UNITY_EDITOR
	[ExecuteInEditMode]
	#endif
    public class DetectScreenSize : MonoBehaviour {

        [Required]
        [SerializeField]
        private AppSettings _appSettings;

        private AppSettings appSettings
        {
            get
            {
                if (_appSettings == null) {
                    _appSettings = Utils.GetAppSettings();
                }

                return _appSettings;
            }
            set => _appSettings = value;
        }

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

        private void Start()
		{
            SaveScreenValues();
		}

        private void SaveScreenValues()
        {
            // Sometimes these values are erroneous, so make sure
            // we don't use them when they do
            if (Screen.width <= 0 || Screen.height <= 0) return;
            
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