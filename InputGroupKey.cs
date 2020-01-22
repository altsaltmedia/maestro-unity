using System;
using UnityEngine;

namespace AltSalt.Maestro
{
    [Serializable]
    [CreateAssetMenu(menuName = "AltSalt/Input Group Key")]
    public class InputGroupKey : CustomKey
    {
#if UNITY_EDITOR
        protected override string title => nameof(InputGroupKey);        
#endif
    }
}