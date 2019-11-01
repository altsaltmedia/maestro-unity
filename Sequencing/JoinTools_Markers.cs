using AltSalt.Sequencing.Touch;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Timeline;

namespace AltSalt.Sequencing
{
    
    public enum MarkerPlacement { StartOfSequence, EndOfSequence, Manual }
    
    [HideInMenu]
    public class JoinTools_Marker : Input_Marker
    {
        public virtual MarkerPlacement markerPlacement => MarkerPlacement.Manual;

        public override void OnInitialize(TrackAsset trackAsset)
        {
            base.OnInitialize(trackAsset);
            this.ModifyMarkerTime(trackAsset);
        }

        private void ModifyMarkerTime(TrackAsset trackAsset)
        {
            if (markerPlacement == MarkerPlacement.StartOfSequence) {
                time = trackAsset.start;
            } else if(markerPlacement == MarkerPlacement.EndOfSequence) {
                time = trackAsset.end;
            }
        }
    }

    interface IJoinSequence
    {
        Sequence sequenceToJoin { get; }
    }
    
    public class JoinTools_JoinPrevious : JoinTools_Marker, IJoinSequence
    {
        [SerializeField]
        private Sequence _previousSequence;

        public Sequence sequenceToJoin => _previousSequence;

        public override MarkerPlacement markerPlacement => MarkerPlacement.StartOfSequence;
    }
    
    public class JoinTools_JoinNext : JoinTools_Marker, IJoinSequence
    {
        [SerializeField]
        private Sequence _nextSequence;

        public Sequence sequenceToJoin => _nextSequence;

        public override MarkerPlacement markerPlacement => MarkerPlacement.EndOfSequence;
    }
    
    [HideInMenu]
    public class JoinTools_ForkMarker : JoinTools_Marker
    {
        [SerializeField]
        private AxisModifier_TouchFork _fork;

        public AxisModifier_TouchFork fork => _fork;
    }
    
    public class JoinTools_ForkJoinPrevious : JoinTools_ForkMarker
    {
        public override MarkerPlacement markerPlacement => MarkerPlacement.StartOfSequence;
    }

    public class JoinTools_ForkJoinNext : JoinTools_ForkMarker
    {
        public override MarkerPlacement markerPlacement => MarkerPlacement.EndOfSequence;
    }
    
}