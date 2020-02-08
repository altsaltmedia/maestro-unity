using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Timeline;

namespace AltSalt.Maestro.Sequencing
{
    [HideInMenu]
    public class JoinMarker : ConfigMarker
    {
        [SerializeField]
        private string _description;

        public string description => _description;
        
        public virtual MarkerPlacement markerPlacement => MarkerPlacement.Manual;

        // public override void OnInitialize(TrackAsset aPent)
        // {
        //     base.OnInitialize(aPent);
        //     ModifyMarkerTime(this, aPent);
        // }

        private static void ModifyMarkerTime(JoinMarker joinMarker, TrackAsset aPent)
        {
            if (joinMarker.markerPlacement == MarkerPlacement.StartOfSequence) {
                joinMarker.time = 0d;
            } else if(joinMarker.markerPlacement == MarkerPlacement.EndOfSequence) {
                joinMarker.time = aPent.timelineAsset.duration;
            }
        }
    }
}