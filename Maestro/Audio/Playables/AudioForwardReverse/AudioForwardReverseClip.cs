using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Audio
{
    [Serializable]
    public class AudioForwardReverseClip : LerpToTargetClip
    {
        [FormerlySerializedAs("template")]
        [SerializeField]
        private AudioForwardReverseBehaviour _template = new AudioForwardReverseBehaviour ();

        private AudioForwardReverseBehaviour template
        {
            get => _template;
            set => _template = value;
        }
        
        [SerializeField]
        private BoolReference _isReversing;

        public BoolReference isReversing
        {
            get => _isReversing;
            set => _isReversing = value;
        }

        [SerializeField]
        private FloatReference _frameStepValue;

        public FloatReference frameStepValue
        {
            get => _frameStepValue;
            set => _frameStepValue = value;
        }

        [SerializeField]
        private FloatReference _swipeModifierOutput;

        public FloatReference swipeModifierOutput
        {
            get => _swipeModifierOutput;
            set => _swipeModifierOutput = value;
        }

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