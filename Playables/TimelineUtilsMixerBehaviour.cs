using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt.Maestro
{    
    public class TimelineUtilsMixerBehaviour : LerpToTargetMixerBehaviour
    {
        private float timelineDebugTime
        {
            set => timelineInstanceConfig.timelineDebugTime = value;
        }

        private SimpleEventTrigger onEditorGraphStart => timelineInstanceConfig.onEditorGraphStart;

        public override void OnGraphStart(Playable playable)
        {
            base.OnGraphStart(playable);
#if UNITY_EDITOR
            onEditorGraphStart.RaiseEvent(timelineInstanceConfig.gameObject, "debug timeline");
#endif
        }
        
        public override void PrepareFrame(Playable playable, FrameData info)
        {
            timelineInstanceConfig.currentTime = playable.GetGraph().GetRootPlayable(0).GetTime();
#if UNITY_EDITOR
            timelineDebugTime = (float)timelineInstanceConfig.currentTime;
#endif
        }

    }   
}