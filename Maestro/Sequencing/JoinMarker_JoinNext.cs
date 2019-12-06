using UnityEngine;

namespace AltSalt.Maestro.Sequencing
{
    public class JoinMarker_JoinNext : JoinMarker, JoinMarker_IJoinSequence
    {
        [SerializeField]
        private JoinerDestination _nextDestination;

        public JoinerDestination joinDestination => _nextDestination;

        public override MarkerPlacement markerPlacement => MarkerPlacement.EndOfSequence;
    }
}