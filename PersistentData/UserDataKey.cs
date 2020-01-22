using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro
{
    [Serializable]
    [CreateAssetMenu(menuName = "AltSalt/User Data Key")]
    public class UserDataKey : CustomKey
    {
#if UNITY_EDITOR
        protected override string title => nameof(UserDataKey);
#endif
    }

}