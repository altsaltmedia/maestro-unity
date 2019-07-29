using UnityEngine;
using UnityEngine.Playables;
using UnityEditor.Timeline;

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
            trackBinding = Utils.GetFloatVariable("TimelineCurrentTime");
            TimelineEditor.inspectedDirector.time = trackBinding.Value;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            trackBinding.SetValue((float)currentTime);
        }
        
    }   
}