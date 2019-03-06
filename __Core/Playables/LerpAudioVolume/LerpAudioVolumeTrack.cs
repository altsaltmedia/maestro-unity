using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Video;

namespace AltSalt
{
    [TrackColor(0.245149f, 0.595372f, 0.1679245f)]
    [TrackClipType(typeof(LerpAudioVolumeClip))]
    [TrackBindingType(typeof(AudioSource))]
    public class LerpAudioVolumeTrack : LerpToTargetTrack
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            StoreClipStartEndTime();
            return ScriptPlayable<LerpAudioVolumeMixerBehaviour>.Create (graph, inputCount);
        }
        
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {

//#if UNITY_EDITOR
//            var comp = director.GetGenericBinding(this) as AudioSource;
//            if (comp == null)
//                return;
//            var so = new UnityEditor.SerializedObject(comp);
//            var iter = so.GetIterator();
//            while (iter.NextVisible(true)) {
//                if (iter.hasVisibleChildren)
//                    continue;
//                driver.AddFromName<AudioSource>(comp.gameObject, iter.propertyPath);
//            }
//#endif

            base.GatherProperties(director, driver);
        }
    }
    
}