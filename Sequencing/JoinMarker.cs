using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Timeline;

namespace AltSalt.Sequencing
{
    [HideInMenu]
    public class JoinMarker : ConfigMarker
    {
        public virtual MarkerPlacement markerPlacement => MarkerPlacement.Manual;

//        This code doesn't work; likely a Unity bug 
//
//        public override void OnInitialize(TrackAsset aPent)
//        {
//            base.OnInitialize(aPent);
//            ModifyMarkerTime(this, aPent);
//        }

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