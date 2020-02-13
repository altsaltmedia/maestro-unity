using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Layout {

    public abstract class RelativeElement : MonoBehaviour {
        
        [FormerlySerializedAs("referenceObject")]
        [SerializeField]
        private RectTransform _referenceObject;

        public RectTransform referenceObject
        {
            get => _referenceObject;
            set => _referenceObject = value;
        }

        [FormerlySerializedAs("isNegative")]
        [SerializeField]
        private bool _isNegative;

        protected bool isNegative => _isNegative;

        protected RectTransform _rectTransform;

        public RectTransform rectTransform
        {
            get {
                if (_rectTransform == null) {
                    _rectTransform = GetComponent<RectTransform>();
                }

                return _rectTransform;
            }
            private set => _rectTransform = value;
        }

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            ExecuteRelativeAction();
        }
#endif

        public virtual void ExecuteRelativeAction() { }
    }
    
}
