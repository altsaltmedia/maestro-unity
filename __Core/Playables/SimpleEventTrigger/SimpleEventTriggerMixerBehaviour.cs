using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt
{    
    public class SimpleEventTriggerMixerBehaviour : PlayableBehaviour
    {
        // Utility vars - specified here to prevent garbage collection
        double currentTime;
        protected int inputCount;
        protected float inputWeight;
        protected float modifier;

        SimpleEvent trackBinding;
        ScriptPlayable<SimpleEventTriggerBehaviour> inputPlayable;
        SimpleEventTriggerBehaviour input;

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            base.PrepareFrame(playable, info);
            currentTime = playable.GetGraph().GetRootPlayable(0).GetTime();
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            trackBinding = playerData as SimpleEvent;
            
            if (!trackBinding)
                return;
            
            inputCount = playable.GetInputCount ();
            
            for (int i = 0; i < inputCount; i++)
            {
                inputWeight = playable.GetInputWeight(i);
                inputPlayable = (ScriptPlayable<SimpleEventTriggerBehaviour>)playable.GetInput(i);
                input = inputPlayable.GetBehaviour ();

                if (inputWeight > 0 && input.triggered == false) {
                    input.triggered = true;
                    trackBinding.Raise();
                } else {
                    if (currentTime >= input.endTime) {
                        input.triggered = false;
                    }
                    //else if (i == 0 && currentTime <= input.startTime) {
                    //    input.triggered = false;
                    //}
                }
            }
        }
    }   
}