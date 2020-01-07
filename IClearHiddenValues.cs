using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt.Maestro
{
    public interface IClearHiddenValues
    {
#if UNITY_EDITOR
        void ClearHiddenValues();
#endif
    }
}