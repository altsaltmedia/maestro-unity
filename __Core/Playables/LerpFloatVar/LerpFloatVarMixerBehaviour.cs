using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt
{    
    public class LerpFloatVarMixerBehaviour : LerpToTargetMixerBehaviour
    {
        FloatVariable trackBinding;
        ScriptPlayable<LerpFloatVarBehaviour> inputPlayable;
        LerpFloatVarBehaviour input;
        bool originalValueSet;
        float originalValue;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            trackBinding = playerData as FloatVariable;
            
            if (!trackBinding)
                return;

            if(originalValueSet == false) {
                originalValue = trackBinding.Value;
                originalValueSet = true;
            }
            
            inputCount = playable.GetInputCount ();
            
            for (int i = 0; i < inputCount; i++)
            {
                inputWeight = playable.GetInputWeight(i);
                inputPlayable = (ScriptPlayable<LerpFloatVarBehaviour>)playable.GetInput(i);
                input = inputPlayable.GetBehaviour ();
                
                if (inputWeight >= 1f) {
                    modifier = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());;
                    trackBinding.SetValue(Mathf.Lerp(input.initialValue, input.targetValue, input.easingFunction(0f, 1f, modifier)));
                } else {
                    if (currentTime >= input.endTime) {
                        trackBinding.SetValue(input.targetValue);
                    }
                    else if (i == 0 && currentTime <= input.startTime) {
                        trackBinding.SetValue(input.initialValue);
                    }
                }
            }
        }
        
        public override void OnPlayableDestroy(Playable playable)
        {
            base.OnPlayableDestroy(playable);

            // Reset color if we're working in edit mode
            if (trackBinding != null) {
                if (Application.isPlaying == true && scrubberActive == true) {
                    trackBinding.SetValue(originalValue);
                } else {
#if UNITY_EDITOR
                    trackBinding.SetValue(originalValue);
#endif
                }
            }
        }
    }   
}