using UnityEngine;
using UnityEngine.Playables;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AltSalt
{    
    public class AxisSwitchMixerBehaviour : PlayableBehaviour
    {
        // Utility vars - specified here to prevent garbage collection
        double currentTime;
        protected int inputCount;
        protected float inputWeight;
        protected float modifier;
//
//        ScriptPlayable<SimpleSwitchBehaviour> inputPlayable;
//        SimpleSwitchBehaviour input;

        public override void PrepareFrame(Playable playable, FrameData info)
        {
//            base.PrepareFrame(playable, info);
//            currentTime = playable.GetGraph().GetRootPlayable(0).GetTime();
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {   
//            inputCount = playable.GetInputCount ();
//            
//            for (int i = 0; i < inputCount; i++)
//            {
//                inputWeight = playable.GetInputWeight(i);
//                inputPlayable = (ScriptPlayable<SimpleSwitchBehaviour>)playable.GetInput(i);
//                input = inputPlayable.GetBehaviour ();
//
//                if (input.switchType == SwitchType.SimpleSwitch && input.axisSwitch != null) {
//
//                    input.axisSwitch.SwitchEnabled = true;
//                    input.axisSwitch.AxisInflectionPoint = input.midPoint;
//
//                } else if (input.switchType == SwitchType.ForkSwitch && input.forkSwitch != null) {
//
//                    input.forkSwitch.SwitchEnabled = true;
//                    input.forkSwitch.ForkInflectionPoint = (float)input.endTime;
//
//                } else if (input.switchType == SwitchType.InvertSwitch && input.invertSwitch != null) {
//
//                    input.invertSwitch.SwitchEnabled = true;
//                    input.invertSwitch.AxisInflectionPoint = input.midPoint;
//                }
//            }
        }

        //public override void OnGraphStop(Playable playable)
        //{
        //    if (input.axisSwitch != null) {

        //        input.axisSwitch.SwitchEnabled = false;

        //    } else if (input.forkSwitch != null) {

        //        input.forkSwitch.SwitchEnabled = false;

        //    } else if (input.invertSwitch != null) {

        //        input.invertSwitch.SwitchEnabled = false;
        //    }

        //    base.OnGraphStop(playable);
        //}
    }   
}