using System;
using UnityEngine;

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
    }   
}