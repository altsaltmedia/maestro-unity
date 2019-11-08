using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro
{
    [Serializable]
    [CreateAssetMenu(menuName = "AltSalt/Custom Key")]
    public class CustomKey : ScriptableObject
    {
#if UNITY_EDITOR
        [SerializeField]
        [Multiline]
        [Title("$"+nameof(title))]
        [InfoBox("Use custom keys to allow different components to communicate.")]
        protected string description;

        protected virtual string title => "Custom Key";
#endif
    }

}
