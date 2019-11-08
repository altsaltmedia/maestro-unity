using UnityEngine;
using UnityEngine.Playables;
using TMPro;

namespace AltSalt.Maestro
{
    public class TMProUIColorMixerBehaviour : LerpToTargetMixerBehaviour
    {
        TextMeshProUGUI trackBinding;
        ScriptPlayable<TMProColorBehaviour> inputPlayable;
        TMProColorBehaviour input;
        TextMeshProUGUI trackBindingComponent;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            trackBinding = playerData as TextMeshProUGUI;
            
            if (!trackBinding)
                return;
            
            inputCount = playable.GetInputCount ();
            
            for (int i = 0; i < inputCount; i++)
            {
                inputWeight = playable.GetInputWeight(i);
                inputPlayable = (ScriptPlayable<TMProColorBehaviour>)playable.GetInput(i);
                input = inputPlayable.GetBehaviour ();
                
                trackBindingComponent = trackBinding.GetComponent<TextMeshProUGUI>();
                
                if(inputWeight >= 1f) {
                    modifier = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());
                    trackBindingComponent.color = Color.Lerp(input.initialColor, input.targetColor, input.easingFunction(0f, 1f, modifier));
                } else {
                    if(currentTime >= input.endTime) {
                        trackBindingComponent.color = input.targetColor;
                    } else if (i == 0 && currentTime <= input.startTime) {
                        trackBindingComponent.color = input.initialColor;
                    }
                }
            }
        }
    }   
}