using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Layout
{
    [RequireComponent(typeof(ResizableBoxCollider2D))]
    [ExecuteInEditMode]
    public class Touchable : MonoBehaviour, IPointerClickHandler
    {
        [FormerlySerializedAs("active")]
        [SerializeField]
        private bool _active = true;

        private bool active {
            get => _active;
            set => _active = value;
        }

        [SerializeField]
        private GameObjectGenericAction _action;

        private GameObjectGenericAction action => _action;

        public void OnPointerClick(PointerEventData pointerEventData)
        {
            if(active == true) {
                action.Invoke(this.gameObject);
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
