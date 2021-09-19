using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Layout
{
    [RequireComponent(typeof(ResizableBoxCollider2D))]
    [ExecuteInEditMode]
    public class Touchable : MonoBehaviour
    {
        [FormerlySerializedAs("active")]
        [SerializeField]
        public bool _active = true;

        public bool active {
            get => _active;
            set => _active = value;
        }

        [SerializeField]
        private GameObjectGenericAction _action;

        private GameObjectGenericAction action => _action;

        
        private BoxCollider2D _boxCollider;

        private BoxCollider2D boxCollider
        {
            get
            {
                if (_boxCollider == null) {
                    _boxCollider = GetComponent<BoxCollider2D>();
                }

                return _boxCollider;
            }
            set => _boxCollider = value;
        }

        private void Awake()
        {
            boxCollider = GetComponent<BoxCollider2D>();
        }
        

        public void OnMouseDown()
        {
            // Make sure not only that we're active, but
            // that the user isn't pressing a UI element
            if(active == true && EventSystem.current.currentSelectedGameObject == null) {
                action.Invoke(this.gameObject);
            }
        }
        
        private void Start()
        {
            if (active == false) {
                boxCollider.enabled = false;
            }
        }

        public void Activate()
        {
            active = true;
            boxCollider.enabled = true;
        }
        
        public void Deactivate()
        {
            active = false;
            boxCollider.enabled = false;
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
