using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Video;

namespace AltSalt
{    
    public class LerpAudioVolumeMixerBehaviour : LerpToTargetMixerBehaviour
    {
        AudioSource trackBinding;
        ScriptPlayable<LerpAudioVolumeBehaviour> inputPlayable;
        LerpAudioVolumeBehaviour input;

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
                inputPlayable = (ScriptPlayable<LerpAudioVolumeBehaviour>)playable.GetInput(i);
                input = inputPlayable.GetBehaviour ();
                
                if (inputWeight >= 1f) {
                    doubleModifier = inputPlayable.GetTime() / inputPlayable.GetDuration();
                    trackBinding.volume = Mathf.Lerp(input.initialVolume, input.targetVolume, (float)DoubleEasingFunction(0f, 1f, doubleModifier));
                } else {
                    if (currentTime >= input.endTime) {
                        trackBinding.volume = input.targetVolume;
                    }
                    else if (i == 0 && currentTime <= input.startTime && input.disableReset == false) {
                        trackBinding.volume = input.initialVolume;
                    }
                }

#if UNITY_EDITOR
                if (input.debugCurrentVolume == true) {
                    Debug.Log("Current volume: " + trackBinding.volume.ToString("F4"));
                }
#endif
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