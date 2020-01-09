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

        public bool isNegative
        {
            get => _isNegative;
            set => _isNegative = value;
        }

        protected RectTransform rectTransform;

        protected void Start()
        {
            GetRectTransform();
        }

        void GetRectTransform()
        {
            if (rectTransform == null) {
                rectTransform = GetComponent<RectTransform>();
            }
        }

        void OnValidate()
        {
            if(rectTransform != null) {
                ExecuteRelativeAction();
            }
        }

        public virtual void ExecuteRelativeAction()
        {
            GetRectTransform();
        }
    }
    
}
