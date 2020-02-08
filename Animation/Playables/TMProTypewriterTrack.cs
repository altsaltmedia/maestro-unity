/***********************************************

Copyright © 2018 AltSalt Media, LLC.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using TMPro;

namespace AltSalt.Maestro.Animation
{
    [TrackColor(0.1981132f, 0.5f, 0.1065063f)]
    [TrackClipType(typeof(TMProTypewriterClip))]
    [TrackBindingType(typeof(TextMeshPro))]
    public class TMProTypewriterTrack : LerpToTargetTrack
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            StoreClipProperties(go);
            ScriptPlayable<TMProTypewriterMixerBehaviour> trackPlayable = ScriptPlayable<TMProTypewriterMixerBehaviour>.Create(graph, inputCount);
            TMProTypewriterMixerBehaviour behaviour = trackPlayable.GetBehaviour();
            StoreMixerProperties(go, behaviour);

            return trackPlayable;
        }
    }    
}