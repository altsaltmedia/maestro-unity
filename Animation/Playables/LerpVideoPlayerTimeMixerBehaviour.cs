using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Video;

namespace AltSalt.Maestro.Animation
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
                    percentageComplete = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());
#if UNITY_ANDROID
                    trackBinding.SetTime((double)Mathf.Lerp(input.initialValueAndroid, input.targetValueAndroid, input.easingFunction(0f, 1f, modifier)));
#else
                    trackBinding.SetTime((double)Mathf.Lerp(input.initialValueIOS, input.targetValueIOS, input.easingFunction(0f, 1f, percentageComplete)));
#endif
                }
                else {
                    if (currentTime >= input.endTime) {
#if UNITY_ANDROID
                        trackBinding.SetTime(input.targetValueAndroid);
#else
                        trackBinding.SetTime(input.targetValueIOS);
#endif
                    }
                    else if (i == 0 && currentTime <= input.startTime) {
#if UNITY_ANDROID
                        trackBinding.SetTime(input.initialValueAndroid);
#else
                        trackBinding.SetTime(input.initialValueIOS);
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