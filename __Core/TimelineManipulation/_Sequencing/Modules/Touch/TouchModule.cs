using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Sequencing.Touch
{
    public class TouchModule : InputModule
    {
        [SerializeField]
        private TouchController _touchController;

        protected TouchController touchController
        {
            get => _touchController;
            set => _touchController = value;
        }
        
        [SerializeField]
        [ValidateInput("IsPopulated")]
        private BoolReference _isReversing;

        protected bool isReversing
        {
            get => _isReversing.Value;
            set => _isReversing.Variable.SetValue(value);
        }
     
        protected bool _isparentModuleNull;
        
        protected virtual void Start()
        {
            _isparentModuleNull = touchController == null;
        }
    }
}