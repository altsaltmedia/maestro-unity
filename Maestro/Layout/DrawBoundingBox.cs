using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt.Maestro
{
    [RequireComponent(typeof(RectTransform))]
    public class DrawBoundingBox : MonoBehaviour
    {
        [SerializeField]
        protected bool drawWhenNotSelected;

        [SerializeField]
        protected Color transformBoxColor = new Color(1, 1, 1, 1);

        protected virtual void OnDrawGizmos()
        {
            if (drawWhenNotSelected == true) {
                RectTransform rectTransform = transform as RectTransform;

                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
                Gizmos.color = transformBoxColor;
                Gizmos.DrawWireCube(Vector3.zero, new Vector3(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y));
            }
        }

        protected virtual void OnDrawGizmosSelected()
        {
            RectTransform rectTransform = transform as RectTransform;

            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.color = transformBoxColor;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y));
        }
    }
}
