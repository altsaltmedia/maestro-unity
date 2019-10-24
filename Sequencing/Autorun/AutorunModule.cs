using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Sequencing.Autorun
{
    public abstract class AutorunModule : InputModule
    {
        [Required]
        [SerializeField]
        private AutorunController _autorunController;

        protected AutorunController autorunController
        {
            get => _autorunController;
            set => _autorunController = value;
        }

        protected bool _isparentModuleNull;
        
        protected virtual void Start()
        {
            _isparentModuleNull = autorunController == null;
        }

        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}