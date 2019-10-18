using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Sequencing.Autorun
{
    public abstract class AutorunModule : InputModule
    {
        [SerializeField]
        private AutorunController _autorunController;

        protected AutorunController autorunController
        {
            get => _autorunController;
            set => _autorunController = value;
        }
        
        [ValidateInput("IsPopulated")]
        private BoolReference _isReversing;

        protected bool isReversing
        {
            get => _isReversing.Value;
        }
        
        protected bool _isparentModuleNull;
        
        protected virtual void Start()
        {
            _isparentModuleNull = autorunController == null;
        }
    }
}