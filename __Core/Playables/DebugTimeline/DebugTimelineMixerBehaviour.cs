using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt
{    
    public class DebugTimelineMixerBehaviour : LerpToTargetMixerBehaviour
    {
        FloatVariable trackBinding;
        ScriptPlayable<DebugTimelineBehaviour> inputPlayable;
        DebugTimelineBehaviour input;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            trackBinding = playerData as FloatVariable;
            trackBinding.SetValue((float)currentTime);
        }
        
//        public override void OnGraphStop(Playable playable)
//        {
//            base.OnGraphStop(playable);

//            // Reset color if we're working in edit mode
//#if UNITY_EDITOR
//            if(trackBinding != null) {
//                trackBinding.SetDefaultValue();
//            }
//#endif
            
        //}
    }   
}