using UnityEngine;
using System.Collections;

namespace AltSalt
{
    [RequireComponent(typeof(RectTransform))]
    public class DrawBoundingBox : MonoBehaviour {

        [SerializeField]
        Color boxColor = new Color(1,1,1,1);

        [SerializeField]
        bool drawBoxWhenNotSelected;

        void OnDrawGizmos() {
            if(drawBoxWhenNotSelected == true) {
                RectTransform rectTransform = transform as RectTransform;

                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
                Gizmos.color = boxColor;
                Gizmos.DrawWireCube(Vector3.zero, new Vector3(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y));
            }
	    }

	    void OnDrawGizmosSelected() {
            RectTransform rectTransform = transform as RectTransform;

            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.color = boxColor;
		    Gizmos.DrawWireCube (Vector3.zero, new Vector3 (rectTransform.sizeDelta.x, rectTransform.sizeDelta.y));
	    }
    }
}