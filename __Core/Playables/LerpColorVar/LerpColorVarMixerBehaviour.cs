using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt
{    
    public class LerpColorVarMixerBehaviour : LerpToTargetMixerBehaviour
    {
        ColorVariable trackBinding;
        ScriptPlayable<LerpColorVarBehaviour> inputPlayable;
        LerpColorVarBehaviour input;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            trackBinding = playerData as ColorVariable;
            
            if (!trackBinding)
                return;
            
            inputCount = playable.GetInputCount ();
            
            for (int i = 0; i < inputCount; i++)
            {
                inputWeight = playable.GetInputWeight(i);
                inputPlayable = (ScriptPlayable<LerpColorVarBehaviour>)playable.GetInput(i);
                input = inputPlayable.GetBehaviour ();
                
                if (inputWeight >= 1f) {
                    modifier = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());
                    trackBinding.SetValue(Color.Lerp(input.initialColor, input.targetColor, input.easingFunction(0f, 1f, modifier)));
                } else {
                    if (currentTime >= input.endTime) {
                        trackBinding.SetValue(input.targetColor);
                    }
                    else if (i == 0 && currentTime <= input.startTime) {
                        trackBinding.SetValue(input.initialColor);
                    }
                }
            }
        }
        
        public override void OnGraphStop(Playable playable)
        {
            base.OnGraphStop(playable);

            // Reset color if we're working in edit mode
#if UNITY_EDITOR
            if(trackBinding != null) {
                trackBinding.SetDefaultValue();
            }
#endif
            
        }
    }   
}