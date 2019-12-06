using AltSalt.Maestro.Sequencing.Navigate;
using AltSalt.Maestro.Sequencing.Touch;
using UnityEngine;
using UnityEngine.Timeline;

namespace AltSalt.Maestro.Sequencing
{
    [HideInMenu]
    public class ForkMarker : JoinMarker, IMarkerDescription, JoinMarker_IJoinSequence
    {
        
        [SerializeField]
        private Fork _fork;

        public JoinerDestination joinDestination => _fork;
    }
}