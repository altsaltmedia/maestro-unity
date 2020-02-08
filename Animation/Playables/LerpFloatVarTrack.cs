using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AltSalt.Maestro.Animation
{
    [TrackColor(0.245149f, 0.895372f, 0.5679245f)]
    [TrackClipType(typeof(FloatClip))]
    [TrackBindingType(typeof(FloatVariable))]
    public class LerpFloatVarTrack : LerpToTargetTrack
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            StoreClipProperties(go);
            ScriptPlayable<LerpFloatVarMixerBehaviour> trackPlayable = ScriptPlayable<LerpFloatVarMixerBehaviour>.Create(graph, inputCount);
            LerpFloatVarMixerBehaviour behaviour = trackPlayable.GetBehaviour();
            StoreMixerProperties(go, behaviour);
            return trackPlayable;
        }
    }
}