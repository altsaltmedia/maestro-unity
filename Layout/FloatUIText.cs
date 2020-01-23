using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Layout
{
    
	public class FloatUIText : MonoBehaviour {

        // TO DO - abstract this so that it can update both float and text values if needed
        // ALSO... figure out how this script works in conjunction with localization
        [ValidateInput("IsPopulated")]
        public FloatReference floatValue = new FloatReference();

        // Fade variables
        private TextMeshProUGUI textRenderer;

		// Use this for initialization
		void Start ()
        {
            UpdateText();
        }
        
        public void UpdateText ()
        {
            if(textRenderer == null) {
                StoreTextRenderer();
            }
            SetText(floatValue.GetValue());
        }

        void SetText(float number)
        {
            textRenderer.SetText(number.ToString());
        }

        void StoreTextRenderer()
        {
            textRenderer = gameObject.GetComponent<TextMeshProUGUI>();
        }

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }
	
}