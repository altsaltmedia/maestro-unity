using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Sequencing
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

        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        private List<MasterSequence> _masterSequences = new List<MasterSequence>();

        public List<MasterSequence> masterSequences
        {
            get => _masterSequences;
        }

        [Required]
        [SerializeField]
        private JoinTools _joinTools;

        public JoinTools joinTools
        {
            get => _joinTools;
        }

        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        private List<Input_RootDataCollector> _rootDataCollectors = new List<Input_RootDataCollector>();

        public List<Input_RootDataCollector> rootDataCollectors
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
            
            joinTools.rootConfig = this;
            joinTools.ConfigureData();

            for (int i = 0; i < rootDataCollectors.Count; i++) {
                rootDataCollectors[i].rootConfig = this;
                rootDataCollectors[i].ConfigureData();
            }
            
        }

        private static bool IsPopulated(List<MasterSequence> attribute)
        {
            return Utils.IsPopulated(attribute);
        }

        private static bool IsPopulated(List<Input_RootDataCollector> attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}