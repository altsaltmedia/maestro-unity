using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Sequencing.Touch
{
    public class InvertSwitchBehaviour : LerpToTargetBehaviour
    {
        [SerializeField]
        private InvertSwitchType _switchType;

        public InvertSwitchType switchType
        {
            get => _switchType;
            set => _switchType = value;
        }

    }
}