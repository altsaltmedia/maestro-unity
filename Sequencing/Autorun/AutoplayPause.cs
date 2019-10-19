using UnityEngine;
using UnityEngine.Timeline;

namespace AltSalt.Sequencing.Autorun
{
    [CustomStyle(nameof(AutoplayPause))]
    public class AutoplayPause : InputMarker, IVideoConfigurator {
        
        [SerializeField]
        private bool _isVideoSequence;

        public bool isVideoSequence
        {
            get => _isVideoSequence;
        }
    }
}