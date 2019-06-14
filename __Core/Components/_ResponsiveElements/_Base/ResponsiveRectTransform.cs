using UnityEngine;

namespace AltSalt
{
    public class ResponsiveRectTransform : ResponsiveElement
    {
        bool transformStored = false;
        protected RectTransform rectTransform;

        void StoreTransform()
        {
            if (transformStored == false) {
                rectTransform = GetComponent<RectTransform>();
                transformStored = true;
            }
        }

        public override void ExecuteResponsiveAction()
        {
            base.ExecuteResponsiveAction();
            StoreTransform();
        }

    }    
}