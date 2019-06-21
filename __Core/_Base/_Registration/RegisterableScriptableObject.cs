using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AltSalt
{
    [Serializable]
    public abstract class RegisterableScriptableObject : ScriptableObject
    {
        [SerializeField]
        [Description(FontStyle.Italic, TextAnchor.UpperCenter)]
        string registrationNotice = "This object and its dependents will be registered whenever the RegisterDependencies tool is used.";
    }
}