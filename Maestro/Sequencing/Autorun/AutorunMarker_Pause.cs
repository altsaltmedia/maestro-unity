using AltSalt.Maestro.Sequencing.Navigate;
using UnityEngine;
using UnityEngine.Timeline;

namespace AltSalt.Maestro.Sequencing.Autorun
{
    [CustomStyle(nameof(AutorunMarker_Pause))]
    public class AutorunMarker_Pause : ConfigMarker, IVideoConfigurator, IMarkerDescription
    {
        [SerializeField]
        private string _description;

        public string description
        {
            get => _description;
        }
        
        [SerializeField]
        private bool _isVideoSequence;

        public bool isVideoSequence
        {
            get => _isVideoSequence;
        }
    }
}