using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt.Maestro {

    public abstract class RelativeElement : MonoBehaviour {

        public RectTransform referenceObject;
        public bool isNegative;

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
