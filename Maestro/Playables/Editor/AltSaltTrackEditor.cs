using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine.Timeline;

namespace AltSalt
{
    [CustomTimelineEditor(typeof(TrackAsset))]
	public class AltSaltTrackEditor : TrackEditor
    {
        public override TrackDrawOptions GetTrackOptions(TrackAsset track, Object binding)
        {
            TrackDrawOptions drawOptions = base.GetTrackOptions(track, binding);
            drawOptions.minimumHeight = 15f;
            return drawOptions;
        }
    }
}
