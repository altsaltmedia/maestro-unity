using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt.Maestro
{    
    public class SimpleEventTimelineTriggerMixerBehaviour : LerpToTargetMixerBehaviour
    {
        ScriptPlayable<SimpleEventTimelineTriggerBehaviour> inputPlayable;
        SimpleEventTimelineTriggerBehaviour input;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {   
            inputCount = playable.GetInputCount ();
            
            for (int i = 0; i < inputCount; i++)
            {
                inputWeight = playable.GetInputWeight(i);
                inputPlayable = (ScriptPlayable<SimpleEventTimelineTriggerBehaviour>)playable.GetInput(i);
                input = inputPlayable.GetBehaviour ();

                if (inputWeight >= 1 && input.triggered == false) {
                    if (input.isReversing == true && input.disableOnReverse == true) {
                        continue;
                    }
                    input.triggered = true;
                    for(int q=0; q<input.simpleEventTriggers.Count; q++) {
                        input.simpleEventTriggers[q].RaiseEvent(input.trackAssetConfig.gameObject, $"{input.trackAssetConfig.name} director at {trackAssetConfig.currentTime:F2}");
                    }
                } else {
                    if (trackAssetConfig.currentTime > input.endTime || trackAssetConfig.currentTime < input.startTime) {
                        input.triggered = false;
                    }
                }
            }
        }
    }   
}