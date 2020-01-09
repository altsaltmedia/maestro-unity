using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Layout
{

    public class FloatText : MonoBehaviour
    {

        // TO DO - abstract this so that it can update both float and text values if needed
        // ALSO... figure out how this script works in conjunction with localization
        [ValidateInput("IsPopulated")]
        public FloatReference floatValue = new FloatReference();

        [SerializeField]
        string prependText;

        // Fade variables
        private TextMeshPro textRenderer;

        // Use this for initialization
        void Start()
        {
            UpdateText();
        }

        public void UpdateText()
        {
            if (textRenderer == null) {
                StoreTextRenderer();
            }
            SetText(floatValue.GetValue(this.gameObject));
        }

        void SetText(float number)
        {
            textRenderer.SetText(prependText + number.ToString());
        }

        void StoreTextRenderer()
        {
            textRenderer = gameObject.GetComponent<TextMeshPro>();
        }

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }

}