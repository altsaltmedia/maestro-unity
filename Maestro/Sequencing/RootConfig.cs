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
            get => _appSettings;
        }
        
        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        private ComplexEventTrigger _sequenceModified;

        public ComplexEventTrigger sequenceModified => _sequenceModified;

        [Required]
        [SerializeField]
        private GameObject _masterSequenceContainer;

        public GameObject masterSequenceContainer
        {
            get => _masterSequenceContainer;
            set => _masterSequenceContainer = value;
        }

        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        [OnValueChanged(nameof(Configure))]
        private List<MasterSequence> _masterSequences = new List<MasterSequence>();

        public List<MasterSequence> masterSequences
        {
            get => _masterSequences;
        }

        [Required]
        [SerializeField]
        private Joiner _joiner;

        public Joiner joiner
        {
            get => _joiner;
        }

        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        [OnValueChanged(nameof(Configure))]
        private List<RootDataCollector> _rootDataCollectors = new List<RootDataCollector>();

        public List<RootDataCollector> rootDataCollectors
        {
            get => _rootDataCollectors;
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

        private static bool IsPopulated(ComplexEventTrigger attribute)
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