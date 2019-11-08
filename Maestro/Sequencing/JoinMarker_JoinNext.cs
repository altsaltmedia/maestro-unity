using UnityEngine;

namespace AltSalt.Maestro.Sequencing
{
    public class JoinMarker_JoinNext : JoinMarker, JoinMarker_IJoinSequence
    {
        [SerializeField]
        private Sequence _nextSequence;

        public JoinerDestination joinDestination => _nextSequence;

        public override MarkerPlacement markerPlacement => MarkerPlacement.EndOfSequence;
    }
}