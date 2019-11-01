using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Sequencing
{
    public abstract class Input_RootDataCollector : MonoBehaviour
    {
        public AppSettings appSettings
        {
            get => rootConfig.appSettings;
        } 
        
        public List<MasterSequence> masterSequences
        {
            get => rootConfig.masterSequences;
        } 
        
        [ReadOnly]
        [SerializeField]
        [Required]
        [InfoBox("This must be populated via a root config component")]
        private RootConfig _rootConfig;

        public RootConfig rootConfig
        {
            get => _rootConfig;
            set => _rootConfig = value;
        }

        public abstract void ConfigureData();
        
    }
}