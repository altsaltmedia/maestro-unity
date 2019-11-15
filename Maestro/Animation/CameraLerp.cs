using UnityEngine;

namespace  AltSalt.Maestro.Animation
{
    
    [ExecuteInEditMode]
    public class CameraLerp : MonoBehaviour {

        RectTransform rectTransform;
        Vector3 vector3;

        void Start()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void LerpToTarget (Vector3 targetVector) {
            rectTransform.position = Vector3.SmoothDamp(rectTransform.position, targetVector, ref vector3, Time.deltaTime);
        }
    }

}
    