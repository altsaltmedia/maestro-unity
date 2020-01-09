using UnityEngine;
using TMPro;

namespace AltSalt.Maestro.Layout
{
    public class ResponsiveUIText : ResponsiveLayoutElement
    {
        bool textStored = false;
        protected TextMeshProUGUI textMeshProUGUI;

        void StoreText()
        {
            if (textStored == false) {
                textMeshProUGUI = GetComponent<TextMeshProUGUI>();
                textStored = true;
            }
        }

        public override void ExecuteResponsiveAction()
        {
            base.ExecuteResponsiveAction();
            StoreText();
        }
    }
}