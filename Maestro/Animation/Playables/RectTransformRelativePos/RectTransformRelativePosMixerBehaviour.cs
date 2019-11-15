using UnityEngine;
using UnityEngine.Playables;
using TMPro;

namespace AltSalt.Maestro.Animation
{
    public class RectTransformRelativePosMixerBehaviour : LerpToTargetMixerBehaviour
    {
        RectTransform trackBinding;
        ScriptPlayable<RectTransformRelativePosBehaviour> inputPlayable;
        RectTransformRelativePosBehaviour input;
        RectTransform trackBindingComponent;
        Vector3 originalValue;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            
            trackBinding = playerData as RectTransform;

            if (!trackBinding)
                return;

            if(trackBindingComponent == null) {
                trackBindingComponent = trackBinding.GetComponent<RectTransform>();
                originalValue = trackBindingComponent.anchoredPosition3D;
            }

            inputCount = playable.GetInputCount ();
            
            for (int i = 0; i < inputCount; i++)
            {
                inputWeight = playable.GetInputWeight(i);
                inputPlayable = (ScriptPlayable<RectTransformRelativePosBehaviour>)playable.GetInput(i);
                input = inputPlayable.GetBehaviour ();

                if (inputWeight >= 1f) {
                    modifier = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());
                    trackBindingComponent.localPosition = Vector3.Lerp(input.originReferenceObject.GetComponent<RectTransform>().localPosition - input.offsetVector, input.targetReferenceObject.GetComponent<RectTransform>().localPosition - input.offsetVector, input.easingFunction(0f, 1f, modifier));
                } else {
                    if(currentTime >= input.endTime) {
                        trackBindingComponent.localPosition = input.targetReferenceObject.GetComponent<RectTransform>().localPosition - input.offsetVector;
                    } else if (i == 0 && currentTime <= input.startTime && input.disableReset == false) {
                        trackBindingComponent.localPosition = input.originReferenceObject.GetComponent<RectTransform>().localPosition - input.offsetVector;
                    }
                }
            }
        }

        public override void OnGraphStop(Playable playable)
        {
            base.OnGraphStop(playable);
            if (Application.isPlaying == true) {
                if (trackBindingComponent != null) {
                    trackBindingComponent.anchoredPosition3D = originalValue;
                }
            }
        }
    }   
}