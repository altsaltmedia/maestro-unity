using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt.Maestro
{
    public class ResponsiveRectTransformScaleMixerBehaviour : ResponsiveLerpToTargetMixerBehaviour
    {
        RectTransform trackBinding;
        ScriptPlayable<ResponsiveVector3Behaviour> inputPlayable;
        ResponsiveVector3Behaviour input;
        RectTransform trackBindingComponent;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            trackBinding = playerData as RectTransform;
            
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
                    modifier = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());
                    trackBindingComponent.localScale = Vector3.Lerp(input.initialValue, input.targetValue, input.easingFunction(0f, 1f, modifier));
                } else {
                    if(currentTime >= input.endTime) {
                        trackBindingComponent.localScale = input.targetValue;
                    } else if (i == 0 && currentTime <= input.startTime) {
                        trackBindingComponent.localScale = input.initialValue;
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