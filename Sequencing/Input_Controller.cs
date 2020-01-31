using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Sequencing
{
    [Serializable]
    [ExecuteInEditMode]
    public abstract class Input_Controller : RootDataCollector
    {
        public InputGroupKey inputGroupKey => rootConfig.inputGroupKey; 
        
        public AppSettings appSettings => rootConfig.appSettings;
        
        public Joiner joiner => rootConfig.joiner;

        public bool appUtilsRequested => appSettings.GetAppUtilsRequested(this.gameObject, inputGroupKey);
        
        public ComplexEventManualTrigger requestModifyToSequence =>
            appSettings.GetRequestModifyToSequence(this.gameObject, inputGroupKey);

        protected static bool IsPopulated(ComplexEventManualTrigger attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }   
}