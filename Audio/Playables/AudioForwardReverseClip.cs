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
        
        public override LerpToTargetBehaviour templateReference => template;

        [NonSerialized]
        private BoolVariable _isReversingVariable;

        public BoolVariable isReversingVariable
        {
            get => _isReversingVariable;
            set => _isReversingVariable = value;
        }

        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            template.timelineInstanceConfig = timelineInstanceConfig;
            template._isReversingVariable = isReversingVariable;

            var playable = ScriptPlayable<AudioForwardReverseBehaviour>.Create(graph, template);
            return playable;
        }
    }
}