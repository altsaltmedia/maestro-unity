using UnityEngine;
using UnityEngine.Playables;

#if UNITY_EDITOR
using UnityEditor.Timeline;
#endif

namespace AltSalt
{    
    public class DebugTimelineMixerBehaviour : LerpToTargetMixerBehaviour
    {
        FloatVariable trackBinding;
        ScriptPlayable<DebugTimelineBehaviour> inputPlayable;
        DebugTimelineBehaviour input;

        public override void OnGraphStart(Playable playable)
        {
            base.OnGraphStart(playable);
#if UNITY_EDITOR
            trackBinding = Utils.GetFloatVariable("TimelineCurrentTime");
            if(Application.isPlaying == false ) {
                TimelineEditor.inspectedDirector.time = trackBinding.Value;
            }
#endif
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
#if UNITY_EDITOR
            trackBinding.SetValue((float)currentTime);
#endif
        }
    }   
}