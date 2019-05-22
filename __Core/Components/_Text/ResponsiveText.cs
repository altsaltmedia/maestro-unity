using UnityEngine;
using TMPro;

namespace AltSalt
{
    public class ResponsiveText : ResponsiveElement
    {
        bool textStored = false;
        protected TextMeshPro textMeshPro;

        void StoreText()
        {
            if (textStored == false) {
                textMeshPro = GetComponent<TextMeshPro>();
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