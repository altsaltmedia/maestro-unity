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
                    for(int q=0; q<input.complexEventTriggerPackagers.Count; q++) {
                        PlayableDirector playableDirector = playable.GetGraph().GetResolver() as PlayableDirector;
                        input.complexEventTriggerPackagers[q].RaiseEvent(playableDirector.gameObject, playableDirector.gameObject.scene.name, string.Format("{0} director at {1}", playableDirector.gameObject.name, currentTime.ToString("F2")));
                        //input.complexEventTriggerPackagers[q].RaiseEvent(playable.GetGraph().GetEditorName() + " director at " + currentTime.ToString("F6"));
                    }
                } else {
                    if (currentTime >= input.endTime || currentTime < input.startTime) {
                        input.triggered = false;
                    }
                }
            }
        }
    }   
}