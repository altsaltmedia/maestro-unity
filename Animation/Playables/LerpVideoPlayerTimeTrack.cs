using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Video;

namespace AltSalt.Maestro.Animation
{
    [TrackColor(0.245149f, 0.595372f, 0.1679245f)]
    [TrackClipType(typeof(LerpVideoPlayerTimeClip))]
    [TrackBindingType(typeof(PlayableVideoPlayerController))]
    public class LerpVideoPlayerTimeTrack : LerpToTargetTrack
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            StoreClipProperties(go);
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
                driver.AddFromName<PlayableVideoPlayerController>(comp.gameObject, iter.propertyPath);
            }
#endif

            base.GatherProperties(director, driver);
        }
    }
    
}