using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Sequencing.Autorun
{
    public abstract class Autorun_Module : Input_Module
    {
        [Required]
        [SerializeField]
        private Autorun_Controller _autorunController;

        public Autorun_Controller autorunController => _autorunController;

        protected float autorunThreshold => autorunController.appSettings.GetAutorunThreshold(this.gameObject, autorunController.inputGroupKey);

        protected override Input_Controller inputController => autorunController;
    }
}