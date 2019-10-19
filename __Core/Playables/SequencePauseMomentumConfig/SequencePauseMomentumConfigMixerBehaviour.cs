using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt
{    
    public class SequencePauseMomentumConfigMixerBehaviour : PlayableBehaviour
    {
        // Utility vars - specified here to prevent garbage collection
//        Sequence trackBinding;
//
//        protected int inputCount;
//
//        bool configSaved;
//
//        ScriptPlayable<SequencePauseMomentumConfigBehaviour> inputPlayable;
//        SequencePauseMomentumConfigBehaviour input;
//
//        public override void OnGraphStart(Playable playable)
//        {
//            base.OnGraphStart(playable);
//            configSaved = false;
//        }
//
//        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
//        {   
//            trackBinding = playerData as Sequence;
//
//            if (!trackBinding || configSaved == true)
//                return;
//
//            trackBinding.pauseMomentumThresholds.Clear();
//            trackBinding.hasPauseMomentum = true;
//
//            inputCount = playable.GetInputCount();
//
//            for (int i = 0; i < inputCount; i++)
//            {
//                inputPlayable = (ScriptPlayable<SequencePauseMomentumConfigBehaviour>)playable.GetInput(i);
//                input = inputPlayable.GetBehaviour ();
//
//                StartEndThreshold threshold = new StartEndThreshold(input.startTime, input.endTime);
//                threshold.description = input.description;
//                threshold.isVideoSequence = input.isVideoSequence;
//                trackBinding.pauseMomentumThresholds.Add(threshold);
//            }
//
//            configSaved = true;
//        }
    }   
}