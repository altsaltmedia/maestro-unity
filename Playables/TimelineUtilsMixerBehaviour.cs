using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt.Maestro
{    
    public class TimelineUtilsMixerBehaviour : LerpToTargetMixerBehaviour
    {
        private float timelineDebugTime
        {
            set => trackAssetConfig.timelineDebugTime = value;
        }
        
        private static double _currentTime;

        public static double currentTime
        {
            get => _currentTime;
            set => _currentTime = value;
        }

        private SimpleEventTrigger onEditorGraphStart => trackAssetConfig.onEditorGraphStart;

        public override void OnGraphStart(Playable playable)
        {
            base.OnGraphStart(playable);
#if UNITY_EDITOR
            onEditorGraphStart.RaiseEvent(trackAssetConfig.gameObject, "debug timeline");
#endif
        }
        
        public override void PrepareFrame(Playable playable, FrameData info)
        {
            currentTime = playable.GetGraph().GetRootPlayable(0).GetTime();
#if UNITY_EDITOR
            timelineDebugTime = (float)currentTime;
#endif
        }

    }   
}