using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AltSalt.Maestro.Animation
{
    [TrackColor(0.745149f, 0.495372f, 0.8679245f)]
    [TrackClipType(typeof(ColorClip))]
    [TrackBindingType(typeof(ColorVariable))]
    public class LerpColorVarTrack : LerpToTargetTrack
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            StoreClipProperties(go);
            ScriptPlayable<LerpColorVarMixerBehaviour> trackPlayable = ScriptPlayable<LerpColorVarMixerBehaviour>.Create(graph, inputCount);
            LerpColorVarMixerBehaviour behaviour = trackPlayable.GetBehaviour();
            StoreMixerProperties(go, behaviour);
            return trackPlayable;
        }

    }
    
}