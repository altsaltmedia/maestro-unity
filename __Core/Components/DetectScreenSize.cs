using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt {
    
	#if UNITY_EDITOR
	[ExecuteInEditMode]
	#endif
    public class DetectScreenSize : MonoBehaviour {

        [ValidateInput("IsPopulated")]
        public FloatReference screenWidth;

        [ValidateInput("IsPopulated")]
        public FloatReference screenHeight;

        [ValidateInput("IsPopulated")]
        public FloatReference aspectRatio;

        [Required]
        public SimpleEvent screenResized;

		void Start()
		{
            SaveScreenValues();
		}

        void SaveScreenValues()
        {
            screenWidth.Variable.SetValue(Screen.width);
            screenHeight.Variable.SetValue(Screen.height);
            aspectRatio.Variable.SetValue((float)Screen.height / Screen.width);
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
                if (Mathf.Approximately(screenWidth.Value, Screen.width) == false ||
                    Mathf.Approximately(screenHeight.Value, Screen.height) == false) {
                    return true;
                } else {
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