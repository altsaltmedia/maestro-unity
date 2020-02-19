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
            if(gameObject.TryGetComponent(typeof(TimelineInstanceConfig), out Component component) == false) {
                gameObject.AddComponent(typeof(TimelineInstanceConfig));
            }
            
            TimelineInstanceConfig timelineInstanceConfig = component as TimelineInstanceConfig;
            behaviour.timelineInstanceConfig = timelineInstanceConfig;
            
            if (Application.isPlaying == false && TimelineEditor.inspectedDirector != null) {
                TimelineEditor.inspectedDirector.time = timelineInstanceConfig.timelineDebugTime;
            }
#endif
            return trackPlayable;
        }
    }
}