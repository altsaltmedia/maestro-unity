using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt.Maestro.Layout
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(BoxCollider2D))]
    public class ResizableBoxCollider2D : ResizableCollider
    {
        protected BoxCollider2D boxCollider;

        protected override void GetCollider()
        {
            if (boxCollider == null) {
                boxCollider = GetComponent<BoxCollider2D>();
            }
        }

        protected override void ResizeCollider()
        {
            Vector2 newSize = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y);
            boxCollider.size = newSize;
        }

        private void Reset()
        {
            GetCollider();
            boxCollider.offset = new Vector2(0, 0);
        }

    }
}