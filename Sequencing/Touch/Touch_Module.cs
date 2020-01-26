using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Sequencing.Touch
{
    public class Touch_Module : Input_Module
    {
        [Required]
        [SerializeField]
        private Touch_Controller _touchController;

        public Touch_Controller touchController => _touchController;

        protected override Input_Controller inputController => touchController;
        
    }
}