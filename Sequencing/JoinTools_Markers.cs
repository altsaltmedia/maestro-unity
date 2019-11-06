using AltSalt.Sequencing.Navigate;
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
                time = 0d;
            } else if(markerPlacement == MarkerPlacement.EndOfSequence) {
                time = trackAsset.timelineAsset.duration;
            }
        }
    }

    interface IJoinSequence
    {
        ScriptableObject joinDestination { get; }
    }

    [HideInMenu]
    public class JoinTools_ForkMarker : JoinTools_Marker, IMarkerDescription, IJoinSequence
    {
        [SerializeField]
        private string _description;

        public string description => _description;

        [SerializeField]
        private AxisModifier_TouchFork _fork;

        public ScriptableObject joinDestination => _fork;
    }
}