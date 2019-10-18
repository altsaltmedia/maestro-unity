using UnityEngine;
using UnityEngine.Timeline;

namespace AltSalt
{
    [CustomStyle(nameof(AutoplayStart))]
    public class AutoplayStart : SequenceConfigMarker, IVideoConfigurator
    {
        [SerializeField]
        private bool _isVideoSequence;

        public bool isVideoSequence
        {
            get => _isVideoSequence;
        }
    }
}