using AltSalt.Sequencing.Navigate;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

namespace AltSalt.Sequencing.Autorun
{
    [CustomStyle(nameof(AutorunMarker_Start))]
    public class AutorunMarker_Start : ConfigMarker, IVideoConfigurator, IMarkerDescription
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