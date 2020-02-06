using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Video;

namespace AltSalt.Maestro.Audio
{
    [TrackColor(0.245149f, 0.595372f, 0.1679245f)]
    [TrackClipType(typeof(AudioForwardReverseClip))]
    [TrackBindingType(typeof(AudioSource))]
    public class AudioForwardReverseTrack : LerpToTargetTrack
    {
        protected override void StoreClipProperties(GameObject directorObject)
        {
            foreach (var clip in GetClips()) {
                var myAsset = clip.asset as AudioForwardReverseClip;
                if (myAsset) {
                    myAsset.startTime = clip.start;
                    myAsset.endTime = clip.end;
                    myAsset.parentTrack = this;
                    myAsset.trackAssetConfig = directorObject.GetComponent<TrackAssetConfig>();
                    myAsset.isReversingVariable = myAsset.trackAssetConfig.isReversingVariable;
                }
            }
        }

        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject directorObject, int inputCount)
        {
            StoreClipProperties(directorObject);
            ScriptPlayable<AudioForwardReverseMixerBehaviour> trackPlayable = ScriptPlayable<AudioForwardReverseMixerBehaviour>.Create(graph, inputCount);
            AudioForwardReverseMixerBehaviour behaviour = trackPlayable.GetBehaviour();
            StoreMixerProperties(directorObject, behaviour);
            return ScriptPlayable<AudioForwardReverseMixerBehaviour>.Create(graph, inputCount);
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