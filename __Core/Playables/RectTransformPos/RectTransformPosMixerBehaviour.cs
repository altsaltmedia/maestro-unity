using UnityEngine;
using UnityEngine.Playables;
using TMPro;

namespace AltSalt
{
    public class RectTransformPosMixerBehaviour : LerpToTargetMixerBehaviour
    {
        RectTransform trackBinding;
        ScriptPlayable<RectTransformPosBehaviour> inputPlayable;
        RectTransformPosBehaviour input;
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
                inputPlayable = (ScriptPlayable<RectTransformPosBehaviour>)playable.GetInput(i);
                input = inputPlayable.GetBehaviour ();
               
                if(inputWeight >= 1f) {
                    modifier = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());
                    trackBindingComponent.anchoredPosition3D = Vector3.Lerp(input.initialPosition, input.targetPosition, input.easingFunction(0f, 1f, modifier));
                } else {
                    if(currentTime >= input.endTime) {
                        trackBindingComponent.anchoredPosition3D = input.targetPosition;
                    } else if (i == 0 && currentTime <= input.startTime && input.disableReset == false) {
                        trackBindingComponent.anchoredPosition3D = input.initialPosition;
                    }
                }
            }
        }

        public override void OnGraphStop(Playable playable)
        {
            base.OnGraphStop(playable);
            if (Application.isPlaying == true && directorUpdater != null && directorUpdater.scrubberActive.Value == true) {
                if (trackBindingComponent != null) {
                    trackBindingComponent.anchoredPosition3D = originalValue;
                }
            }
        }
    }   
}