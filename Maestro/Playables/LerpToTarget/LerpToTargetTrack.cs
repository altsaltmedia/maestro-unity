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

                MigrateClip(clip, directorObject);
            }
        }

        private void MigrateClip(TimelineClip clip, GameObject directorObject)
        {
            if (clip.asset is ImageUIColorClip imageUiColorClip) {

                if (imageUiColorClip.migrated == false) {
                    TimelineClip migratedClip = clip.parentTrack.CreateClip<ColorClip>();
                    ColorClip migratedClipAsset = migratedClip.asset as ColorClip;

                    migratedClip.duration = clip.duration;
                    migratedClip.start = clip.start;

                    migratedClipAsset.template.ease = imageUiColorClip.template.ease;
                    migratedClipAsset.template.initialValue = imageUiColorClip.template.initialValue;
                    migratedClipAsset.template.targetValue = imageUiColorClip.template.targetValue;
                        
                    imageUiColorClip.migrated = true;
                }

                if (imageUiColorClip.migrated == true) {
                    TimelineAsset directorAsset = directorObject.GetComponent<PlayableDirector>().playableAsset as TimelineAsset;
                    directorAsset.DeleteClip(clip);
                }
            }

            if (clip.asset is LerpColorVarClip lerpColorVarClip) {

                if (lerpColorVarClip.migrated == false) {
                    TimelineClip migratedClip = clip.parentTrack.CreateClip<ColorClip>();
                    ColorClip migratedClipAsset = migratedClip.asset as ColorClip;

                    migratedClip.duration = clip.duration;
                    migratedClip.start = clip.start;

                    migratedClipAsset.template.ease = lerpColorVarClip.template.ease;
                    migratedClipAsset.template.initialValue = lerpColorVarClip.template.initialValue;
                    migratedClipAsset.template.targetValue = lerpColorVarClip.template.targetValue;
                        
                    lerpColorVarClip.migrated = true;
                }

                if (lerpColorVarClip.migrated == true) {
                    TimelineAsset directorAsset = directorObject.GetComponent<PlayableDirector>().playableAsset as TimelineAsset;
                    directorAsset.DeleteClip(clip);
                }
            }
            
            if (clip.asset is LerpFloatVarClip lerpFloatVarClip) {

                if (lerpFloatVarClip.migrated == false) {
                    TimelineClip migratedClip = clip.parentTrack.CreateClip<FloatClip>();
                    FloatClip migratedClipAsset = migratedClip.asset as FloatClip;

                    migratedClip.duration = clip.duration;
                    migratedClip.start = clip.start;

                    migratedClipAsset.template.ease = lerpFloatVarClip.template.ease;
                    migratedClipAsset.template.initialValue = lerpFloatVarClip.template.initialValue;
                    migratedClipAsset.template.targetValue = lerpFloatVarClip.template.targetValue;
                        
                    lerpFloatVarClip.migrated = true;
                }

                if (lerpFloatVarClip.migrated == true) {
                    TimelineAsset directorAsset = directorObject.GetComponent<PlayableDirector>().playableAsset as TimelineAsset;
                    directorAsset.DeleteClip(clip);
                }
            }

            if (clip.asset is RectTransformPosClip rectTransformPosClip) {

                if (rectTransformPosClip.migrated == false) {
                    TimelineClip migratedClip = clip.parentTrack.CreateClip<ResponsiveVector3Clip>();
                    ResponsiveVector3Clip migratedClipAsset = migratedClip.asset as ResponsiveVector3Clip;

                    migratedClip.duration = clip.duration;
                    migratedClip.start = clip.start;

                    migratedClipAsset.template.ease = rectTransformPosClip.template.ease;
                    migratedClipAsset.template.breakpointInitialValue.Add(rectTransformPosClip.template.initialValue);
                    migratedClipAsset.template.breakpointTargetValue.Add(rectTransformPosClip.template.targetValue);
                        
                    rectTransformPosClip.migrated = true;
                }

                if (rectTransformPosClip.migrated == true) {
                    TimelineAsset directorAsset = directorObject.GetComponent<PlayableDirector>().playableAsset as TimelineAsset;
                    directorAsset.DeleteClip(clip);
                }
            }

            if (this is ResponsiveRectTransformPosTrack && clip.asset is ResponsiveVector3Clip) {
                
                if (migrated == false) {
                    PlayableDirector currentDirector = directorObject.GetComponent<PlayableDirector>(); 
                    TimelineAsset directorAsset = currentDirector.playableAsset as TimelineAsset;
                    TrackAsset migratedTrack = directorAsset.CreateTrack(typeof(RectTransformPosTrack), null, nameof(RectTransformPosTrack));

                    UnityEngine.Object trackBinding = new UnityEngine.Object();
                    
                    foreach (PlayableBinding playableBinding in this.outputs) {
                        trackBinding = currentDirector.GetGenericBinding(playableBinding.sourceObject);
                    }

                    foreach (PlayableBinding playableBinding in migratedTrack.outputs) {
                        currentDirector.SetGenericBinding(playableBinding.sourceObject, trackBinding);
                    }
                    
                    foreach (TimelineClip trackClip in this.GetClips()) {
                        ResponsiveVector3Clip originalClipAsset = trackClip.asset as ResponsiveVector3Clip;
                        TimelineClip migratedClip = migratedTrack.CreateClip<ResponsiveVector3Clip>();
                        ResponsiveVector3Clip migratedClipAsset = migratedClip.asset as ResponsiveVector3Clip;

                        migratedClip.duration = trackClip.duration;
                        migratedClip.start = trackClip.start;
                        
                        Type assetType = migratedClip.asset.GetType();
                        FieldInfo[] assetFields = assetType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                        // Need to create a copy of the asset, otherwise the new clip
                        // will use references to the old clip's asset values
                        var trackClipAssetCopy = Instantiate(trackClip.asset);

                        foreach (FieldInfo assetField in assetFields) {

                            assetField.SetValue(migratedClip.asset, assetField.GetValue(trackClipAssetCopy));                        
                        }
                        // This value should come over automatically
                        //migratedClipAsset.template.ease = originalClipAsset.template.responsiveEase;
                    }

                    GroupTrack groupTrack = this.GetGroup();

                    if (groupTrack != null) {
                        List<TrackAsset> revisedTrackList = new List<TrackAsset>();
                        revisedTrackList.AddRange(groupTrack.GetChildTracks());
                        
                        for (int q = 0; q < revisedTrackList.Count; q++) {
                            if (revisedTrackList[q] == this) {
                                revisedTrackList.Insert(q + 1, migratedTrack);
                            }
                        }
                        
                        for (int z = 0; z < revisedTrackList.Count; z++) {
                            revisedTrackList[z].SetGroup(null);
                            revisedTrackList[z].SetGroup(groupTrack);
                        }

                    }
                    
                    migrated = true;
                }

                if (migrated == true) {
                    TimelineAsset directorAsset = directorObject.GetComponent<PlayableDirector>().playableAsset as TimelineAsset;
                    directorAsset.DeleteTrack(this);
                    TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);
                    TimelineEditor.Refresh(RefreshReason.WindowNeedsRedraw);
                }
            }
            
            if (clip.asset is RectTransformRotationClip rectTransformRotationClip) {

                if (rectTransformRotationClip.migrated == false) {
                    TimelineClip migratedClip = clip.parentTrack.CreateClip<ResponsiveVector3Clip>();
                    ResponsiveVector3Clip migratedClipAsset = migratedClip.asset as ResponsiveVector3Clip;

                    migratedClip.duration = clip.duration;
                    migratedClip.start = clip.start;

                    migratedClipAsset.template.ease = rectTransformRotationClip.template.ease;
                    migratedClipAsset.template.breakpointInitialValue.Add(rectTransformRotationClip.template.initialValue);
                    migratedClipAsset.template.breakpointTargetValue.Add(rectTransformRotationClip.template.targetValue);
                        
                    rectTransformRotationClip.migrated = true;
                }

                if (rectTransformRotationClip.migrated == true) {
                    TimelineAsset directorAsset = directorObject.GetComponent<PlayableDirector>().playableAsset as TimelineAsset;
                    directorAsset.DeleteClip(clip);
                }
            }
            
            if (clip.asset is RectTransformScaleClip rectTransformScaleClip) {

                if (rectTransformScaleClip.migrated == false) {
                    TimelineClip migratedClip = clip.parentTrack.CreateClip<ResponsiveVector3Clip>();
                    ResponsiveVector3Clip migratedClipAsset = migratedClip.asset as ResponsiveVector3Clip;

                    migratedClip.duration = clip.duration;
                    migratedClip.start = clip.start;

                    migratedClipAsset.template.ease = rectTransformScaleClip.template.ease;
                    migratedClipAsset.template.breakpointInitialValue.Add(rectTransformScaleClip.template.initialValue);
                    migratedClipAsset.template.breakpointTargetValue.Add(rectTransformScaleClip.template.targetValue);
                        
                    rectTransformScaleClip.migrated = true;
                }

                if (rectTransformScaleClip.migrated == true) {
                    TimelineAsset directorAsset = directorObject.GetComponent<PlayableDirector>().playableAsset as TimelineAsset;
                    directorAsset.DeleteClip(clip);
                }
            }

            if (this is ResponsiveRectTransformScaleTrack &&
                clip.asset is ResponsiveVector3Clip) {

                if (migrated == false) {
                    PlayableDirector currentDirector = directorObject.GetComponent<PlayableDirector>();
                    TimelineAsset directorAsset = currentDirector.playableAsset as TimelineAsset;
                    TrackAsset migratedTrack = directorAsset.CreateTrack(typeof(RectTransformScaleTrack), null,
                        nameof(RectTransformPosTrack));

                    UnityEngine.Object trackBinding = new UnityEngine.Object();

                    foreach (PlayableBinding playableBinding in this.outputs) {
                        trackBinding = currentDirector.GetGenericBinding(playableBinding.sourceObject);
                    }

                    foreach (PlayableBinding playableBinding in migratedTrack.outputs) {
                        currentDirector.SetGenericBinding(playableBinding.sourceObject, trackBinding);
                    }

                    foreach (TimelineClip trackClip in this.GetClips()) {
                        ResponsiveVector3Clip originalClipAsset = trackClip.asset as ResponsiveVector3Clip;
                        TimelineClip migratedClip = migratedTrack.CreateClip<ResponsiveVector3Clip>();
                        ResponsiveVector3Clip migratedClipAsset = migratedClip.asset as ResponsiveVector3Clip;

                        migratedClip.duration = trackClip.duration;
                        migratedClip.start = trackClip.start;

                        Type assetType = migratedClip.asset.GetType();
                        FieldInfo[] assetFields =
                            assetType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                        // Need to create a copy of the asset, otherwise the new clip
                        // will use references to the old clip's asset values
                        var trackClipAssetCopy = Instantiate(trackClip.asset);

                        foreach (FieldInfo assetField in assetFields) {

                            assetField.SetValue(migratedClip.asset, assetField.GetValue(trackClipAssetCopy));
                        }
                        // This value should come over automatically
                        //migratedClipAsset.template.ease = originalClipAsset.template.responsiveEase;
                    }

                    GroupTrack groupTrack = this.GetGroup();

                    if (groupTrack != null) {
                        List<TrackAsset> revisedTrackList = new List<TrackAsset>();
                        revisedTrackList.AddRange(groupTrack.GetChildTracks());

                        for (int q = 0; q < revisedTrackList.Count; q++) {
                            if (revisedTrackList[q] == this) {
                                revisedTrackList.Insert(q + 1, migratedTrack);
                            }
                        }

                        for (int z = 0; z < revisedTrackList.Count; z++) {
                            revisedTrackList[z].SetGroup(null);
                            revisedTrackList[z].SetGroup(groupTrack);
                        }

                    }

                    migrated = true;
                }
                
                if (migrated == true) {
                    TimelineAsset directorAsset = directorObject.GetComponent<PlayableDirector>().playableAsset as TimelineAsset;
                    directorAsset.DeleteTrack(this);
                    TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);
                    TimelineEditor.Refresh(RefreshReason.WindowNeedsRedraw);
                }
            }
            
            if (clip.asset is SpriteColorClip spriteColorClip) {

                if (spriteColorClip.migrated == false) {
                    TimelineClip migratedClip = clip.parentTrack.CreateClip<ColorClip>();
                    ColorClip migratedClipAsset = migratedClip.asset as ColorClip;

                    migratedClip.duration = clip.duration;
                    migratedClip.start = clip.start;
                    
                    migratedClipAsset.template.ease = spriteColorClip.template.ease;
                    migratedClipAsset.template.initialValue = spriteColorClip.template.initialValue;
                    migratedClipAsset.template.targetValue = spriteColorClip.template.targetValue;
                        
                    spriteColorClip.migrated = true;
                }

                if (spriteColorClip.migrated == true) {
                    TimelineAsset directorAsset = directorObject.GetComponent<PlayableDirector>().playableAsset as TimelineAsset;
                    directorAsset.DeleteClip(clip);
                }
            }
            
            if (clip.asset is TMProColorClip tmProColorClip) {

                if (tmProColorClip.migrated == false) {
                    TimelineClip migratedClip = clip.parentTrack.CreateClip<ColorClip>();
                    ColorClip migratedClipAsset = migratedClip.asset as ColorClip;

                    migratedClip.duration = clip.duration;
                    migratedClip.start = clip.start;

                    migratedClipAsset.template.ease = tmProColorClip.template.ease;
                    migratedClipAsset.template.initialValue = tmProColorClip.template.initialValue;
                    migratedClipAsset.template.targetValue = tmProColorClip.template.targetValue;
                        
                    tmProColorClip.migrated = true;
                }

                if (tmProColorClip.migrated == true) {
                    TimelineAsset directorAsset = directorObject.GetComponent<PlayableDirector>().playableAsset as TimelineAsset;
                    directorAsset.DeleteClip(clip);
                }
            }
            
            if (clip.asset is AudioLerpVolumeClip audioLerpVolumeClip) {

                if (audioLerpVolumeClip.migrated == false) {
                    TimelineClip migratedClip = clip.parentTrack.CreateClip<FloatClip>();
                    FloatClip migratedClipAsset = migratedClip.asset as FloatClip;

                    migratedClip.duration = clip.duration;
                    migratedClip.start = clip.start;

                    migratedClipAsset.template.ease = audioLerpVolumeClip.template.ease;
                    migratedClipAsset.template.initialValue = audioLerpVolumeClip.template.initialValue;
                    migratedClipAsset.template.targetValue = audioLerpVolumeClip.template.targetValue;
                        
                    audioLerpVolumeClip.migrated = true;
                }

                if (audioLerpVolumeClip.migrated == true) {
                    TimelineAsset directorAsset = directorObject.GetComponent<PlayableDirector>().playableAsset as TimelineAsset;
                    directorAsset.DeleteClip(clip);
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