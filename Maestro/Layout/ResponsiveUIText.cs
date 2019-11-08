using UnityEngine;
using TMPro;

namespace AltSalt.Maestro
{
    public class ResponsiveUIText : ResponsiveElement
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