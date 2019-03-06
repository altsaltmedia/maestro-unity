using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace AltSalt
{
    
	public class FloatUIText : MonoBehaviour {

        // TO DO - abstract this so that it can update both float and text values if needed
        // ALSO... figure out how this script works in conjunction with localization
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
            SetText(floatValue.Value);
        }

        void SetText(float number)
        {
            textRenderer.SetText(number.ToString());
        }

        void StoreTextRenderer()
        {
            textRenderer = gameObject.GetComponent<TextMeshProUGUI>();
        }

	}
	
}