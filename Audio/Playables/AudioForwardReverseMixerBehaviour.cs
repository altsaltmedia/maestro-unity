using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Video;

namespace AltSalt.Maestro.Audio
{    
    public class AudioForwardReverseMixerBehaviour : LerpToTargetMixerBehaviour
    {
        AudioSource trackBinding;
        ScriptPlayable<AudioForwardReverseBehaviour> inputPlayable;
        AudioForwardReverseBehaviour input;
        bool lastSwipeDirection;
        float swipeValue;
        float modifier;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            trackBinding = playerData as AudioSource;
            
            if (!trackBinding)
                return;
            
            inputCount = playable.GetInputCount ();

            for (int i = 0; i < inputCount; i++)
            {
                inputWeight = playable.GetInputWeight(i);
                inputPlayable = (ScriptPlayable<AudioForwardReverseBehaviour>)playable.GetInput(i);
                input = inputPlayable.GetBehaviour ();

                if (Mathf.Approximately(swipeValue, input.swipeModifierOutput) == false) {
                    swipeValue = input.swipeModifierOutput;
                    modifier = Mathf.Abs(swipeValue);
                }
                modifier -= Time.deltaTime;
                if (modifier > input.frameStepValue) {
                    SetTrackPitch(trackBinding, input.isReversing, (modifier - input.frameStepValue) + 1);
                } else {
                    SetTrackPitch(trackBinding, input.isReversing, 1);
                }

                if (inputWeight > 0)  {
                    if (input.playingTriggered == false) {
                        input.playingTriggered = true;
                        trackBinding.Play();
                    }
                } else if(timelineInstanceConfig.currentTime > input.endTime || timelineInstanceConfig.currentTime < input.startTime) {
                    input.playingTriggered = false;
                    trackBinding.Stop();
                    if(timelineInstanceConfig.currentTime > input.endTime) {
                        trackBinding.time = input.clipEndTime;
                    } else if(timelineInstanceConfig.currentTime < input.startTime) {
                        trackBinding.time = 0;
                    }
                }
            }
        }

        void SetTrackPitch(AudioSource audioSource, bool reverse, float pitchModifier) {
            if(reverse == false) {
                audioSource.pitch = pitchModifier;
            } else {
                audioSource.pitch = pitchModifier * -1f;
            }
        }
        
        public override void OnGraphStop(Playable playable)
        {
            base.OnGraphStop(playable);

            // Reset color if we're working in edit mode
#if UNITY_EDITOR
            if (trackBinding != null) {
                trackBinding.time = 0;
                trackBinding.pitch = 1;
                trackBinding.Stop();
            }
#endif

        }
    }   
}