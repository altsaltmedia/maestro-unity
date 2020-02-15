using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt.Maestro
{    
    public class SimpleEventTimelineTriggerMixerBehaviour : LerpToTargetMixerBehaviour
    {
        private ScriptPlayable<SimpleEventTimelineTriggerBehaviour> inputPlayable;
        private SimpleEventTimelineTriggerBehaviour input;
        
        private bool _internalIsReversingVal = false;

        private bool internalIsReversingVal
        {
            get => _internalIsReversingVal;
            set => _internalIsReversingVal = value;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {   
            inputCount = playable.GetInputCount ();

            // If there's been a change in direction, reset all of the clips 
            if (trackAssetConfig.isReversingVariable.value != internalIsReversingVal) {
                internalIsReversingVal = trackAssetConfig.isReversingVariable.value;
                
                for (int i = 0; i < inputCount; i++) {
                    inputWeight = playable.GetInputWeight(i);
                    inputPlayable = (ScriptPlayable<SimpleEventTimelineTriggerBehaviour>) playable.GetInput(i);
                    input = inputPlayable.GetBehaviour();

                    input.triggered = false;
                }
            }

            if (trackAssetConfig.isReversingVariable.value == false) {
                for (int i = 0; i < inputCount; i++)
                {
                    inputWeight = playable.GetInputWeight(i);
                    inputPlayable = (ScriptPlayable<SimpleEventTimelineTriggerBehaviour>)playable.GetInput(i);
                    input = inputPlayable.GetBehaviour ();

                    if (inputWeight >= 1 && input.triggered == false) {
                        input.triggered = true;
                        TriggerEvents(input);
                    } else {
                        if (trackAssetConfig.currentTime > input.endTime
                            && input.forceActivateOnForward == true && input.triggered == false) {
                            input.triggered = true;
                            TriggerEvents(input);
                        }
                    }
                }
            }
            else {
                // When we're reversing, we need to make sure all
                // the events get executed in reverse order as well
                for (int i = inputCount - 1; i >= 0; i--) {
                    inputWeight = playable.GetInputWeight(i);
                    inputPlayable = (ScriptPlayable<SimpleEventTimelineTriggerBehaviour>) playable.GetInput(i);
                    input = inputPlayable.GetBehaviour();

                    if (inputWeight >= 1 && input.triggered == false) {
                        input.triggered = true;
                        TriggerEvents(input);
                    } else {
                        if (trackAssetConfig.currentTime < input.startTime
                            && input.forceActivateOnReverse == true && input.triggered == false) {
                            input.triggered = true;
                            TriggerEvents(input);
                        }
                    }
                }
            }
        }

        private static SimpleEventTimelineTriggerBehaviour TriggerEvents(SimpleEventTimelineTriggerBehaviour triggerBehaviour)
        {
            for (int q = 0; q < triggerBehaviour.simpleEventTriggers.Count; q++) {
                triggerBehaviour.simpleEventTriggers[q].RaiseEvent(triggerBehaviour.trackAssetConfig.gameObject,
                    $"{triggerBehaviour.trackAssetConfig.name} director at {triggerBehaviour.trackAssetConfig.currentTime:F2}");
            }

            return triggerBehaviour;
        }
    }   
}