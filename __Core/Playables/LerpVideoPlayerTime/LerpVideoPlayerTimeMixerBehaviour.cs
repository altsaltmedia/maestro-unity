using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Video;

namespace AltSalt
{
    public class LerpVideoPlayerTimeMixerBehaviour : LerpToTargetMixerBehaviour
    {
        PlayableVideoPlayerController trackBinding;
        ScriptPlayable<LerpVideoPlayerTimeBehaviour> inputPlayable;
        LerpVideoPlayerTimeBehaviour input;

        bool initialized = false;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            trackBinding = playerData as PlayableVideoPlayerController;

            if (!trackBinding)
                return;

            if (initialized == false) {
                trackBinding.PrepareVideos();
                initialized = true;
            }

            inputCount = playable.GetInputCount();

            for (int i = 0; i < inputCount; i++) {
                inputWeight = playable.GetInputWeight(i);
                inputPlayable = (ScriptPlayable<LerpVideoPlayerTimeBehaviour>)playable.GetInput(i);
                input = inputPlayable.GetBehaviour();

                if (inputWeight >= 1f) {
                    modifier = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());
#if UNITY_ANDROID
                    trackBinding.SetTime((double)Mathf.Lerp(input.initialTimeAndroid, input.targetTimeAndroid, input.easingFunction(0f, 1f, modifier)));
#else
                    trackBinding.SetTime((double)Mathf.Lerp(input.initialTime, input.targetTime, input.easingFunction(0f, 1f, modifier)));
#endif
                }
                else {
                    if (currentTime >= input.endTime) {
#if UNITY_ANDROID
                        trackBinding.SetTime(input.targetTimeAndroid);
#else
                        trackBinding.SetTime(input.targetTime);
#endif
                    }
                    else if (i == 0 && currentTime <= input.startTime) {
#if UNITY_ANDROID
                        trackBinding.SetTime(input.initialTimeAndroid);
#else
                        trackBinding.SetTime(input.initialTime);
#endif
                    }
                }

#if UNITY_EDITOR
                if (input.debugCurrentTime == true) {
                    trackBinding.LogTime();
                }
#endif
            }
        }
    }   
}