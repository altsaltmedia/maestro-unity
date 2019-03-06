using UnityEngine;
using TMPro;

namespace AltSalt
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

        protected override void ExecuteResponsiveAction()
        {
            StoreText();
        }
    }
}