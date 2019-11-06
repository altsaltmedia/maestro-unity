using UnityEngine;

namespace AltSalt.Sequencing
{
    public class JoinTools_JoinNext : JoinTools_Marker, IJoinSequence
    {
        [SerializeField]
        private Sequence _nextSequence;

        public ScriptableObject joinDestination => _nextSequence;

        public override MarkerPlacement markerPlacement => MarkerPlacement.EndOfSequence;
    }
}