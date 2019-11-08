using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt.Maestro
{
    [ExecuteInEditMode]
    public abstract class ResizableCollider : InteractableRectTransform
    {
        [SerializeField]
        protected Color boxColor = new Color(1, 1, 1, 1);

        protected override void Start()
        {
            base.Start();
            ResizeCollider();
        }

        protected override void GetComponents()
        {
            base.GetComponents();
            GetCollider();
        }

        protected abstract void GetCollider();

        protected abstract void ResizeCollider();

#if UNITY_EDITOR
        float internalWidthValue = 0f;
        float internalHeightValue = 0f;

        void OnRenderObject()
        {
            GetComponents();
            if (MarginChanged() == true) {
                ResizeCollider();
            }
        }

        bool MarginChanged()
        {
            if (Mathf.Approximately(internalWidthValue, rectTransform.sizeDelta.x) == false ||
                Mathf.Approximately(internalHeightValue, rectTransform.sizeDelta.y) == false) {
                StoreInternalValues();
                return true;
            } else {
                return false;
            }
        }

        void StoreInternalValues()
        {
            internalWidthValue = rectTransform.sizeDelta.x;
            internalHeightValue = rectTransform.sizeDelta.y;
        }
#endif

    }

}