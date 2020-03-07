using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Sequencing
{
    [CreateAssetMenu(menuName = "Maestro/Sequencing/Sequence")]
    public class Sequence : JoinerDestination
    {
        [SerializeField]
        [TitleGroup("$"+nameof(propertiesTitle))]
        private bool _active = false;
        
        public bool active {
            get => _active;
            set => _active = value;
        }
        
        [SerializeField]
        [TitleGroup("$"+nameof(propertiesTitle))]
        private bool _paused = false;
        
        public bool paused {
            get => _paused;
            set => _paused = value;
        }
        
        [SerializeField]
        [TitleGroup("$"+nameof(propertiesTitle))]
        private bool _canBeScrubbed = true;
        
        public bool canBeScrubbed {
            get => _canBeScrubbed;
            set => _canBeScrubbed = value;
        }
        
        [SerializeField]
        [TitleGroup("$"+nameof(propertiesTitle))]
        private double _currentTime;

        public double currentTime
        {
            get => _currentTime;
            set => _currentTime = value;
        }

        [Required]
        [SerializeField]
        [TitleGroup("$"+nameof(propertiesTitle))]
        private PlayableAsset _sourcePlayable;
        
        public PlayableAsset sourcePlayable {
            get => _sourcePlayable;
            set => _sourcePlayable = value;
        }

        [FormerlySerializedAs("_sequenceConfig"),SerializeField]
        [ReadOnly]
        [InfoBox("This value must be set at runtime via a Sequence Config component.")]
        [TitleGroup("$"+nameof(propertiesTitle))]
        private SequenceController _sequenceController;
        
        public SequenceController sequenceController {
            get => _sequenceController;
            set => _sequenceController = value;
        }

        [SerializeField]
        [TitleGroup("$"+nameof(defaultsTitle))]
        private bool _defaultStatus;

        public bool defaultStatus
        {
            get => _defaultStatus;
            set => _defaultStatus = value;
        }

        [SerializeField]
        [TitleGroup("$"+nameof(defaultsTitle))]
        private double _defaultTime;

        private double defaultTime => _defaultTime;

        private string propertiesTitle => "Properties";
        
        private string defaultsTitle => "Defaults";
        
        public void SetStatus(bool targetStatus)
        {
            active = targetStatus;
        }

        public void SetToDefaults()
        {
            active = defaultStatus;
            currentTime = defaultTime;
        }

    }
}