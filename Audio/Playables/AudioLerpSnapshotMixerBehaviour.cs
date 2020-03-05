using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Audio;

namespace AltSalt.Maestro.Audio
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
                percentageComplete = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());

                // Skip this clip if snapshots are not defined.
                if (input.snapshotA == null || input.snapshotB == null) continue;

                AudioMixerSnapshot[] audioMixerSnapshots = CreateSnapshotArrayFromInput(input);

                // Because the volume of mixers is on an logarithmic scale,
                // we need special handling for when we're crossfading volume to make
                // sure things scale as expected
                if (input.crossfade == true) {

                    if (input.initialBlend < 0.0001f) {
                        input.initialBlend = 0.0001f;
                    }

                    if (input.targetBlend < 0.0001f) {
                        input.targetBlend = 0.0001f;
                    }

                    float weightModifier = CrossfadeAudio(i, input, inputWeight, percentageComplete, timelineInstanceConfig.currentTime);

                    if (float.IsNaN(weightModifier) == false) {
                        float[] newWeights = new float[2];
                        newWeights[0] = weightModifier / -80f;
                        newWeights[1] = 1 - (weightModifier / -80f);
                        trackBinding.TransitionToSnapshots(audioMixerSnapshots, newWeights, Time.deltaTime);
                    }

                } else {

                    float weightModifier = BlendSnapshots(i, input, inputWeight, percentageComplete, timelineInstanceConfig.currentTime);

                    if (float.IsNaN(weightModifier) == false) {
                        float[] newWeights = new float[2];
                        newWeights[0] = 1 - weightModifier;
                        newWeights[1] = weightModifier;
                        trackBinding.TransitionToSnapshots(audioMixerSnapshots, newWeights, Time.deltaTime);
                    }  
                }

            } 
        }

        private static AudioMixerSnapshot[] CreateSnapshotArrayFromInput(AudioLerpSnapshotBehaviour behaviour)
        {
            AudioMixerSnapshot[] snapshotArray = new AudioMixerSnapshot[2];

            snapshotArray[0] = behaviour.snapshotA;
            snapshotArray[1] = behaviour.snapshotB;

            return snapshotArray;
        }

        private static float CrossfadeAudio(int inputCount, AudioLerpSnapshotBehaviour behaviour, float inputWeight, float clipPercentage, double currentTime)
        {
            float weightModifier = float.NaN;

            if (inputWeight >= 1f) {
                weightModifier = Mathf.Log10(Mathf.Lerp(behaviour.initialBlend, behaviour.targetBlend, behaviour.easingFunction(0f, 1f, clipPercentage))) * 20f;
            } else {
                if (currentTime >= behaviour.endTime) {
                    weightModifier = Mathf.Log10(Mathf.Lerp(behaviour.initialBlend, behaviour.targetBlend, behaviour.easingFunction(0f, 1f, 1f))) * 20f;
                } else if (inputCount == 0 && currentTime <= behaviour.startTime) {
                    weightModifier = Mathf.Log10(Mathf.Lerp(behaviour.initialBlend, behaviour.targetBlend, behaviour.easingFunction(0f, 1f, 0))) * 20f;
                }
            }

            return weightModifier;
        }


        protected virtual float BlendSnapshots(int inputCount, AudioLerpSnapshotBehaviour behaviour, float inputWeight, float clipPercentage, double currentTime)
        {
            float weightModifier = float.NaN;

            if (inputWeight >= 1f) {
                weightModifier = Mathf.Lerp(behaviour.initialBlend, behaviour.targetBlend, behaviour.easingFunction(0f, 1f, clipPercentage));
            } else {
                if (currentTime >= behaviour.endTime) {
                    weightModifier = Mathf.Lerp(behaviour.initialBlend, behaviour.targetBlend, behaviour.easingFunction(0f, 1f, 1f));
                } else if (inputCount == 0 && currentTime <= behaviour.startTime) {
                    weightModifier = Mathf.Lerp(behaviour.initialBlend, behaviour.targetBlend, behaviour.easingFunction(0f, 1f, 0));
                }
            }

            return weightModifier;
        }

    }
}