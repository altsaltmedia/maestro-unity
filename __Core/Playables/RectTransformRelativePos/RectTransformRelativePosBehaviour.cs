/***********************************************

Copyright © 2018 AltSalt Media, LLC.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System;
using UnityEngine;

namespace AltSalt
{
    [Serializable]
    public class RectTransformRelativePosBehaviour : LerpToTargetBehaviour
    {
        public GameObject originReferenceObject;
        public GameObject targetReferenceObject;
        public Vector3 offsetVector = new Vector3(0,0,5);
    }
}