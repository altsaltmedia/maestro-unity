namespace AltSalt.Maestro.Sequencing
{
    public class ForkMarker_JoinNext : ForkMarker
    {
        public override MarkerPlacement markerPlacement => MarkerPlacement.EndOfSequence;
    }
}