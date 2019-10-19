/***********************************************

Copyright © 2018 AltSalt Media, LLC.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using System.Linq;

namespace AltSalt
{    
    [HideInMenu]
    public class LerpToTargetTrack : TrackAsset {
    
        public void StoreClipProperties(GameObject directorObject)
        {
            foreach (var clip in GetClips()) {
                var myAsset = clip.asset as LerpToTargetClip;
                if (myAsset) {
                    myAsset.startTime = clip.start;
                    myAsset.endTime = clip.end;
                    myAsset.parentTrack = this;
                    myAsset.directorObject = directorObject;
                }
            }
        }

        public LerpToTargetMixerBehaviour StoreMixerProperties(GameObject go, LerpToTargetMixerBehaviour trackMixer)
        {
            trackMixer.directorObject = go;
    #if UNITY_EDITOR
            trackMixer.scrubberActive = Utils.GetBoolVariable(nameof(VarDependencies.ScrubberActive));
    #endif
            trackMixer.parentTrack = this;
            
            return trackMixer;
        }

        // This method should be overridden in child classes
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            StoreClipProperties(go);
            ScriptPlayable<LerpToTargetMixerBehaviour> trackPlayable = ScriptPlayable<LerpToTargetMixerBehaviour>.Create(graph, inputCount);
            LerpToTargetMixerBehaviour behaviour = trackPlayable.GetBehaviour();
            StoreMixerProperties(go, behaviour);

            //trackPlayable.GetBehaviour().markers = GetMarkers().ToList();
            return trackPlayable;
        }

        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            // #if UNITY_EDITOR
            //
            // SYNTAX FOR MARKING PROPERTIES TO NOT BE SAVED IN EDIT MODE, CAN ITERATE
            // THROUGH ALL PROPERTIES DYNAMICALLY OR USE DEBUG.LOG STATEMENT BELOW TO FIND
            // NAMES OF SERIALIZED PROPERTIES AND MARK THEM EXPLICITY
            //
            // GameObject trackBinding = director.GetGenericBinding(this) as GameObject;
            // if (trackBinding == null)
            //    return;
            //
            //var serializedObject = new UnityEditor.SerializedObject(trackBinding);
            //var iterator = serializedObject.GetIterator();

            //while (iterator.NextVisible(true)) {
            //    if (iterator.hasVisibleChildren)
            //        continue;

            //    Debug.Log(iterator.propertyPath);
            //    driver.AddFromName<GameObject>(trackBinding.gameObject, iterator.propertyPath);
            //}
            //
            //#endif

            base.GatherProperties(director, driver);
        }
    }    
}