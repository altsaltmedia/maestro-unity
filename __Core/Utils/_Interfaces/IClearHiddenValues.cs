using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt
{
    public interface IClearHiddenValues
    {
#if UNITY_EDITOR
        void ClearHiddenValues();
#endif
    }
}