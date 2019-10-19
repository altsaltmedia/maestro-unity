using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;

namespace AltSalt.Sequencing.Touch
{
    [Serializable]
    public class SimpleSwitchBehaviour : LerpToTargetBehaviour
    {
        [SerializeField]
        private SimpleSwitchType _switchType;

        public SimpleSwitchType switchType
        {
            get => _switchType;
            set => _switchType = value;
        }
        
    }
}