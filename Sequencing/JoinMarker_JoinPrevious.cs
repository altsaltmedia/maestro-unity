using UnityEngine;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Sequencing
{
    public class JoinMarker_JoinPrevious : JoinMarker, JoinMarker_IJoinSequence
    {
        [FormerlySerializedAs("_previousSequence"),SerializeField]
        private JoinerDestination _previousDestination;

        public JoinerDestination joinDestination => _previousDestination;

        public override MarkerPlacement markerPlacement => MarkerPlacement.StartOfSequence;
    }
}