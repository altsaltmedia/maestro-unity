using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AltSalt.Maestro
{
    public abstract class PopulateClips
    {
        public abstract PlayableAsset PopulateClip(PlayableDirector targetDirector, TrackAsset parentTrack,
            EasingFunction.Ease easeType, TimelineClip timelineClip);
    }
}