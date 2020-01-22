using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace AltSalt.Maestro.Sequencing
{
    [ExecuteInEditMode]
    public class RootConfig : MonoBehaviour
    {
        [Required]
        [SerializeField]
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

        [SerializeField]
        private InputGroupKeyReference _inputGroupKey = new InputGroupKeyReference();

        public InputGroupKey inputGroupKey => _inputGroupKey.GetVariable(this.gameObject);

        [SerializeField]
        private UserDataKeyReference _userKey;

        public UserDataKey userKey => _userKey.GetVariable(this.gameObject);
        
        public ComplexEventManualTrigger sequenceModified =>
            appSettings.GetSequenceModified(this.gameObject, inputGroupKey);

        [Required]
        [SerializeField]
        private GameObject _masterSequenceContainer;

        public GameObject masterSequenceContainer => _masterSequenceContainer;

        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        [OnValueChanged(nameof(Configure))]
        private List<MasterSequence> _masterSequences = new List<MasterSequence>();

        public List<MasterSequence> masterSequences => _masterSequences;

        [Required]
        [SerializeField]
        private Joiner _joiner;

        public Joiner joiner => _joiner;

        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        [OnValueChanged(nameof(Configure))]
        private List<RootDataCollector> _rootDataCollectors = new List<RootDataCollector>();

        public List<RootDataCollector> rootDataCollectors => _rootDataCollectors;

        private void OnEnable()
        {
            Configure();
        }

        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.4f, 1)]
        public void Configure()
        {
            for (int i = 0; i < masterSequences.Count; i++) {
                masterSequences[i].rootConfig = this;
                masterSequences[i].Init();
            }
            
            joiner.rootConfig = this;
            joiner.ConfigureData();

            for (int i = 0; i < rootDataCollectors.Count; i++) {
                rootDataCollectors[i].rootConfig = this;
                rootDataCollectors[i].ConfigureData();
            }
            
        }

        private static bool IsPopulated(ComplexEventManualTrigger attribute)
        {
            return Utils.IsPopulated(attribute);
        }
        
        private static bool IsPopulated(List<MasterSequence> attribute)
        {
            return Utils.IsPopulated(attribute);
        }

        private static bool IsPopulated(List<RootDataCollector> attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}