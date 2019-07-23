using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEditor.Timeline;
using TMPro;

namespace AltSalt
{
    public static class TrackClipCreation
    {
        public static PageBuilderReferences pageBuilderReferences;
        static List<TrackData> copiedTracks = new List<TrackData>();

        public static List<TrackData> CopyTracks(PlayableDirector sourceDirector, UnityEngine.Object[] sourceObjects)
        {
            List<TrackData> tracksToCopy = new List<TrackData>();
            for (int i = 0; i < sourceObjects.Length; i++) {
                if (sourceObjects[i] is TrackAsset) {
                    List<TrackData> trackData = GetTrackData(sourceDirector, sourceObjects[i] as TrackAsset);
                    tracksToCopy.AddRange(trackData);
                }
            }
            return tracksToCopy;
        }

        static List<TrackData> GetTrackData(PlayableDirector sourceDirector, TrackAsset sourceTrack)
        {
            List<TrackData> trackData = new List<TrackData>();
            UnityEngine.Object sourceObject = new UnityEngine.Object();
            foreach (PlayableBinding playableBinding in sourceTrack.outputs) {
                sourceObject = sourceDirector.GetGenericBinding(playableBinding.sourceObject);
            }
            trackData.Add(new TrackData(sourceTrack, sourceObject, sourceTrack.GetClips()));
            foreach(TrackAsset childTrack in sourceTrack.GetChildTracks()) {
                trackData.AddRange(GetTrackData(sourceDirector, childTrack));
            }
            return trackData;
        }

        public static TrackAsset[] PasteTracks(TimelineAsset targetTimelineAsset, PlayableDirector targetDirector, UnityEngine.Object[] destinationSelection, List<TrackData> sourceTrackData)
        {
            TrackAsset[] pastedTracks = new TrackAsset[sourceTrackData.Count];
            TrackAsset parentTrack = GetDestinationTrackFromSelection(destinationSelection);

            // Paste tracks
            for (int i = 0; i < sourceTrackData.Count; i++) {
                TrackAsset trackAsset = targetTimelineAsset.CreateTrack(sourceTrackData[i].trackType, parentTrack, sourceTrackData[i].trackName);
                pastedTracks[i] = trackAsset;

                foreach (PlayableBinding playableBinding in trackAsset.outputs) {
                    targetDirector.SetGenericBinding(playableBinding.sourceObject, sourceTrackData[i].trackBinding);
                }
                foreach (TimelineClip trackClip in sourceTrackData[i].trackClips) {
                    TimelineClip pastedClip = trackAsset.CreateDefaultClip();

                    pastedClip.duration = trackClip.duration;
                    pastedClip.start = trackClip.start;

                    Type assetType = pastedClip.asset.GetType();
                    FieldInfo[] assetFields = assetType.GetFields();

                    foreach(FieldInfo assetField in assetFields) {
                        assetField.SetValue(pastedClip.asset, assetField.GetValue(trackClip.asset));
                    }
                }
            }

            // Set groups if applicable
            for (int i = 0; i < sourceTrackData.Count; i++) {
                if (sourceTrackData[i].groupTrack != null) {
                    for (int q = 0; q < pastedTracks.Length; q++) {
                        if (pastedTracks[q].name == sourceTrackData[i].groupTrack.name) {
                            pastedTracks[i].SetGroup(pastedTracks[q] as GroupTrack);
                        }
                    }
                }
            }

            TimelineUtilitiesCore.RefreshTimelineContentsAddedOrRemoved();

            return pastedTracks;
        }

        static bool ObjectsSelected()
        {
            if (Selection.objects.Length > 0) {
                return true;
            } else {
                return false;
            }
        }

        static bool TracksSelected()
        {
            for (int i = 0; i < Selection.objects.Length; i++) {
                if (Selection.objects[i] is TrackAsset) {
                    return true;
                }
            }
            return false;
        }

        public static TrackAsset[] TriggerCreateTrack(TimelineAsset targetTimelineAsset, PlayableDirector targetDirector, UnityEngine.Object[] sourceObjects, Type trackType, UnityEngine.Object[] destinationSelection)
        {
            TrackAsset[] trackAssets = new TrackAsset[sourceObjects.Length];
            TrackAsset parentTrack = GetDestinationTrackFromSelection(destinationSelection);

            if (sourceObjects.Length > 0) {
                for (int i = 0; i < sourceObjects.Length; i++) {
                    TrackAsset newTrack = CreateNewTrack(targetTimelineAsset, parentTrack, trackType);
                    trackAssets[i] = newTrack;
                    PopulateTrackAsset(targetDirector, newTrack, sourceObjects[i]);
                }
            } else {
                CreateNewTrack(targetTimelineAsset, parentTrack, trackType);
            }
            TimelineUtilitiesCore.RefreshTimelineContentsAddedOrRemoved();
            return trackAssets;
        }

        public static TrackAsset[] TriggerCreateTrack(TimelineAsset targetTimelineAsset, PlayableDirector targetDirector, GameObject[] sourceGameObjects, Type trackType, UnityEngine.Object[] destinationSelection)
        {
            TrackAsset[] trackAssets = new TrackAsset[sourceGameObjects.Length];
            TrackAsset parentTrack = GetDestinationTrackFromSelection(destinationSelection);

            if (sourceGameObjects.Length > 0) {
                Array.Sort(sourceGameObjects, new Utils.GameObjectSort());
                for (int i = 0; i < sourceGameObjects.Length; i++) {
                    TrackAsset newTrack = CreateNewTrack(targetTimelineAsset, parentTrack, trackType);
                    trackAssets[i] = newTrack;
                    PopulateTrackAsset(targetDirector, newTrack, sourceGameObjects[i]);
                }
            } else {
                CreateNewTrack(targetTimelineAsset, parentTrack, trackType);
            }
            TimelineUtilitiesCore.RefreshTimelineContentsAddedOrRemoved();
            return trackAssets;
        }

        static TrackAsset CreateNewTrack(TimelineAsset targetTimelineAsset, TrackAsset parentTrack, Type trackType)
        {
            TrackAsset trackAsset;    
            trackAsset = targetTimelineAsset.CreateTrack(trackType, parentTrack, trackType.Name);
            return trackAsset;
        }

        static TrackAsset PopulateTrackAsset(PlayableDirector targetDirector, TrackAsset targetTrack, UnityEngine.Object targetObject)
        {
            foreach (PlayableBinding playableBinding in targetTrack.outputs) {

                switch(targetTrack.GetType().Name) {

                    case nameof(TMProColorTrack):
                        targetDirector.SetGenericBinding(playableBinding.sourceObject, ((GameObject)targetObject).GetComponent<TMP_Text>());
                        break;

                    case nameof(RectTransformPosTrack):
                        targetDirector.SetGenericBinding(playableBinding.sourceObject, ((GameObject)targetObject).GetComponent<RectTransform>());
                        break;

                    case nameof(SpriteColorTrack):
                        targetDirector.SetGenericBinding(playableBinding.sourceObject, ((GameObject)targetObject).GetComponent<SpriteRenderer>());
                        break;

                    case nameof(LerpFloatVarTrack):
                        targetDirector.SetGenericBinding(playableBinding.sourceObject, targetObject);
                        break;

                    default:
                        Debug.LogError("Track type not supported");
                        break;
                }
            }

            return targetTrack;
        }

        public static List<TimelineClip> CreateClips(PlayableDirector targetDirector, UnityEngine.Object[] selection, float duration)
        {
            List<TimelineClip> timelineClips = new List<TimelineClip>();
            for (int i = 0; i < selection.Length; i++) {
                if (selection[i] is TrackAsset) {
                    TrackAsset trackAsset = selection[i] as TrackAsset;
                    TimelineClip newClip = trackAsset.CreateDefaultClip();
                    newClip.start = TimelineUtilitiesCore.CurrentTime;
                    newClip.duration = duration;
                    PopulateClip(targetDirector, trackAsset, newClip);
                    timelineClips.Add(newClip);
                }
            }

            TimelineUtilitiesCore.RefreshTimelineContentsAddedOrRemoved();

            return timelineClips;
        }

        static PlayableAsset PopulateClip(PlayableDirector targetDirector, TrackAsset parentTrack, TimelineClip timelineClip)
		{
            UnityEngine.Object sourceObject = null;
            foreach (PlayableBinding playableBinding in parentTrack.outputs) {
                sourceObject = targetDirector.GetGenericBinding(playableBinding.sourceObject);
            }

            switch (timelineClip.asset.GetType().Name) {

                case nameof(TMProColorClip):
                    TMProColorClip tmProAsset = timelineClip.asset as TMProColorClip;
                    TMP_Text tmProObject = sourceObject as TMP_Text;
                    tmProAsset.template.initialColor = tmProObject.color;
                    tmProAsset.template.targetColor = tmProObject.color;
                    return tmProAsset;

                case nameof(RectTransformPosClip):
                    RectTransformPosClip rectTransAsset = timelineClip.asset as RectTransformPosClip;
                    RectTransform rectTransObject = sourceObject as RectTransform;
                    rectTransAsset.template.initialPosition = rectTransObject.anchoredPosition3D;
                    rectTransAsset.template.targetPosition = rectTransObject.anchoredPosition3D;
                    return rectTransAsset;

                case nameof(SpriteColorClip):
                    SpriteColorClip spriteAsset = timelineClip.asset as SpriteColorClip;
                    SpriteRenderer spriteObject = sourceObject as SpriteRenderer;
                    spriteAsset.template.initialColor = spriteObject.color;
                    spriteAsset.template.targetColor = spriteObject.color;
                    return spriteAsset;

                case nameof(LerpFloatVarClip):
                    LerpFloatVarClip lerpFloatVarClip = timelineClip.asset as LerpFloatVarClip;
                    FloatVariable floatVariable = sourceObject as FloatVariable;
                    lerpFloatVarClip.template.initialValue = floatVariable.Value;
                    lerpFloatVarClip.template.targetValue = floatVariable.Value;
                    return lerpFloatVarClip;
            }

            return null;
		}

        static TrackAsset GetDestinationTrackFromSelection(UnityEngine.Object[] destinationSelection)
        {
            TrackAsset destinationTrack = null;
            for (int q = 0; q < destinationSelection.Length; q++) {
                if (destinationSelection[q] is GroupTrack) {
                    destinationTrack = destinationSelection[q] as TrackAsset;
                    return destinationTrack;
                }
            }
            for (int q = 0; q < destinationSelection.Length; q++) {
                TrackAsset trackAsset = destinationSelection[q] as TrackAsset;
                if (trackAsset != null && trackAsset.parent is GroupTrack) {
                    destinationTrack = trackAsset.parent as TrackAsset;
                    return destinationTrack;
                }
            }
            return destinationTrack;
        }

        public static GroupTrack TriggerCreateGroupTrack(TimelineAsset targetTimelineAsset, UnityEngine.Object[] childSelection)
        {
            GroupTrack groupTrack = targetTimelineAsset.CreateTrack(typeof(GroupTrack), null, typeof(GroupTrack).Name) as GroupTrack;
            GroupTrack childGroup = null;

            for(int i=0; i<childSelection.Length; i++) {
                if(childSelection[i] is TrackAsset) {
                    TrackAsset childTrack = childSelection[i] as TrackAsset;
                    if(childGroup == null) {
                        childGroup = childTrack.GetGroup();
                    }
                    childTrack.SetGroup(groupTrack);
                }
            }

            if(childGroup != null) {
                groupTrack.SetGroup(childGroup);
            }

            TimelineUtilitiesCore.RefreshTimelineContentsAddedOrRemoved();

            return groupTrack;
        }

        public class TrackData
        {
            public TrackAsset trackAsset;
            public GroupTrack groupTrack;
            public Type trackType;
            public string trackName;
            public UnityEngine.Object trackBinding;
            public IEnumerable<TimelineClip> trackClips;

            public TrackData(TrackAsset trackAsset, UnityEngine.Object binding, IEnumerable<TimelineClip> trackClips)
            {
                this.trackAsset = trackAsset;
                this.groupTrack = trackAsset.GetGroup();
                this.trackType = trackAsset.GetType();
                this.trackName = trackAsset.name;
                this.trackBinding = binding;
                this.trackClips = trackClips;
            }
        }
    }
}
