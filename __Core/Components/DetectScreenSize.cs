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
            screenWidth.Variable.SetValue(Screen.safeArea.width);
            screenHeight.Variable.SetValue(Screen.safeArea.height);
            aspectRatio.Variable.SetValue((float)Screen.safeArea.height / Screen.safeArea.width);
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
                if (Mathf.Approximately(screenWidth.Value, Screen.safeArea.width) == false ||
                    Mathf.Approximately(screenHeight.Value, Screen.safeArea.height) == false) {
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