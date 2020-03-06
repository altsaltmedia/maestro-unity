using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Audio;

namespace AltSalt.Maestro.Audio
{
    [TrackColor(0.545149f, 0.895372f, 0.2679245f)]
    [TrackClipType(typeof(AudioLerpSnapshotClip))]
    [TrackBindingType(typeof(AudioMixer))]
    public class AudioLerpSnapshotTrack : LerpToTargetTrack
    {
        [SerializeField]
        private DefaultSnapshotLerpType _defaultSnapshotLerpType = DefaultSnapshotLerpType.Standard;

        public DefaultSnapshotLerpType defaultSnapshotLerpType
        {
            get => _defaultSnapshotLerpType;
            set => _defaultSnapshotLerpType = value;
        }

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