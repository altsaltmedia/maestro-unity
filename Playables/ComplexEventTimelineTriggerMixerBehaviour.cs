using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt.Maestro
{    
    public class ComplexEventTimelineTriggerMixerBehaviour : LerpToTargetMixerBehaviour
    {
        private ScriptPlayable<ComplexEventTimelineTriggerBehaviour> inputPlayable;
        private ComplexEventTimelineTriggerBehaviour input;
        
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
            if (timelineInstanceConfig.isReversingVariable.value != internalIsReversingVal) {
                internalIsReversingVal = timelineInstanceConfig.isReversingVariable.value;
                
                for (int i = 0; i < inputCount; i++) {
                    inputWeight = playable.GetInputWeight(i);
                    inputPlayable = (ScriptPlayable<ComplexEventTimelineTriggerBehaviour>) playable.GetInput(i);
                    input = inputPlayable.GetBehaviour();

                    input.triggered = false;
                }
            }

            if (timelineInstanceConfig.isReversingVariable.value == false) {
                for (int i = 0; i < inputCount; i++)
                {
                    inputWeight = playable.GetInputWeight(i);
                    inputPlayable = (ScriptPlayable<ComplexEventTimelineTriggerBehaviour>)playable.GetInput(i);
                    input = inputPlayable.GetBehaviour ();
                    
                    if (input.disableOnForward == true) {
                        continue;
                    }

                    if (inputWeight >= 1 && input.triggered == false) {
                        input.triggered = true;
                        TriggerEvents(this, input);
                    } else {
                        if (timelineInstanceConfig.currentTime > input.endTime
                            && input.forceActivateOnForward == true && input.triggered == false) {
                            input.triggered = true;
                            TriggerEvents(this, input);
                        }
                    }
                }
            }
            
            else {
                // When we're reversing, we need to make sure all
                // the events get executed in reverse order as well
                for (int i = inputCount - 1; i >= 0; i--) {
                    
                    inputWeight = playable.GetInputWeight(i);
                    inputPlayable = (ScriptPlayable<ComplexEventTimelineTriggerBehaviour>) playable.GetInput(i);
                    input = inputPlayable.GetBehaviour();
                    
                    if (input.disableOnReverse == true) {
                        continue;
                    }

                    if (inputWeight >= 1 && input.triggered == false) {
                        input.triggered = true;
                        TriggerEvents(this, input);
                    } else {
                        if (timelineInstanceConfig.currentTime < input.startTime
                            && input.forceActivateOnReverse == true && input.triggered == false) {
                            input.triggered = true;
                            TriggerEvents(this, input);
                        }
                    }
                }
            }
        }
        
        private static ComplexEventTimelineTriggerBehaviour TriggerEvents(
            ComplexEventTimelineTriggerMixerBehaviour mixerBehaviour,
            ComplexEventTimelineTriggerBehaviour triggerBehaviour)
        {
            if (mixerBehaviour.timelineInstanceConfig.connectedToSequence == true) {
                
                if (mixerBehaviour.timelineInstanceConfig.bookmarkLoadingCompleted == false &&
                    triggerBehaviour.executeWhileLoadingBookmarks == false) {
                    return triggerBehaviour;
                }

                if (mixerBehaviour.appUtilsRequested == true &&
                    triggerBehaviour.executeWhileAppUtilsRequested == false) {
                    return triggerBehaviour;
                }
                
            }
            
            for (int q = 0; q < triggerBehaviour.complexEventConfigurableTriggers.Count; q++) {
                
                if(triggerBehaviour.complexEventConfigurableTriggers[q].GetVariable() == null) continue;
                
                triggerBehaviour.complexEventConfigurableTriggers[q].RaiseEvent(triggerBehaviour.timelineInstanceConfig.gameObject,
                    $"{triggerBehaviour.timelineInstanceConfig.name} director at {triggerBehaviour.timelineInstanceConfig.currentTime:F2}");
            }

            return triggerBehaviour;
        }
    }   
}