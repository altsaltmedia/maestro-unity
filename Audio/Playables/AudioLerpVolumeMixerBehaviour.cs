using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Video;

namespace AltSalt.Maestro.Audio
{    
    public class AudioLerpVolumeMixerBehaviour : LerpToTargetMixerBehaviour
    {
        AudioSource trackBinding;
        ScriptPlayable<FloatBehaviour> inputPlayable;
        FloatBehaviour input;

        double doubleModifier;

        public double DoubleEasingFunction(double start, double end, double value) {
            return ((end - start) * value) + start;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            trackBinding = playerData as AudioSource;
            
            if (!trackBinding)
                return;
            
            inputCount = playable.GetInputCount ();
            
            for (int i = 0; i < inputCount; i++)
            {
                inputWeight = playable.GetInputWeight(i);
                inputPlayable = (ScriptPlayable<FloatBehaviour>)playable.GetInput(i);
                input = inputPlayable.GetBehaviour ();
                
                if (inputWeight >= 1f) {
                    doubleModifier = inputPlayable.GetTime() / inputPlayable.GetDuration();
                    trackBinding.volume = Mathf.Lerp(input.initialValue, input.targetValue, (float)DoubleEasingFunction(0f, 1f, doubleModifier));
                } else {
                    if (trackAssetConfig.currentTime >= input.endTime) {
                        trackBinding.volume = input.targetValue;
                    }
                    else if (i == 0 && trackAssetConfig.currentTime <= input.startTime && input.disableReset == false) {
                        trackBinding.volume = input.initialValue;
                    }
                }
            }
        }
        
        public override void OnGraphStop(Playable playable)
        {
            base.OnGraphStop(playable);

            // Reset color if we're working in edit mode
#if UNITY_EDITOR
            if (trackBinding != null) {
                trackBinding.time = 0;
                trackBinding.volume = 0;
            }
#endif

        }
    }   
}