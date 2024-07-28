using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Layout
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(BoxCollider2D))]
    public class ResizableBoxCollider2D : ResizableCollider
    {
        // You can combine with HideLabelAttribute to display a message in the inspector.
        [DisplayAsString, HideLabel]
        [NonSerialized]
        [ShowInInspector]
        [PropertyOrder(0)]
        [InfoBox("This component automatically resizes a box collider based on the width and height attributes of a rectTransform. Do NOT modify the box collider dimensions directly - your changes will get erased!", InfoMessageType.Warning)]
        private string notice = "";

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