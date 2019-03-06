﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt {

    public abstract class RelativeElement : MonoBehaviour {

        public RectTransform referenceObject;
        public bool isNegative;

        protected RectTransform rectTransform;

        protected void Start()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        void OnValidate()
        {
            if(rectTransform != null) {
                ExecuteRelativeAction();
            }
        }

        public abstract void ExecuteRelativeAction();
    }
    
}
