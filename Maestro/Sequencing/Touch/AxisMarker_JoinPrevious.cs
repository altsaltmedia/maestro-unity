using UnityEngine;

namespace AltSalt.Maestro.Sequencing.Touch
{
    public class AxisMarker_JoinPrevious : AxisMarker, JoinMarker_IJoinSequence
    {
        [SerializeField]
        private JoinerDestination _previousDestination;

        public JoinerDestination joinDestination => _previousDestination;
        
        public override MarkerPlacement markerPlacement => MarkerPlacement.StartOfSequence;
    }
}