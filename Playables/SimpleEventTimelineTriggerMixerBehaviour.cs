using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt.Maestro
{    
    public class SimpleEventTimelineTriggerMixerBehaviour : PlayableBehaviour
    {
        // Utility vars - specified here to prevent garbage collection
        double currentTime;
        protected int inputCount;
        protected float inputWeight;
        protected float modifier;

        ScriptPlayable<SimpleEventTimelineTriggerBehaviour> inputPlayable;
        SimpleEventTimelineTriggerBehaviour input;

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
                inputPlayable = (ScriptPlayable<SimpleEventTimelineTriggerBehaviour>)playable.GetInput(i);
                input = inputPlayable.GetBehaviour ();

                if (currentTime >= input.startTime && currentTime <= input.endTime && input.triggered == false) {
                    if (input.isReversing == true && input.disableOnReverse == true) {
                        continue;
                    }
                    input.triggered = true;
                    for(int q=0; q<input.simpleEventTriggers.Count; q++) {
                        input.simpleEventTriggers[q].RaiseEvent(input.trackAssetConfig.gameObject, $"{input.trackAssetConfig.name} director at {currentTime:F2}");
                    }
                } else {
                    if (currentTime > input.endTime || currentTime < input.startTime) {
                        input.triggered = false;
                    }
                }
            }
        }
    }   
}