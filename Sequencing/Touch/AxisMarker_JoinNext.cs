using UnityEngine;
using UnityEngine.Timeline;

namespace AltSalt.Maestro.Sequencing.Touch
{
    public class AxisMarker_JoinNext : AxisMarker, IMarker, JoinMarker_IJoinSequence
    {
        [SerializeField]
        private Sequence _nextSequence;

        public JoinerDestination joinDestination => _nextSequence;

        public override MarkerPlacement markerPlacement => MarkerPlacement.EndOfSequence;
    }
}