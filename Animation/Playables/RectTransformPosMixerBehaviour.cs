using UnityEngine;
using UnityEngine.Playables;
using TMPro;

namespace AltSalt.Maestro.Animation
{
    public class RectTransformPosMixerBehaviour : LerpToTargetMixerBehaviour
    {
        RectTransform trackBinding;
        ScriptPlayable<ResponsiveVector3Behaviour> inputPlayable;
        ResponsiveVector3Behaviour input;
        RectTransform trackBindingComponent;
        Vector3 originalValue;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            trackBinding = playerData as RectTransform;

            if (!trackBinding) {
                return;
            }

            if(trackBindingComponent == null) {
                trackBindingComponent = trackBinding.GetComponent<RectTransform>();
                originalValue = trackBindingComponent.anchoredPosition3D;
            }

            inputCount = playable.GetInputCount ();
            
            for (int i = 0; i < inputCount; i++)
            {
                inputWeight = playable.GetInputWeight(i);
                inputPlayable = (ScriptPlayable<ResponsiveVector3Behaviour>)playable.GetInput(i);
                input = inputPlayable.GetBehaviour ();
               
                if(inputWeight >= 1f) {
                    percentageComplete = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());
                    trackBindingComponent.anchoredPosition3D = Vector3.Lerp(input.initialValue, input.targetValue, input.easingFunction(0f, 1f, percentageComplete));
                } else {
                    if(currentTime >= input.endTime) {
                        trackBindingComponent.anchoredPosition3D = input.targetValue;
                    } else if (i == 0 && currentTime <= input.startTime && input.disableReset == false) {
                        trackBindingComponent.anchoredPosition3D = input.initialValue;
                    }
                }
            }
        }

        public override void OnGraphStop(Playable playable)
        {
            base.OnGraphStop(playable);
            if (Application.isPlaying == true && scrubberActive == true) {
                if (trackBindingComponent != null) {
                    trackBindingComponent.anchoredPosition3D = new Vector3(1000, 1000);
                }
            }
        }
    }   
}