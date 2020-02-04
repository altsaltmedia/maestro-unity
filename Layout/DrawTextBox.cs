using UnityEngine;
using TMPro;

namespace AltSalt.Maestro.Layout
{
    [RequireComponent(typeof(TextMeshPro))]
    public class DrawTextBox : DrawBoundingBox
    {
        [SerializeField]
        protected Color textBoxColor = new Color(1, 1, 1, 1);

        [SerializeField]
        bool drawTextBox;

#if UNITY_EDITOR
        protected override void OnDrawGizmos() {
            base.OnDrawGizmos();

            if(drawWhenNotSelected == true && drawTextBox == true) {
                Bounds textBounds = GetComponent<TextMeshPro>().textBounds;

                Gizmos.matrix = Matrix4x4.TRS(textBounds.center, transform.rotation, transform.lossyScale);
                Gizmos.color = textBoxColor;
                Gizmos.DrawWireCube(Vector3.zero, new Vector3(textBounds.size.x, textBounds.size.y));
            }
	    }

        protected override void OnDrawGizmosSelected() {
            base.OnDrawGizmosSelected();

            if(drawTextBox == true) {
                Bounds textBounds = GetComponent<TextMeshPro>().textBounds;

                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
                Gizmos.color = textBoxColor;
		        Gizmos.DrawWireCube (Vector3.zero, new Vector3 (textBounds.size.x, textBounds.size.y));
            }
	    }
#endif
        
    }
}