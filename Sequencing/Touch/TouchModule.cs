using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Sequencing.Touch
{
    public class TouchModule : InputModule
    {
        [SerializeField]
        [Required]
        private TouchController _touchController;

        protected TouchController touchController
        {
            get => _touchController;
            set => _touchController = value;
        }

        protected bool _isparentModuleNull;
        
        protected virtual void Start()
        {
            _isparentModuleNull = touchController == null;
        }
    }
}