using UnityEngine;
using UnityEngine.Playables;
using TMPro;

namespace AltSalt.Maestro.Animation
{
    public class TransformPosMixerBehaviour : LerpToTargetMixerBehaviour
    {
        Transform trackBinding;
        ScriptPlayable<ResponsiveVector3Behaviour> inputPlayable;
        ResponsiveVector3Behaviour input;
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            trackBinding = playerData as Transform;

            if (!trackBinding) {
                return;
            }

            inputCount = playable.GetInputCount ();
            
            for (int i = 0; i < inputCount; i++)
            {
                inputWeight = playable.GetInputWeight(i);
                inputPlayable = (ScriptPlayable<ResponsiveVector3Behaviour>)playable.GetInput(i);
                input = inputPlayable.GetBehaviour ();
               
                if(inputWeight >= 1f) {
                    percentageComplete = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());
                    trackBinding.localPosition = Vector3.Lerp(input.initialValue, input.targetValue, input.easingFunction(0f, 1f, percentageComplete));
                } else {
                    if(timelineInstanceConfig.currentTime >= input.endTime) {
                        trackBinding.localPosition = input.targetValue;
                    } else if (i == 0 && timelineInstanceConfig.currentTime <= input.startTime && input.disableReset == false) {
                        trackBinding.localPosition = input.initialValue;
                    }
                }
            }
        }

        public override void OnGraphStop(Playable playable)
        {
            base.OnGraphStop(playable);
            if (Application.isPlaying == true && isScrubbing == true) {
                if (trackBinding != null) {
                    trackBinding.localPosition = new Vector3(1000, 1000);
                }
            }
        }
    }   
}