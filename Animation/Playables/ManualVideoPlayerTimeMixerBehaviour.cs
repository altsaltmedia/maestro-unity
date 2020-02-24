using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Playables;
using UnityEngine.Video;

namespace AltSalt.Maestro.Animation
{
    public class ManualVideoPlayerTimeMixerBehaviour : LerpToTargetMixerBehaviour
    {
        private ManualVideoPlayer trackBinding;
        private ScriptPlayable<ManualVideoPlayerTimeBehaviour> inputPlayable;
        private ManualVideoPlayerTimeBehaviour input;

        private bool _executeAndroidOverrides;

        private bool executeAndroidOverrides
        {
            get => _executeAndroidOverrides;
            set => _executeAndroidOverrides = value;
        }

        private double _originalTime;

        private double originalTime
        {
            get => _originalTime;
            set => _originalTime = value;
        }

        bool initialized = false;

        public override void OnGraphStart(Playable playable)
        {
            base.OnGraphStart(playable);
            
#if UNITY_ANDROID
            executeAndroidOverrides = true;
#endif
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            trackBinding = playerData as ManualVideoPlayer;

            if (!trackBinding)
                return;

            if (initialized == false) {
                trackBinding.CallPrepareVideo();
                trackBinding.timelineInstanceConfig = timelineInstanceConfig;
                originalTime = trackBinding.masterTime;
                initialized = true;
            }

            inputCount = playable.GetInputCount();

            for (int i = 0; i < inputCount; i++) {
                inputWeight = playable.GetInputWeight(i);
                inputPlayable = (ScriptPlayable<ManualVideoPlayerTimeBehaviour>)playable.GetInput(i);
                input = inputPlayable.GetBehaviour();

                if (inputWeight >= 1f) {
                    percentageComplete = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());

                    if (executeAndroidOverrides == false) {
                        trackBinding.CallSetTime((double)Mathf.Lerp(input.initialValueIOS, input.targetValueIOS, input.easingFunction(0f, 1f, percentageComplete)));
                    }
                    else {
                        trackBinding.CallSetTime((double)Mathf.Lerp(input.initialValueAndroid, input.targetValueAndroid, input.easingFunction(0f, 1f, percentageComplete)));
                    }

                }
                else {
                    if (timelineInstanceConfig.currentTime >= input.endTime) {
                        if (executeAndroidOverrides == false && trackBinding.masterTime != input.targetValueIOS ||
                            executeAndroidOverrides == true && trackBinding.masterTime != input.targetValueAndroid) {
                            trackBinding.CallSetTime(executeAndroidOverrides == false
                                ? input.targetValueIOS
                                : input.targetValueAndroid);
                        }
                        
                    }
                    else if (i == 0 && timelineInstanceConfig.currentTime <= input.startTime) {
                        if (executeAndroidOverrides == false && trackBinding.masterTime != input.initialValueIOS ||
                            executeAndroidOverrides == true && trackBinding.masterTime != input.initialValueAndroid) {
                            trackBinding.CallSetTime(executeAndroidOverrides == false
                                ? input.initialValueIOS
                                : input.initialValueAndroid);
                        }
                    }
                }

#if UNITY_EDITOR
                if (input.debugCurrentTime == true) {
                    trackBinding.LogTime();
                }
#endif
            }
        }
        
        public override void OnPlayableDestroy(Playable playable)
        {
            base.OnPlayableDestroy(playable);

            // Reset color if we're working in edit mode
            if (trackBinding != null) {
                
#if UNITY_EDITOR
                if(Application.isPlaying == false) {
                    trackBinding.CallSetTime(originalTime);
                }
#endif
            }
        }
    }   
}