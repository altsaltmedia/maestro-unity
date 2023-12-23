using UnityEngine;
using UnityEditor.Timeline;
using UnityEngine.Timeline;

namespace AltSalt.Maestro
{
    [CustomTimelineEditor(typeof(TrackAsset))]
	public class CustomTrackEditor : TrackEditor
    {
        public override TrackDrawOptions GetTrackOptions(TrackAsset track, Object binding)
        {
            TrackDrawOptions drawOptions = base.GetTrackOptions(track, binding);
            drawOptions.minimumHeight = 14f;
            return drawOptions;
        }
    }
}
