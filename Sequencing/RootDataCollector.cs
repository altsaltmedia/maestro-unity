using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Sequencing
{
    public abstract class RootDataCollector : MonoBehaviour
    {
        [SerializeField]
        [FormerlySerializedAs("appSettings")]
        private AppSettings _appSettings;

        public AppSettings appSettings
        {
            get
            {
                if (_appSettings == null) {
                    _appSettings = Utils.GetAppSettings();
                }

                return _appSettings;
            }
            set => _appSettings = value;
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