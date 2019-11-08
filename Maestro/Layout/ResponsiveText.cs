using UnityEngine;
using TMPro;

namespace AltSalt.Maestro
{
    public class ResponsiveText : ResponsiveElement
    {
        protected TextMeshPro textMeshPro;

        protected override void OnEnable()
        {
            base.OnEnable();
            StoreText();
        }

        void StoreText()
        {
            if (textMeshPro == null) {
                textMeshPro = GetComponent<TextMeshPro>();
            }
        }

        public override void ExecuteResponsiveAction()
        {
            base.ExecuteResponsiveAction();
            StoreText();
        }
    }
}