using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro
{
    [Serializable]
    [CreateAssetMenu(menuName = "Maestro/Data Keys/Custom Key")]
    public class CustomKey : RegisterableScriptableObject
    {
    protected virtual string title => "Custom Key";
        
#if UNITY_EDITOR
        [SerializeField]
        [Multiline]
        [Title("$"+nameof(title))]
        [InfoBox("Use custom keys to allow different components to communicate.")]
        protected string description;
#endif
    }

}
