using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt
{    
    public class LerpColorVarMixerBehaviour : LerpToTargetMixerBehaviour
    {
        ColorVariable trackBinding;
        ScriptPlayable<LerpColorVarBehaviour> inputPlayable;
        LerpColorVarBehaviour input;
        bool originalValueSet;
        Color originalValue;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            trackBinding = playerData as ColorVariable;
            
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
        
        public override void OnPlayableDestroy(Playable playable)
        {
            base.OnPlayableDestroy(playable);

            if (trackBinding != null) {
                if (Application.isPlaying == true && directorUpdater != null && directorUpdater.scrubberActive.Value == true) {
                    trackBinding.SetValue(Utils.transparent);
                } else {
#if UNITY_EDITOR
                    trackBinding.SetValue(originalValue);
#endif
                }
            }

        }
    }   
}