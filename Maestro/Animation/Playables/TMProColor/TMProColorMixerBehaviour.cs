using UnityEngine;
using UnityEngine.Playables;
using TMPro;

namespace AltSalt.Maestro.Animation
{
    public class TMProColorMixerBehaviour : LerpToTargetMixerBehaviour
    {
        TMP_Text trackBinding;
        ScriptPlayable<ColorBehaviour> inputPlayable;
        ColorBehaviour input;
        TMP_Text trackBindingComponent;
        Color originalValue;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            trackBinding = playerData as TMP_Text;

            if (!trackBinding)
                return;

            if (trackBindingComponent == null) {
                trackBindingComponent = trackBinding.GetComponent<TMP_Text>();
                originalValue = trackBindingComponent.color;
            }

            inputCount = playable.GetInputCount ();
            
            for (int i = 0; i < inputCount; i++)
            {
                inputWeight = playable.GetInputWeight(i);
                inputPlayable = (ScriptPlayable<ColorBehaviour>)playable.GetInput(i);
                input = inputPlayable.GetBehaviour ();
                
                trackBindingComponent = trackBinding.GetComponent<TMP_Text>();
                
                if(inputWeight >= 1f) {
                    modifier = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());
                    trackBindingComponent.color = Color.Lerp(input.initialValue, input.targetValue, input.easingFunction(0f, 1f, modifier));
                } else {
                    if(currentTime >= input.endTime) {
                        trackBindingComponent.color = input.targetValue;
                    } else if (i == 0 && currentTime <= input.startTime) {
                        trackBindingComponent.color = input.initialValue;
                    }
                }
            }
        }

        public override void OnGraphStop(Playable playable)
        {
            base.OnGraphStop(playable);
            if (Application.isPlaying == true && scrubberActive == true) {
                if (trackBindingComponent != null) {    
                    trackBindingComponent.color = Utils.transparent;
                }
            }
        }
    }   
}