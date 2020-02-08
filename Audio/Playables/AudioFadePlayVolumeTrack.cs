using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Video;

namespace AltSalt.Maestro.Audio
{
    [TrackColor(0.245149f, 0.595372f, 0.1679245f)]
    [TrackClipType(typeof(AudioFadePlayVolumeClip))]
    public class AudioFadePlayVolumeTrack : LerpToTargetTrack
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            StoreClipProperties(go);
            ScriptPlayable<AudioFadePlayVolumeMixerBehaviour> trackPlayable = ScriptPlayable<AudioFadePlayVolumeMixerBehaviour>.Create(graph, inputCount);
            AudioFadePlayVolumeMixerBehaviour behaviour = trackPlayable.GetBehaviour();
            StoreMixerProperties(go, behaviour);
            return trackPlayable;
        }
    }
    
}