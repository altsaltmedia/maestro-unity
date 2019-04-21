using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt {
    
	#if UNITY_EDITOR
	[ExecuteInEditMode]
	#endif
    public class DetectScreenSize : MonoBehaviour {

        [Required]
        public AppSettings appSettings;

        [ValidateInput("IsPopulated")]
        public FloatReference screenWidth;

        [ValidateInput("IsPopulated")]
        public FloatReference screenHeight;

        [ValidateInput("IsPopulated")]
        public FloatReference aspectRatio;

        [Required]
        public SimpleEvent screenResized;

        float internalHeightValue;

		void Start()
		{
            SaveScreenValues();
		}

        void SaveScreenValues()
        {
            if(appSettings.pillarBoxingEnabled == true) {
                screenWidth.Variable.SetValue(Screen.safeArea.width);
                aspectRatio.Variable.SetValue((float)Screen.safeArea.height / Screen.safeArea.width);
                if(aspectRatio < 1.78f) {
                    screenHeight.Variable.SetValue(Screen.safeArea.height);
                } else {
                    screenHeight.Variable.SetValue((16 * screenWidth.Value) / 9);
                }
                internalHeightValue = screenHeight.Value;
            } else {
                screenWidth.Variable.SetValue(Screen.width);
                screenHeight.Variable.SetValue(Screen.height);
                aspectRatio.Variable.SetValue((float)Screen.height / Screen.width);
            }
        }

        #if UNITY_EDITOR

            void OnGUI()
            {
                if (ScreenResized() == true) {
                    SaveScreenValues();
                    screenResized.Raise();
                }
            }

            public bool ScreenResized()
            {
                if (appSettings.pillarBoxingEnabled == true) {
                    if (Mathf.Approximately(screenWidth.Value, Screen.safeArea.width) == false ||
                        Mathf.Approximately(screenHeight.Value, internalHeightValue) == false) {
                        return true;
                    }
                    else {
                        return false;
                    }
                } else {
                    if (Mathf.Approximately(screenWidth.Value, Screen.width) == false ||
                        Mathf.Approximately(screenHeight.Value, Screen.height) == false) {
                        return true;
                    }
                    else {
                        return false;
                    }
                }
            }
        #endif

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
	}
	
}