using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Video;

namespace AltSalt.Maestro.Audio
{    
    public class AudioFadePlayVolumeMixerBehaviour : LerpToTargetMixerBehaviour
    {
        ScriptPlayable<AudioFadePlayVolumeBehaviour> inputPlayable;
        AudioFadePlayVolumeBehaviour input;

        double doubleModifier;

        public double DoubleEasingFunction(double start, double end, double value) {
            return ((end - start) * value) + start;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            inputCount = playable.GetInputCount ();

            for (int i = 0; i < inputCount; i++) {
                inputWeight = playable.GetInputWeight(i);
                inputPlayable = (ScriptPlayable<AudioFadePlayVolumeBehaviour>)playable.GetInput(i);
                input = inputPlayable.GetBehaviour();

                for (int q=0; q < input.targetAudioSources.Count; q++) {

                    AudioSource audioSource = input.targetAudioSources[q];

                    // In most cases, when we set up a fadePlay track,
                    // we want to play audio when moving forward and slowly
                    // fade out when going in reverse. However, when we link a
                    // sequence containing an initial fadePlay track to a sibling
                    // sequence, we'll simply want to make sure the track is playing
                    // in the sibling and don't need the fading in / out behaviour.
                    if (input.isReset == true && inputWeight >= 1f || timelineInstanceConfig.currentTime >= input.endTime) {
                        if(audioSource.isPlaying == false) {
                            audioSource.volume = 1;
                            audioSource.Play();
                        }
                        continue;
                    }

                    // While the playhead is over the clip, slowly fade in / fade out a track's volume
                    if (inputWeight >= 1f) {
                        doubleModifier = inputPlayable.GetTime() / inputPlayable.GetDuration();
                        audioSource.volume = Mathf.Lerp(0, 1, (float)DoubleEasingFunction(0f, 1f, doubleModifier));
                    } else {
                        // Once the playhead is beyond the clip's end threshold, play the audio
                        if (timelineInstanceConfig.currentTime >= input.endTime) {
                            if(audioSource.isPlaying == false) {
                                audioSource.volume = 1;
                                audioSource.Play();
                            }
                            
                        }
                        // Once the playhead is before the clip's start threshold, stop the audio
                        else if (i == 0 && timelineInstanceConfig.currentTime <= input.startTime) {
                            audioSource.volume = 0;
                            audioSource.Stop();
                        }
                    }

    #if UNITY_EDITOR
                    if (input.debugCurrentVolume == true) {
                        Debug.Log("Current volume: " + audioSource.volume.ToString("F4"));
                    }
    #endif
                }
            }
        }
        
        public override void OnGraphStop(Playable playable)
        {
            base.OnGraphStop(playable);

#if UNITY_EDITOR
            if (input != null) {
                for (int q = 0; q < input.targetAudioSources.Count; q++) {
                    AudioSource audioSource = input.targetAudioSources[q];
                    if(Application.isPlaying == false && audioSource != null) {
                        audioSource.Stop();
                        audioSource.time = 0;
                        audioSource.volume = 0;
                    }
                }
            }
#endif

        }
    }   
}