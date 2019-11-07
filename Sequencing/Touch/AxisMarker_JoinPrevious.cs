using UnityEngine;

namespace AltSalt.Sequencing.Touch
{
    public class AxisMarker_JoinPrevious : AxisMarker, JoinMarker_IJoinSequence
    {
        [SerializeField]
        private Sequence _previousSequence;

        public JoinerDestination joinDestination => _previousSequence;
        
        public override MarkerPlacement markerPlacement => MarkerPlacement.StartOfSequence;
    }
}