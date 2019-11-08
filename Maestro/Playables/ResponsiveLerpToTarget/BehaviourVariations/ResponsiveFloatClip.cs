using System;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt.Maestro
{
    [Serializable]
    public class ResponsiveFloatClip : ResponsiveLerpToTargetClip
    {
        public ResponsiveFloatBehaviour template = new ResponsiveFloatBehaviour();

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