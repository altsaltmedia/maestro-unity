using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

#if UNITY_EDITOR
using UnityEditor.Timeline;
#endif

namespace AltSalt.Maestro
{
    [TrackColor(0.245149f, 0.895372f, 0.5679245f)]
    [TrackClipType(typeof(TimelineUtilsClip))]
    public class TimelineUtilsTrack : LerpToTargetTrack
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject gameObject, int inputCount)
        {
            StoreClipProperties(gameObject);

            ScriptPlayable<TimelineUtilsMixerBehaviour> trackPlayable = ScriptPlayable<TimelineUtilsMixerBehaviour>.Create(graph, inputCount);
            TimelineUtilsMixerBehaviour behaviour = trackPlayable.GetBehaviour();
            StoreMixerProperties(gameObject, behaviour);
#if UNITY_EDITOR
            if(gameObject.TryGetComponent(typeof(TrackAssetConfig), out Component component) == false) {
                gameObject.AddComponent(typeof(TrackAssetConfig));
            }
            
            TrackAssetConfig trackAssetConfig = component as TrackAssetConfig;
            behaviour.trackAssetConfig = trackAssetConfig;
            
            if (Application.isPlaying == false && TimelineEditor.inspectedDirector != null) {
                TimelineEditor.inspectedDirector.time = trackAssetConfig.timelineDebugTime;
            }
#endif
            return trackPlayable;
        }
    }
}