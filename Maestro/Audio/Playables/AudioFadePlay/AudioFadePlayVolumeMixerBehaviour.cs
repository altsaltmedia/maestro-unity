using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Video;

namespace AltSalt.Maestro
{    
    public class AudioFadePlayVolumeMixerBehaviour : LerpToTargetMixerBehaviour
    {
        ScriptPlayable<AudioFadePlayVolumeBehaviour> inputPlayable;
        AudioFadePlayVolumeBehaviour input;
        List<float> originalVolumes = new List<float>();

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


                if (originalVolumes.Count < 1) {
                    for (int z = 0; z < input.targetAudioSources.Count; z++){
                        originalVolumes.Add(input.targetAudioSources[z].volume);
                    }
                }

                for (int q=0; q < input.targetAudioSources.Count; q++) {

                    AudioSource audioSource = input.targetAudioSources[q];

                    if (inputWeight >= 1f) {
                        doubleModifier = inputPlayable.GetTime() / inputPlayable.GetDuration();
                        audioSource.volume = Mathf.Lerp(0, 1, (float)DoubleEasingFunction(0f, 1f, doubleModifier));
                    } else {
                        if (currentTime >= input.endTime) {
                            if(audioSource.isPlaying == false) {
                                audioSource.volume = 1;
                                audioSource.Play();
                            }
                        } else if (i == 0 && currentTime <= input.startTime) {
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

            // Reset color if we're working in edit mode
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