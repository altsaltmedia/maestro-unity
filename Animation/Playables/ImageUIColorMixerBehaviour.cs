using UnityEngine;
using UnityEngine.Playables;
using TMPro;
using UnityEngine.UI;

namespace AltSalt.Maestro.Animation
{
    public class ImageUIColorMixerBehaviour : LerpToTargetMixerBehaviour
    {
        Image trackBinding;
        ScriptPlayable<ColorBehaviour> inputPlayable;
        ColorBehaviour input;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            trackBinding = playerData as Image;
            
            if (!trackBinding)
                return;

            inputCount = playable.GetInputCount ();
            
            for (int i = 0; i < inputCount; i++)
            {
                inputWeight = playable.GetInputWeight(i);
                inputPlayable = (ScriptPlayable<ColorBehaviour>)playable.GetInput(i);
                input = inputPlayable.GetBehaviour ();

                if(inputWeight >= 1f) {
                    percentageComplete = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());
                    trackBinding.color = Color.Lerp(input.initialValue, input.targetValue, input.easingFunction(0f, 1f, percentageComplete));
                } else {
                    if(trackAssetConfig.currentTime >= input.endTime) {
                        trackBinding.color = input.targetValue;
                    } else if (i == 0 && trackAssetConfig.currentTime <= input.startTime) {
                        trackBinding.color = input.initialValue;
                    }
                }
            }
        }
    }   
}