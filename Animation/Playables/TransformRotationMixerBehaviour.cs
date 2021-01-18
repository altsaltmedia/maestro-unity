using UnityEngine;
using UnityEngine.Playables;
using TMPro;

namespace AltSalt.Maestro.Animation
{
    public class TransformRotationMixerBehaviour : LerpToTargetMixerBehaviour
    {
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            Transform trackBinding = playerData as Transform;
            ScriptPlayable<ResponsiveVector3Behaviour> inputPlayable;
            ResponsiveVector3Behaviour input;
            Transform trackBindingComponent;

            if (!trackBinding)
                return;
            
            inputCount = playable.GetInputCount ();
            
            for (int i = 0; i < inputCount; i++)
            {
                inputWeight = playable.GetInputWeight(i);
                inputPlayable = (ScriptPlayable<ResponsiveVector3Behaviour>)playable.GetInput(i);
                input = inputPlayable.GetBehaviour ();
                
                trackBindingComponent = trackBinding.GetComponent<Transform>();
                
                if(inputWeight >= 1f) {
                    percentageComplete = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());
                    trackBindingComponent.localEulerAngles = Vector3.Lerp(input.initialValue, input.targetValue, input.easingFunction(0f, 1f, percentageComplete));
                } else {
                    if(timelineInstanceConfig.currentTime >= input.endTime) {
                        trackBindingComponent.localEulerAngles = input.targetValue;
                    } else if (i == 0 && timelineInstanceConfig.currentTime <= input.startTime) {
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