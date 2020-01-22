using UnityEngine;
using UnityEngine.Playables;
using TMPro;

namespace AltSalt.Maestro.Animation
{
    public class SpriteColorMixerBehaviour : LerpToTargetMixerBehaviour
    {
        SpriteRenderer trackBinding;
        ScriptPlayable<ColorBehaviour> inputPlayable;
        ColorBehaviour input;
        SpriteRenderer trackBindingComponent;
        Color originalValue;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            trackBinding = playerData as SpriteRenderer;
            
            if (!trackBinding)
                return;

            if(trackBindingComponent == null) {
                trackBindingComponent = trackBinding.GetComponent<SpriteRenderer>();
                originalValue = trackBindingComponent.color;
            }

            inputCount = playable.GetInputCount ();
            
            for (int i = 0; i < inputCount; i++)
            {
                inputWeight = playable.GetInputWeight(i);
                inputPlayable = (ScriptPlayable<ColorBehaviour>)playable.GetInput(i);
                input = inputPlayable.GetBehaviour ();
                
                if(inputWeight >= 1f) {
                    percentageComplete = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());
                    trackBindingComponent.color = Color.Lerp(input.initialValue, input.targetValue, input.easingFunction(0f, 1f, percentageComplete));
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
                trackBindingComponent.color = Utils.transparent;
            }
        }
    }   
}