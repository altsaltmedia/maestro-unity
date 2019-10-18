using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt
{    
    public class SequenceAutoplayConfigMixerBehaviour : PlayableBehaviour
    {
        // Utility vars - specified here to prevent garbage collection
        Sequence trackBinding;

        protected int inputCount;

        bool configSaved;

        ScriptPlayable<SequenceAutoplayConfigBehaviour> inputPlayable;
        SequenceAutoplayConfigBehaviour input;

        public override void OnGraphStart(Playable playable)
        {
            base.OnGraphStart(playable);
            configSaved = false;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {   
            trackBinding = playerData as Sequence;

            if (!trackBinding || configSaved == true)
                return;

            trackBinding.autoplayThresholds.Clear();
            trackBinding.hasAutoplay = true;

            inputCount = playable.GetInputCount();

            for (int i = 0; i < inputCount; i++)
            {
                inputPlayable = (ScriptPlayable<SequenceAutoplayConfigBehaviour>)playable.GetInput(i);
                input = inputPlayable.GetBehaviour ();

                StartEndThreshold threshold = new StartEndThreshold(input.startTime, input.endTime);
                threshold.description = input.description;
                threshold.isVideoSequence = input.isVideoSequence;

                trackBinding.autoplayThresholds.Add(threshold);
            }

            configSaved = true;
        }
    }   
}