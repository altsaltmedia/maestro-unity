using UnityEngine;
using UnityEngine.Playables;
using TMPro;

namespace AltSalt.Maestro
{
    public class RectTransformRotationMixerBehaviour : LerpToTargetMixerBehaviour
    {
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            RectTransform trackBinding = playerData as RectTransform;
            ScriptPlayable<RectTransformRotationBehaviour> inputPlayable;
            RectTransformRotationBehaviour input;
            RectTransform trackBindingComponent;

            if (!trackBinding)
                return;
            
            inputCount = playable.GetInputCount ();
            
            for (int i = 0; i < inputCount; i++)
            {
                inputWeight = playable.GetInputWeight(i);
                inputPlayable = (ScriptPlayable<RectTransformRotationBehaviour>)playable.GetInput(i);
                input = inputPlayable.GetBehaviour ();
                
                trackBindingComponent = trackBinding.GetComponent<RectTransform>();
                
                if(inputWeight >= 1f) {
                    modifier = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());
                    trackBindingComponent.localEulerAngles = Vector3.Lerp(input.initialRotation, input.targetRotation, input.easingFunction(0f, 1f, modifier));
                } else {
                    if(currentTime >= input.endTime) {
                        trackBindingComponent.localEulerAngles = input.targetRotation;
                    } else if (i == 0 && currentTime <= input.startTime) {
                        trackBindingComponent.localEulerAngles = input.initialRotation;
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