using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace AltSalt.Maestro
{
    [Serializable]
    public class FloatClip : LerpToTargetClip
    {
        [FormerlySerializedAs("template")]
        [SerializeField]
        private FloatBehaviour _template = new FloatBehaviour();

        public FloatBehaviour template
        {
            get => _template;
            set => _template = value;
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            template.appSettings = appSettings;
            template.parentTrack = parentTrack;
            template.clipAsset = this;
            template.directorObject = directorObject;
            
            var playable = ScriptPlayable<FloatBehaviour>.Create(graph, template);
            return playable;
        }
    }
}
