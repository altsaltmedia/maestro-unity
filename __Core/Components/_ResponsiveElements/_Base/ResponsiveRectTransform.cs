using UnityEngine;

namespace AltSalt
{
    public class ResponsiveRectTransform : ResponsiveElement
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