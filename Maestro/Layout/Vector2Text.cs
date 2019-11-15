using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Layout
{

    public class Vector2Text : MonoBehaviour
    {

        // TO DO - abstract this so that it can update both float and text values if needed
        // ALSO... figure out how this script works in conjunction with localization
        [ValidateInput("IsPopulated")]
        public V2Reference vectorValue = new V2Reference();

        // Fade variables
        private TextMeshProUGUI textRenderer;

        // Use this for initialization
        void Start()
        {
            UpdateText();
        }

        public void UpdateText()
        {
            if(textRenderer == null) {
                StoreTextRenderer();
            }
            SetText(vectorValue.Value);
        }

        void SetText(Vector2 v2)
        {
            textRenderer.SetText(v2.ToString());
        }

        void StoreTextRenderer()
        {
            textRenderer = gameObject.GetComponent<TextMeshProUGUI>();
        }

        private static bool IsPopulated(V2Reference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }

}