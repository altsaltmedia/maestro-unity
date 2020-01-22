using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt.Maestro
{    
    public class DebugTimelineMixerBehaviour : LerpToTargetMixerBehaviour
    {
        private TrackAssetConfig _trackAssetConfig;

        public TrackAssetConfig trackAssetConfig
        {
            get => _trackAssetConfig;
            set => _trackAssetConfig = value;
        }

        private float timelineDebugTime
        {
            set => trackAssetConfig.timelineDebugTime = value;
        }

        private SimpleEventTrigger onEditorGraphStart
        {
            get => trackAssetConfig.onEditorGraphStart;
        }

        public override void OnGraphStart(Playable playable)
        {
            base.OnGraphStart(playable);
#if UNITY_EDITOR
            onEditorGraphStart.RaiseEvent(base.trackAssetConfig.gameObject, "debug timeline");
#endif
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
#if UNITY_EDITOR
            timelineDebugTime = (float)base.currentTime;
#endif
        }
    }   
}