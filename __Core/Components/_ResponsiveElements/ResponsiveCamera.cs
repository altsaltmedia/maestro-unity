using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt
{
    public class ResponsiveCamera : ResponsiveElement
    {
        protected Camera thisCamera;

        bool cameraStored = false;

        protected override void Start()
        {
            base.Start();
            thisCamera = GetComponent<Camera>();
        }

        void StoreCamera()
        {
            if (cameraStored == false) {
                thisCamera = GetComponent<Camera>();
                cameraStored = true;
            }
        }

        protected override void ExecuteResponsiveAction()
        {
            StoreCamera();
        }
    }
}