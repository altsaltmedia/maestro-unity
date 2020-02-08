using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AltSalt.Maestro
{
    [TrackColor(0.245149f, 0.895372f, 0.5679245f)]
    [TrackClipType(typeof(ComplexEventTimelineTriggerClip))]
    public class ComplexEventTimelineTriggerTrack : TimelineTriggerTrack
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            StoreClipProperties(go);
            ScriptPlayable<ComplexEventTimelineTriggerMixerBehaviour> trackPlayable = ScriptPlayable<ComplexEventTimelineTriggerMixerBehaviour>.Create(graph, inputCount);
            ComplexEventTimelineTriggerMixerBehaviour behaviour = trackPlayable.GetBehaviour();
            StoreMixerProperties(go, behaviour);
            return trackPlayable;
        }
    }
}