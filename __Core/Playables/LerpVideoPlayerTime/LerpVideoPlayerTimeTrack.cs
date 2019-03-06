using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Video;

namespace AltSalt
{
    [TrackColor(0.245149f, 0.595372f, 0.1679245f)]
    [TrackClipType(typeof(LerpVideoPlayerTimeClip))]
    [TrackBindingType(typeof(VideoPlayer))]
    public class LerpVideoPlayerTimeTrack : LerpToTargetTrack
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            StoreClipStartEndTime();
            return ScriptPlayable<LerpVideoPlayerTimeMixerBehaviour>.Create (graph, inputCount);
        }
        
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {

#if UNITY_EDITOR
            var comp = director.GetGenericBinding(this) as VideoPlayer;
            if (comp == null)
                return;
            var so = new UnityEditor.SerializedObject(comp);
            var iter = so.GetIterator();
            while (iter.NextVisible(true)) {
                if (iter.hasVisibleChildren)
                    continue;
                driver.AddFromName<VideoPlayer>(comp.gameObject, iter.propertyPath);
            }
#endif

            base.GatherProperties(director, driver);
        }
    }
    
}