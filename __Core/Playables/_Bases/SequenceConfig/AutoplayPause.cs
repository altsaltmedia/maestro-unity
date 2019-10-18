using UnityEngine;
using UnityEngine.Timeline;

namespace AltSalt
{
    [CustomStyle(nameof(AutoplayPause))]
    public class AutoplayPause : SequenceConfigMarker, IVideoConfigurator {
        
        [SerializeField]
        private bool _isVideoSequence;

        public bool isVideoSequence
        {
            get => _isVideoSequence;
        }
    }
}