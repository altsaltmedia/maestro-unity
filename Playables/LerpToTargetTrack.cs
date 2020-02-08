/***********************************************

Copyright © 2018 AltSalt Media, LLC.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using AltSalt.Maestro.Animation;

namespace AltSalt.Maestro
{
    [Serializable]
    public abstract class LerpToTargetTrack : TrackAsset
    {
        [SerializeField]
        private bool _requiredForSiblingSequence;

        public bool requiredForSiblingSequence
        {
            get => _requiredForSiblingSequence;
            set => _requiredForSiblingSequence = value;
        }

//        This method should be overridden in child classes; a sample body is provided below
//
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
    
        protected virtual void StoreClipProperties(GameObject directorObject)
        {
            foreach (var clip in GetClips()) {
                var myAsset = clip.asset as LerpToTargetClip;
                if (myAsset) {
                    myAsset.startTime = clip.start;
                    myAsset.endTime = clip.end;
                    myAsset.parentTrack = this;
                    myAsset.trackAssetConfig = directorObject.GetComponent<TrackAssetConfig>();
                    
                    // MIGRATION CALLS GO HERE
                    
                    // if (this is TMProColorTrack tmProColorTrack) {
                    //     tmProColorTrack.MigrateClip(clip, directorObject);
                    // }
                }
            }
        }

        // SAMPLE MIGRATION SCRIPT FOR CLIPS  
        
        // public void MigrateClip(TimelineClip clip, GameObject directorObject)
        // {
        //     if (clip.asset is ColorClip colorClip) {
        //
        //         if (colorClip.migrated == false) {
        //             TimelineClip migratedClip = clip.parentTrack.CreateClip<TMProColorClip>();
        //             TMProColorClip migratedClipAsset = migratedClip.asset as TMProColorClip;
        //
        //             migratedClip.duration = clip.duration;
        //             migratedClip.start = clip.start;
        //
        //             migratedClipAsset.template.ease = colorClip.template.ease;
        //             migratedClipAsset.template.initialValue = colorClip.template.initialValue;
        //             migratedClipAsset.template.targetValue = colorClip.template.targetValue;
        //                     
        //             colorClip.migrated = true;
        //         }
        //
        //         if (colorClip.migrated == true) {
        //             TimelineAsset directorAsset = directorObject.GetComponent<PlayableDirector>().playableAsset as TimelineAsset;
        //             directorAsset.DeleteClip(clip);
        //         }
        //     }
        // }

        protected LerpToTargetMixerBehaviour StoreMixerProperties(GameObject directorObject, LerpToTargetMixerBehaviour trackMixer)
        {
            var trackAssetConfig = directorObject.GetComponent<TrackAssetConfig>();
            trackMixer.trackAssetConfig = trackAssetConfig;
            trackMixer.appSettings = trackAssetConfig.appSettings;
            trackMixer.inputGroupKey = trackAssetConfig.inputGroupKey;
            trackMixer.parentTrack = this;
            return trackMixer;
        }

        // SYNTAX FOR MARKING PROPERTIES TO NOT BE SAVED IN EDIT MODE, CAN ITERATE
        // THROUGH ALL PROPERTIES DYNAMICALLY OR USE DEBUG.LOG STATEMENT BELOW TO FIND
        // NAMES OF SERIALIZED PROPERTIES AND MARK THEM EXPLICITY
        
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            // #if UNITY_EDITOR
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