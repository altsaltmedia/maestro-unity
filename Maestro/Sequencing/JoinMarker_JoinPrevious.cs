using UnityEngine;

namespace AltSalt.Maestro.Sequencing
{
    public class JoinMarker_JoinPrevious : JoinMarker, JoinMarker_IJoinSequence
    {
        [SerializeField]
        private Sequence _previousSequence;

        public JoinerDestination joinDestination => _previousSequence;

        public override MarkerPlacement markerPlacement => MarkerPlacement.StartOfSequence;
    }
}