using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine.Timeline;

namespace AltSalt
{
    [CustomTimelineEditor(typeof(LerpToTargetTrack))]
    public class DebugTimelineTrackEditor : TrackEditor
    {
        public override TrackDrawOptions GetTrackOptions(TrackAsset track, Object binding)
        {
            TrackDrawOptions drawOptions = base.GetTrackOptions(track, binding);
            drawOptions.minimumHeight = 25f;
            return drawOptions;
        }
    }
}
