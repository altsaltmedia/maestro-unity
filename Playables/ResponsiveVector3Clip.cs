using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

namespace AltSalt.Maestro
{
    [Serializable]
    public class ResponsiveVector3Clip : ResponsiveLerpToTargetClip
    {
        [FormerlySerializedAs("template")]
        [SerializeField]
        private ResponsiveVector3Behaviour _template = new ResponsiveVector3Behaviour();

        public ResponsiveVector3Behaviour template
        {
            get => _template;
            set => _template = value;
        }
        
        public override LerpToTargetBehaviour templateReference => template;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            template.parentTrack = parentTrack;
            template.clipAsset = this;
            template.trackAssetConfig = trackAssetConfig;

            var playable = ScriptPlayable<ResponsiveVector3Behaviour>.Create(graph, template);
            return playable;
        }
    }
}