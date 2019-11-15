using System;
using UnityEngine;
using UnityEngine.Playables;

namespace AltSalt.Maestro.Animation
{
    [Serializable]
    public class ImageUIColorClip : LerpToTargetClip
    {
        public new ImageUIColorBehaviour template = new ImageUIColorBehaviour ();
        
        public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
        {
            template.startTime = startTime;
            template.endTime = endTime;
            var playable = ScriptPlayable<ImageUIColorBehaviour>.Create (graph, template);
            return playable;
        }
    }   
}