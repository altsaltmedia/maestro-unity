/***********************************************

Copyright © 2018 AltSalt Media, LLC.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System;
using UnityEngine;

namespace AltSalt.Maestro.Animation
{
    [Serializable]
    public class RectTransformRelativePosBehaviour : GameObjectReferenceBehaviour
    {
        public Vector3 offsetVector = new Vector3(0,0,5);
    }
}