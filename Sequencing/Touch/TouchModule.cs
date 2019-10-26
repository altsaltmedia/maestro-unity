using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Sequencing.Touch
{
    public class TouchModule : InputModule
    {
        [Required]
        [SerializeField]
        private TouchController _touchController;

        public TouchController touchController
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