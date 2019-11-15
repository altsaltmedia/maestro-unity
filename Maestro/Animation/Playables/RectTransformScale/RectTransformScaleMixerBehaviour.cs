using UnityEngine;
using UnityEngine.Playables;
using TMPro;

namespace AltSalt.Maestro.Animation
{
    public class RectTransformScaleMixerBehaviour : LerpToTargetMixerBehaviour
    {
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            RectTransform trackBinding = playerData as RectTransform;
            ScriptPlayable<RectTransformScaleBehaviour> inputPlayable;
            RectTransformScaleBehaviour input;
            RectTransform trackBindingComponent;

            if (!trackBinding)
                return;
            
            inputCount = playable.GetInputCount ();
            
            for (int i = 0; i < inputCount; i++)
            {
                inputWeight = playable.GetInputWeight(i);
                inputPlayable = (ScriptPlayable<RectTransformScaleBehaviour>)playable.GetInput(i);
                input = inputPlayable.GetBehaviour ();
                
                trackBindingComponent = trackBinding.GetComponent<RectTransform>();
                
                if(inputWeight >= 1f) {
                    modifier = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());
                    trackBindingComponent.localScale = Vector3.Lerp(input.initialScale, input.targetScale, input.easingFunction(0f, 1f, modifier));
                } else {
                    if(currentTime >= input.endTime) {
                        trackBindingComponent.localScale = input.targetScale;
                    } else if (i == 0 && currentTime <= input.startTime) {
                        trackBindingComponent.localScale = input.initialScale;
                    }
                }
            }
        }

        public override void OnGraphStop(Playable playable)
        {
            base.OnGraphStop(playable);

        }
    }   
}