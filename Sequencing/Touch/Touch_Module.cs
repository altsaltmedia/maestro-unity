using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Sequencing.Touch
{
    public class Touch_Module : Input_Module
    {
        [Required]
        [SerializeField]
        private Touch_Controller _touchController;

        public Touch_Controller touchController
        {
            get => _touchController;
        }

        protected bool _isparentModuleNull;
        
        protected virtual void Start()
        {
            _isparentModuleNull = touchController == null;
        }
    }
}