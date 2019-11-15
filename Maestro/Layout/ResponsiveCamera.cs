using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt.Maestro.Layout
{
    public class ResponsiveCamera : ResponsiveElement
    {
        protected Camera thisCamera;

        protected override void OnEnable()
        {
            base.OnEnable();
            thisCamera = GetComponent<Camera>();
        }

        void StoreCamera()
        {
            if (thisCamera == null) {
                thisCamera = GetComponent<Camera>();
            }
        }

        public override void ExecuteResponsiveAction()
        {
            base.ExecuteResponsiveAction();
            StoreCamera();
        }
    }
}