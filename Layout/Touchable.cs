using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Layout
{
    [RequireComponent(typeof(ResizableBoxCollider2D))]
    public class Touchable : MonoBehaviour, IPointerClickHandler
    {
        [FormerlySerializedAs("active"),SerializeField]
        bool _active = true;
        
        public bool active {
            get {
                return _active;
            }
            set {
                _active = value;
            }
        }

        public UnityEvent unityEvent;

        public void OnPointerClick(PointerEventData pointerEventData)
        {
            if(active == true) {
                unityEvent.Invoke();
            }
        }

        private void Start()
        {
            if (active == false) {
                if (TryGetComponent(typeof(BoxCollider2D), out var boxCollider) == true) {
                    (boxCollider as BoxCollider2D).enabled = false;   
                }
            }
        }

        public void Activate()
        {
            active = true;
            if (TryGetComponent(typeof(BoxCollider2D), out var boxCollider) == true) {
                (boxCollider as BoxCollider2D).enabled = true;   
            }
        }
        
        public void Deactivate()
        {
            active = false;
            if (TryGetComponent(typeof(BoxCollider2D), out var boxCollider) == true) {
                (boxCollider as BoxCollider2D).enabled = false;
            }
        }

        public void Toggle()
        {
            if (active == false) {
                Activate();
            }
            else {
                Deactivate();
            }
        }
    }
}
