using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AltSalt.Maestro
{
    [TrackColor(0.245149f, 0.895372f, 0.5679245f)]
    [TrackClipType(typeof(SimpleEventTimelineTriggerClip))]
    public class SimpleEventTimelineTriggerTrack : TimelineTriggerTrack
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            StoreClipProperties(go);
            ScriptPlayable<SimpleEventTimelineTriggerMixerBehaviour> trackPlayable = ScriptPlayable<SimpleEventTimelineTriggerMixerBehaviour>.Create(graph, inputCount);
            SimpleEventTimelineTriggerMixerBehaviour behaviour = trackPlayable.GetBehaviour();
            StoreMixerProperties(go, behaviour);
            return trackPlayable;
        }
    }
}