using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Video;

namespace AltSalt.Maestro.Audio
{
    [TrackColor(0.245149f, 0.595372f, 0.1679245f)]
    [TrackClipType(typeof(FloatClip))]
    [TrackBindingType(typeof(AudioSource))]
    public class AudioLerpVolumeTrack : LerpToTargetTrack
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            StoreClipProperties(go);
            ScriptPlayable<AudioLerpSnapshotMixerBehaviour> trackPlayable = ScriptPlayable<AudioLerpSnapshotMixerBehaviour>.Create(graph, inputCount);
            AudioLerpSnapshotMixerBehaviour behaviour = trackPlayable.GetBehaviour();
            StoreMixerProperties(go, behaviour);
            return trackPlayable;
        }
    }
}