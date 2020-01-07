using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt.Maestro
{
    public abstract class RegisterablePlayableAsset : PlayableAsset
    {
        [SerializeField]
        [Description(FontStyle.Italic, TextAnchor.UpperCenter)]
        string registrationNotice = "This object and its dependents will be registered whenever the RegisterDependencies tool is used.";
    }
}