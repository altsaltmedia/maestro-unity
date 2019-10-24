using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt.Sequencing.Touch
{
    public class InvertSwitchClip : LerpToTargetClip
    {
        [SerializeField]
        private InvertSwitchBehaviour _template = new InvertSwitchBehaviour();

        public InvertSwitchBehaviour template
        {
            get => _template;
            set => _template = value;
        }

        [SerializeField]
        private InvertSwitchType _switchType;

        public InvertSwitchType switchType
        {
            get => _switchType;
            set => _switchType = value;
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;

            template.switchType = switchType;
            
            var playable = ScriptPlayable<InvertSwitchBehaviour>.Create(graph, template);
            return playable;
        }
    }
}