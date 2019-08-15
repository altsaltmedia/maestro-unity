using UnityEngine;
using UnityEngine.Playables;
using TMPro;

namespace AltSalt
{
    public class TMProCharSpacingMixerBehaviour : ResponsiveLerpToTargetMixerBehaviour
    {
        TMP_Text trackBinding;
        ScriptPlayable<ResponsiveFloatBehaviour> inputPlayable;
        ResponsiveFloatBehaviour input;
        TMP_Text trackBindingComponent;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            trackBinding = playerData as TMP_Text;

            if (!trackBinding)
                return;
            
            inputCount = playable.GetInputCount ();
            
            for (int i = 0; i < inputCount; i++)
            {
                inputWeight = playable.GetInputWeight(i);
                inputPlayable = (ScriptPlayable<ResponsiveFloatBehaviour>)playable.GetInput(i);
                input = inputPlayable.GetBehaviour ();
                
                trackBindingComponent = trackBinding.GetComponent<TMP_Text>();
                
                if(inputWeight >= 1f) {
                    modifier = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());
                    trackBindingComponent.characterSpacing = Mathf.Lerp(input.initialValue, input.targetValue, input.easingFunction(0f, 1f, modifier));
                } else {
                    if(currentTime >= input.endTime) {
                        trackBindingComponent.characterSpacing = input.targetValue;
                    } else if (i == 0 && currentTime <= input.startTime) {
                        trackBindingComponent.characterSpacing = input.initialValue;
                    }
                }
            }
        }
    }   
}