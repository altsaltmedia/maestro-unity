/***********************************************

Copyright © 2018 AltSalt Media, LLC.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using System.Linq;
using System.Reflection;
using AltSalt.Maestro.Animation;
using AltSalt.Maestro.Audio;
using UnityEditor.Timeline;

namespace AltSalt.Maestro
{
    [Serializable]
    public abstract class LerpToTargetTrack : TrackAsset
    {
        [SerializeField]
        private bool _migrated;

        public bool migrated
        {
            get => _migrated;
            set => _migrated = value;
        }

//        This method should be overridden in child classes; a sample body is provided below
//        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
//        {
//            StoreClipProperties(go);
//            ScriptPlayable<LerpToTargetMixerBehaviour> trackPlayable = ScriptPlayable<LerpToTargetMixerBehaviour>.Create(graph, inputCount);
//            LerpToTargetMixerBehaviour behaviour = trackPlayable.GetBehaviour();
//            StoreMixerProperties(go, behaviour);
//
//            //trackPlayable.GetBehaviour().markers = GetMarkers().ToList();
//            return trackPlayable;
//        }
    
        public void StoreClipProperties(GameObject directorObject)
        {
            foreach (var clip in GetClips()) {
                var myAsset = clip.asset as LerpToTargetClip;
                if (myAsset) {
                    myAsset.startTime = clip.start;
                    myAsset.endTime = clip.end;
                    if (myAsset.appSettings == null) {
                        myAsset.appSettings = directorObject.GetComponent<TrackAssetConfig>().appSettings;
                    }
                    myAsset.parentTrack = this;
                    myAsset.directorObject = directorObject;
                }
            }
        }

        protected LerpToTargetMixerBehaviour StoreMixerProperties(GameObject go, LerpToTargetMixerBehaviour trackMixer)
        {
            trackMixer.directorObject = go;
            trackMixer._scrubberActive.Variable = go.GetComponent<TrackAssetConfig>().scrubberActive;
            trackMixer.parentTrack = this;
            return trackMixer;
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