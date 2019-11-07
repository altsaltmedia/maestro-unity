using AltSalt.Sequencing.Navigate;
using AltSalt.Sequencing.Touch;
using UnityEngine;
using UnityEngine.Timeline;

namespace AltSalt.Sequencing
{
    [HideInMenu]
    public class ForkMarker : JoinMarker, IMarkerDescription, JoinMarker_IJoinSequence
    {
        [SerializeField]
        private string _description;

        public string description => _description;

        [SerializeField]
        private TouchFork _fork;

        public JoinerDestination joinDestination => _fork;
    }
}