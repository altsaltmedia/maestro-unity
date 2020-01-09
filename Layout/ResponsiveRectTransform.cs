using UnityEngine;

namespace AltSalt.Maestro.Layout
{
    public class ResponsiveRectTransform : ResponsiveLayoutElement
    {
        protected RectTransform rectTransform;

        protected override void OnEnable()
        {
            base.OnEnable();
            StoreTransform();
        }

        void StoreTransform()
        {
            if (rectTransform == null) {
                rectTransform = GetComponent<RectTransform>();
            }
        }

        public override void ExecuteResponsiveAction()
        {
            base.ExecuteResponsiveAction();
            StoreTransform();
        }

    }    
}