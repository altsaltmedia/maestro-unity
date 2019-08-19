using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Audio;

namespace AltSalt
{
    [ExecuteInEditMode]
    public class AudioLerpSnapshotMixerBehaviour : LerpToTargetMixerBehaviour
    {
        AudioMixer trackBinding;
        ScriptPlayable<AudioLerpSnapshotBehaviour> inputPlayable;
        AudioLerpSnapshotBehaviour input;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            trackBinding = playerData as AudioMixer;

            if (!trackBinding)
                return;

            inputCount = playable.GetInputCount();

            for (int i = 0; i < inputCount; i++) {
                inputWeight = playable.GetInputWeight(i);
                inputPlayable = (ScriptPlayable<AudioLerpSnapshotBehaviour>)playable.GetInput(i);
                input = inputPlayable.GetBehaviour();
                modifier = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());

                AudioMixerSnapshot[] audioMixerSnapshots = CreateSnapshotArrayFromInput(input);

                if (input.crossfade == true) {

                    if (input.targetStartingWeight < 0.0001f) {
                        input.targetStartingWeight = 0.0001f;
                    }

                    if (input.targetEndingWeight < 0.0001f) {
                        input.targetEndingWeight = 0.0001f;
                    }

                    float weightModifier = CrossfadeAudio(i, input, inputWeight, modifier, currentTime);

                    if (float.IsNaN(weightModifier) == false) {
                        float[] newWeights = new float[2];
                        if(input.fadeType == FadeType.FadeIn) {
                            newWeights[0] = weightModifier / -80f;
                            newWeights[1] = 1 - (weightModifier / -80f);
                        } else {
                            newWeights[0] = 1 - (weightModifier / -80f);
                            newWeights[1] = weightModifier / -80f;
                        }
                        trackBinding.TransitionToSnapshots(audioMixerSnapshots, newWeights, Time.deltaTime);
                    }

                } else {

                    float weightModifier = BlendSnapshots(i, input, inputWeight, modifier, currentTime);

                    if (float.IsNaN(weightModifier) == false) {
                        float[] newWeights = new float[2];
                        newWeights[0] = 1 - weightModifier;
                        newWeights[1] = weightModifier;
                        trackBinding.TransitionToSnapshots(audioMixerSnapshots, newWeights, Time.deltaTime);
                    }  
                }

            } 
        }

        AudioMixerSnapshot[] CreateSnapshotArrayFromInput(AudioLerpSnapshotBehaviour behaviour)
        {
            AudioMixerSnapshot[] snapshotArray = new AudioMixerSnapshot[2];

            snapshotArray[0] = behaviour.fromSnapshot;
            snapshotArray[1] = behaviour.targetSnapshot;

            return snapshotArray;
        }

        static float CrossfadeAudio(int inputCount, AudioLerpSnapshotBehaviour behaviour, float inputWeight, float clipPercentage, double currentTime)
        {
            float weightModifier = float.NaN;

            if (inputWeight >= 1f) {
                weightModifier = Mathf.Log10(Mathf.Lerp(behaviour.targetStartingWeight, behaviour.targetEndingWeight, behaviour.easingFunction(0f, 1f, clipPercentage))) * 20f;
            } else {
                if (currentTime >= behaviour.endTime) {
                    weightModifier = Mathf.Log10(Mathf.Lerp(behaviour.targetStartingWeight, behaviour.targetEndingWeight, behaviour.easingFunction(0f, 1f, 1f))) * 20f;
                } else if (inputCount == 0 && currentTime <= behaviour.startTime) {
                    weightModifier = Mathf.Log10(Mathf.Lerp(behaviour.targetStartingWeight, behaviour.targetEndingWeight, behaviour.easingFunction(0f, 1f, 0))) * 20f;
                }
            }

            return weightModifier;
        }


        static float BlendSnapshots(int inputCount, AudioLerpSnapshotBehaviour behaviour, float inputWeight, float clipPercentage, double currentTime)
        {
            float weightModifier = float.NaN;

            if (inputWeight >= 1f) {
                weightModifier = Mathf.Lerp(behaviour.targetStartingWeight, behaviour.targetEndingWeight, behaviour.easingFunction(0f, 1f, clipPercentage));
            } else {
                if (currentTime >= behaviour.endTime) {
                    weightModifier = Mathf.Lerp(behaviour.targetStartingWeight, behaviour.targetEndingWeight, behaviour.easingFunction(0f, 1f, 1f));
                } else if (inputCount == 0 && currentTime <= behaviour.startTime) {
                    weightModifier = Mathf.Lerp(behaviour.targetStartingWeight, behaviour.targetEndingWeight, behaviour.easingFunction(0f, 1f, 0));
                }
            }

            return weightModifier;
        }

    }
}