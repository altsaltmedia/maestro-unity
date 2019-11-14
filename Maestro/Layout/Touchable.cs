using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace AltSalt.Maestro
{
    [RequireComponent(typeof(ResizableBoxCollider2D))]
    public class Touchable : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        bool active = true;
        public bool Active {
            get {
                return active;
            }
            set {
                active = value;
            }
        }

        public UnityEvent unityEvent;

        public void OnPointerClick(PointerEventData pointerEventData)
        {
            if(active == true) {
                unityEvent.Invoke();
            }
        }
    }
}
