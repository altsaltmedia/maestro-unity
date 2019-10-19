using UnityEngine;
using UnityEngine.Timeline;

namespace AltSalt.Sequencing.Autorun
{
    [CustomStyle(nameof(AutoplayStart))]
    public class AutoplayStart : InputMarker, IVideoConfigurator
    {
        [SerializeField]
        private bool _isVideoSequence;

        public bool isVideoSequence
        {
            get => _isVideoSequence;
        }
    }
}