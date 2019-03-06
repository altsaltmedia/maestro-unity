using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Video;

namespace AltSalt
{    
    public class LerpVideoPlayerTimeMixerBehaviour : LerpToTargetMixerBehaviour
    {
        VideoPlayer trackBinding;
        ScriptPlayable<LerpVideoPlayerTimeBehaviour> inputPlayable;
        LerpVideoPlayerTimeBehaviour input;

        bool initialized = false;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            trackBinding = playerData as VideoPlayer;
            
            if (!trackBinding)
                return;

            if (!trackBinding.isPrepared) {
                trackBinding.Prepare();
            }

            if(!initialized) {
                trackBinding.StepForward();
                initialized = true;
            }
            
            inputCount = playable.GetInputCount ();
            
            for (int i = 0; i < inputCount; i++)
            {
                inputWeight = playable.GetInputWeight(i);
                inputPlayable = (ScriptPlayable<LerpVideoPlayerTimeBehaviour>)playable.GetInput(i);
                input = inputPlayable.GetBehaviour ();

                if (inputWeight >= 1f) {
                    modifier = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());
                    trackBinding.time = (double)Mathf.Lerp(input.initialTime, input.targetTime, input.easingFunction(0f, 1f, modifier));
                } else {
                    if (currentTime >= input.endTime) {
                        trackBinding.time = input.targetTime;
                    }
                    else if (i == 0 && currentTime <= input.startTime) {
                        trackBinding.time = input.initialTime;
                    }
                }

#if UNITY_EDITOR
                if (input.debugCurrentTime == true) {
                    Debug.Log("Current time: " + trackBinding.time.ToString("F4"));
                }
#endif
            }
        }
    }   
}