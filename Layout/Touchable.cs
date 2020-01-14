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
        bool _active = true;
        
        public bool active {
            get => _active;
            set => _active = value;
        }

        public UnityEvent unityEvent;

        [SerializeField]
        private GameObjectGenericAction _action;

        private GameObjectGenericAction action
        {
            get => _action;
            set => _action = value;
        }

        [SerializeField]
        private bool _migrated;

        private bool migrated
        {
            get => _migrated;
            set => _migrated = value;
        }

        public void OnPointerClick(PointerEventData pointerEventData)
        {
            if(active == true) {
                action.Invoke(this.gameObject);
            }
        }

        private void OnEnable()
        {
            if (migrated == false) {
                UnityEventUtils.MigrateUnityEventList(nameof(unityEvent), 
                    nameof(_action), new SerializedObject(this));
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
