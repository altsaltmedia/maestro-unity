using System;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt
{
    [Serializable]
    public class ResponsiveVector3Clip : ResponsiveLerpToTargetClip
    {
        public ResponsiveVector3Behaviour template = new ResponsiveVector3Behaviour();

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            template.appSettings = appSettings;
            template.parentTrack = parentTrack;
            template.clipAsset = this;
            template.directorObject = directorObject;

            var playable = ScriptPlayable<ResponsiveVector3Behaviour>.Create(graph, template);
            return playable;
        }
    }
}