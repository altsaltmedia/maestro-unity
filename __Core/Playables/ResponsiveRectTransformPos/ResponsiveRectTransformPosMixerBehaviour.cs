using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt
{
    public class ResponsiveRectTransformPosMixerBehaviour : ResponsiveLerpToTargetMixerBehaviour
    {
        RectTransform trackBinding;
        ScriptPlayable<ResponsiveRectTransformPosBehaviour> inputPlayable;
        ResponsiveRectTransformPosBehaviour input;
        RectTransform trackBindingComponent;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            trackBinding = playerData as RectTransform;
            
            if (!trackBinding)
                return;

            if (trackBindingComponent == null) {
                trackBindingComponent = trackBinding.GetComponent<RectTransform>();
            }

            inputCount = playable.GetInputCount ();
            
            for (int i = 0; i < inputCount; i++)
            {
                inputWeight = playable.GetInputWeight(i);
                inputPlayable = (ScriptPlayable<ResponsiveRectTransformPosBehaviour>)playable.GetInput(i);
                input = inputPlayable.GetBehaviour ();
                
                if(inputWeight >= 1f) {
                    modifier = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());
                    trackBindingComponent.anchoredPosition3D = Vector3.Lerp(input.initialPosition, input.targetPosition, input.easingFunction(0f, 1f, modifier));
                } else {
                    if(currentTime >= input.endTime) {
                        trackBindingComponent.anchoredPosition3D = input.targetPosition;
                    } else if (i == 0 && currentTime <= input.startTime) {
                        trackBindingComponent.anchoredPosition3D = input.initialPosition;
                    }
                }
            }
        }
    }   
}