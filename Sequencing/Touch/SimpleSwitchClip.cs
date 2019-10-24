using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Sirenix.OdinInspector;

namespace AltSalt.Sequencing.Touch
{
    [Serializable]
    public class SimpleSwitchClip : LerpToTargetClip
    {
        private SimpleSwitchBehaviour _template = new SimpleSwitchBehaviour();

        public SimpleSwitchBehaviour template
        {
            get => _template;
            set => _template = value;
        }

        [SerializeField]
        private SimpleSwitchType _switchType;

        public SimpleSwitchType switchType
        {
            get => _switchType;
            set => _switchType = value;
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;

            template.switchType = switchType;
            
            var playable = ScriptPlayable<SimpleSwitchBehaviour>.Create(graph, template);
            return playable;
        }
    }
}