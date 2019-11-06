using UnityEngine;

namespace AltSalt.Sequencing
{
    public class JoinTools_JoinPrevious : JoinTools_Marker, IJoinSequence
    {
        [SerializeField]
        private Sequence _previousSequence;

        public ScriptableObject joinDestination => _previousSequence;

        public override MarkerPlacement markerPlacement => MarkerPlacement.StartOfSequence;
    }
}