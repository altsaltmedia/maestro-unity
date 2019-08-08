/***********************************************

Copyright © 2018 AltSalt Media, LLC.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using TMPro;

namespace AltSalt
{
    [TrackColor(0.1981132f, 0.5f, 0.1065063f)]
    [TrackClipType(typeof(TMProTypewriterClip))]
    [TrackBindingType(typeof(TextMeshPro))]
    public class TMProTypewriterTrack : LerpToTargetTrack
    {
        // This method should be overridden in child classes
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            StoreClipStartEndTime();
            return ScriptPlayable<TMProTypewriterMixerBehaviour>.Create(graph, inputCount);
        }

        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
#if UNITY_EDITOR
            //TextMeshPro trackBinding = director.GetGenericBinding(this) as TextMeshPro;
            // if (trackBinding == null)
            //    return;

            //var serializedObject = new UnityEditor.SerializedObject(trackBinding);
            //var iterator = serializedObject.GetIterator();

            //while (iterator.NextVisible(true)) {
            //    if (iterator.hasVisibleChildren)
            //        continue;

            //    driver.AddFromName<TextMeshPro>(trackBinding.gameObject, iterator.propertyPath);
            //    Debug.Log(iterator.propertyPath);
            //}
#endif
            base.GatherProperties(director, driver);
        }
    }    
}