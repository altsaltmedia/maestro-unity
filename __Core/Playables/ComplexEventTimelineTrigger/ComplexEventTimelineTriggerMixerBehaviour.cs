using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt
{    
    public class ComplexEventTimelineTriggerMixerBehaviour : PlayableBehaviour
    {
        // Utility vars - specified here to prevent garbage collection
        double currentTime;
        protected int inputCount;
        protected float inputWeight;
        protected float modifier;

        ComplexEvent trackBinding;
        ScriptPlayable<ComplexEventTimelineTriggerBehaviour> inputPlayable;
        ComplexEventTimelineTriggerBehaviour input;

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            base.PrepareFrame(playable, info);
            currentTime = playable.GetGraph().GetRootPlayable(0).GetTime();
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            inputCount = playable.GetInputCount ();
            
            for (int i = 0; i < inputCount; i++)
            {
                inputWeight = playable.GetInputWeight(i);
                inputPlayable = (ScriptPlayable<ComplexEventTimelineTriggerBehaviour>)playable.GetInput(i);
                input = inputPlayable.GetBehaviour ();
                
                if (inputWeight > 0 && input.triggered == false) {
                    input.triggered = true;
                    input.complexEventTrigger.RaiseEvent(playable.GetGraph().GetEditorName() + " director at " + currentTime.ToString("F6"));
                } else {
                    if (currentTime >= input.endTime) {
                        input.triggered = false;
                    }
                    //else if (i == 0 && currentTime <= input.startTime) {
                    //    triggered = false;
                    //}
                }
            }
        }
    }   
}