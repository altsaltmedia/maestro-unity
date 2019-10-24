using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt
{    
    public class DebugTimelineMixerBehaviour : LerpToTargetMixerBehaviour
    {
        private FloatReference _timelineCurrentTime = new FloatReference();

        public FloatReference timelineCurrentTime
        {
            get => _timelineCurrentTime;
        }

        public SimpleEventTrigger onGraphStart = new SimpleEventTrigger();

        public override void OnGraphStart(Playable playable)
        {
            base.OnGraphStart(playable);
#if UNITY_EDITOR
            onGraphStart.RaiseEvent(directorObject, directorObject.scene.name, "debug timeline");
#endif
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
#if UNITY_EDITOR
            timelineCurrentTime.Variable.SetValue((float)base.currentTime);
#endif
        }
    }   
}