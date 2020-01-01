using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

namespace AltSalt.Maestro
{
    [Serializable]
    public class ResponsiveFloatClip : ResponsiveLerpToTargetClip
    {
        [FormerlySerializedAs("template")]
        [SerializeField]
        private ResponsiveFloatBehaviour _template = new ResponsiveFloatBehaviour();

        public ResponsiveFloatBehaviour template
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

            var playable = ScriptPlayable<ResponsiveFloatBehaviour>.Create(graph, template);
            return playable;
        }
    }
}