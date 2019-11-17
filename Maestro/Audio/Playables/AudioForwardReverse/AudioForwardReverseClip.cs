using System;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt.Maestro.Audio
{
    [Serializable]
    public class AudioForwardReverseClip : LerpToTargetClip
    {
        public new AudioForwardReverseBehaviour template = new AudioForwardReverseBehaviour ();

        public BoolReference isReversing;
        public FloatReference frameStepValue;
        public FloatReference swipeModifierOutput;

        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            template.isReversing.Variable = isReversing.Variable;
            template.frameStepValue.Variable = frameStepValue.Variable;
            template.swipeModifier.Variable = swipeModifierOutput.Variable;

            var playable = ScriptPlayable<AudioForwardReverseBehaviour>.Create(graph, template);
            return playable;
        }
    }
}