using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace AltSalt.Maestro
{
    [Serializable]
    public class ColorClip : LerpToTargetClip
    {
        [SerializeField]
        private ColorBehaviour _template = new ColorBehaviour();

        public ColorBehaviour template
        {
            get => _template;
            set => _template = value;
        }

        [SerializeField]
        private bool _migrated;

        public bool migrated
        {
            get => _migrated;
            set => _migrated = value;
        }

        public override LerpToTargetBehaviour templateReference => template;
        
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            template.parentTrack = parentTrack;
            template.clipAsset = this;
            template.trackAssetConfig = trackAssetConfig;
            
            var playable = ScriptPlayable<ColorBehaviour>.Create(graph, template);
            return playable;
        }
        
    }
}