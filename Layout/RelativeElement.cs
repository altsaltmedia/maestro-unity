using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Layout {

    public abstract class RelativeElement : MonoBehaviour {
        
        [FormerlySerializedAs("referenceObject")]
        [SerializeField]
        [OnValueChanged(nameof(ExecuteRelativeAction))]
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

        public virtual void ExecuteRelativeAction() { }
    }
    
}
