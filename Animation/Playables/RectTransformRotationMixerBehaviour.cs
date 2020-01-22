using UnityEngine;
using UnityEngine.Playables;
using TMPro;

namespace AltSalt.Maestro.Animation
{
    public class RectTransformRotationMixerBehaviour : LerpToTargetMixerBehaviour
    {
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            RectTransform trackBinding = playerData as RectTransform;
            ScriptPlayable<ResponsiveVector3Behaviour> inputPlayable;
            ResponsiveVector3Behaviour input;
            RectTransform trackBindingComponent;

            if (!trackBinding)
                return;
            
            inputCount = playable.GetInputCount ();
            
            for (int i = 0; i < inputCount; i++)
            {
                inputWeight = playable.GetInputWeight(i);
                inputPlayable = (ScriptPlayable<ResponsiveVector3Behaviour>)playable.GetInput(i);
                input = inputPlayable.GetBehaviour ();
                
                trackBindingComponent = trackBinding.GetComponent<RectTransform>();
                
                if(inputWeight >= 1f) {
                    percentageComplete = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());
                    trackBindingComponent.localEulerAngles = Vector3.Lerp(input.initialValue, input.targetValue, input.easingFunction(0f, 1f, percentageComplete));
                } else {
                    if(currentTime >= input.endTime) {
                        trackBindingComponent.localEulerAngles = input.targetValue;
                    } else if (i == 0 && currentTime <= input.startTime) {
                        trackBindingComponent.localEulerAngles = input.initialValue;
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