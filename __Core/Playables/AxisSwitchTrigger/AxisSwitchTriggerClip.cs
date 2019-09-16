using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [Serializable]
    public class AxisSwitchTriggerClip : LerpToTargetClip
    {
        public AxisSwitchTriggerBehaviour template = new AxisSwitchTriggerBehaviour();

        public SwitchType switchType;

        [ShowIf(nameof(switchType), SwitchType.AxisSwitch)]
        public ExposedReference<AxisSwitch> axisSwitch = new ExposedReference<AxisSwitch>();

        [ShowIf(nameof(switchType), SwitchType.ForkSwitch)]
        public ExposedReference<ForkSwitch> forkSwitch = new ExposedReference<ForkSwitch>();

        [ShowIf(nameof(switchType), SwitchType.InvertSwitch)]
        public ExposedReference<InvertSwitch> invertSwitch = new ExposedReference<InvertSwitch>();

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            template.midPoint = (float)(endTime + startTime) / 2f;

            template.switchType = switchType;
            
            AxisSwitch axisSwitchObject = axisSwitch.Resolve(graph.GetResolver());
            if (axisSwitchObject != null) {
                template.axisSwitch = axisSwitchObject;
            }

            ForkSwitch forkSwitchObject = forkSwitch.Resolve(graph.GetResolver());
            if (forkSwitchObject != null) {
                template.forkSwitch = forkSwitchObject;
            }

            InvertSwitch invertSwitchObject = invertSwitch.Resolve(graph.GetResolver());
            if (invertSwitchObject != null) {
                template.invertSwitch = invertSwitchObject;
            }

            var playable = ScriptPlayable<AxisSwitchTriggerBehaviour>.Create(graph, template);
            return playable;
        }
    }
}