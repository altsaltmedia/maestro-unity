using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Sequencing.Autorun
{
    public abstract class Autorun_Module : Input_Module
    {
        [Required]
        [SerializeField]
        private Autorun_Controller _autorunController;

        protected Autorun_Controller autorunController => _autorunController;

        protected override Input_Controller inputController => autorunController;

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