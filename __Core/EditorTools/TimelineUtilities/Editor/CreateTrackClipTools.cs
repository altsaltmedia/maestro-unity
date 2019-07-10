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
    public static class CreateTrackClipTools
    {
        public static PageBuilderReferences pageBuilderReferences;
        static List<TrackData> copiedTracks = new List<TrackData>();

        public static void ShowTrackClipTools()
        {
            var guiStyle = new GUIStyle("Label");
            guiStyle.fontStyle = FontStyle.Italic;
            guiStyle.alignment = TextAnchor.UpperCenter;

            if (TimelineEditor.inspectedAsset == null) {

                GUILayout.Label("Please select a timeline asset in order to use clip tools.", guiStyle);

            } else {

                EditorGUI.BeginDisabledGroup(!ObjectsSelected());
                if (GUILayout.Button("Copy Track(s)")) {
                    copiedTracks = CopyTracks(Selection.objects);
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(!ObjectsSelected());
                if (GUILayout.Button("Paste Track(s)")) {
                    PasteTracks(Selection.objects, copiedTracks);
                }
                EditorGUI.EndDisabledGroup();


                if (GUILayout.Button("New TextMeshPro Color Track(s)")) {
                    Selection.objects = TriggerCreateTrack(Selection.objects, typeof(TMProColorTrack), Selection.gameObjects);
                }

                if (GUILayout.Button("New Sprite Color Track(s)")) {
                    Selection.objects = TriggerCreateTrack(Selection.objects, typeof(SpriteColorTrack), Selection.gameObjects);
                }

                if (GUILayout.Button("New RectTransform Position Track(s)")) {
                    Selection.objects = TriggerCreateTrack(Selection.objects, typeof(RectTransformPosTrack), Selection.gameObjects);
                }

                if (GUILayout.Button("New Group Track(s)")) {
                    Selection.activeObject = TriggerCreateGroupTrack(Selection.objects);
                }

                GUILayout.Space(10);

                EditorGUI.BeginDisabledGroup(!TracksSelected());
                if (GUILayout.Button("Create Clip(s)")) {
                    TimelineEditor.selectedClips = CreateClips(Selection.objects);
                    TimelineEditor.Refresh(RefreshReason.ContentsModified);
                }
                EditorGUI.EndDisabledGroup();
            }
        }

        static List<TrackData> CopyTracks(UnityEngine.Object[] sourceObjects)
        {
            List<TrackData> tracksToCopy = new List<TrackData>();
            for (int i = 0; i < sourceObjects.Length; i++) {
                if (sourceObjects[i] is TrackAsset) {
                    List<TrackData> trackData = GetTrackData(sourceObjects[i] as TrackAsset);
                    tracksToCopy.AddRange(trackData);
                }
            }
            return tracksToCopy;
        }

        static List<TrackData> GetTrackData(TrackAsset sourceTrack)
        {
            List<TrackData> trackData = new List<TrackData>();
            UnityEngine.Object sourceObject = new UnityEngine.Object();
            foreach (PlayableBinding playableBinding in sourceTrack.outputs) {
                sourceObject = TimelineEditor.inspectedDirector.GetGenericBinding(playableBinding.sourceObject);
            }
            trackData.Add(new TrackData(sourceTrack, sourceObject, sourceTrack.GetClips()));
            foreach(TrackAsset childTrack in sourceTrack.GetChildTracks()) {
                trackData.AddRange(GetTrackData(childTrack));
            }
            return trackData;
        }

        static TrackAsset[] PasteTracks(UnityEngine.Object[] destinationSelection, List<TrackData> sourceTrackData)
        {
            TrackAsset[] pastedTracks = new TrackAsset[sourceTrackData.Count];
            TrackAsset parentTrack = GetDestinationTrackFromSelection(destinationSelection);

            // Paste tracks
            for (int i = 0; i < sourceTrackData.Count; i++) {
                TrackAsset trackAsset = TimelineEditor.inspectedAsset.CreateTrack(sourceTrackData[i].trackType, parentTrack, sourceTrackData[i].trackName);
                pastedTracks[i] = trackAsset;

                foreach (PlayableBinding playableBinding in trackAsset.outputs) {
                    TimelineEditor.inspectedDirector.SetGenericBinding(playableBinding.sourceObject, sourceTrackData[i].trackBinding);
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

            TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);

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

        static TrackAsset[] TriggerCreateTrack(UnityEngine.Object[] destinationSelection, Type trackType, GameObject[] targetGameObjects)
        {
            TrackAsset[] trackAssets = new TrackAsset[targetGameObjects.Length];
            TrackAsset parentTrack = GetDestinationTrackFromSelection(destinationSelection);

            if (targetGameObjects.Length > 0) {
                for (int i = 0; i < targetGameObjects.Length; i++) {
                    TrackAsset newTrack = CreateNewTrack(parentTrack, trackType);
                    trackAssets[i] = newTrack;
                    PopulateTrackAsset(newTrack, targetGameObjects[i]);
                }
            } else {
                CreateNewTrack(parentTrack, trackType);
            }
            TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);
            return trackAssets;
        }

        static TrackAsset CreateNewTrack(TrackAsset parentTrack, Type trackType)
        {
            TrackAsset trackAsset = TimelineEditor.inspectedAsset.CreateTrack(trackType, parentTrack, trackType.Name);
            return trackAsset;
        }

        static TrackAsset PopulateTrackAsset(TrackAsset targetTrack, GameObject targetGameObject)
        {
            foreach (PlayableBinding playableBinding in targetTrack.outputs) {
                TimelineEditor.inspectedDirector.SetGenericBinding(playableBinding.sourceObject, targetGameObject as UnityEngine.Object);
            }

            return targetTrack;
        }

        static TimelineClip[] CreateClips(UnityEngine.Object[] selection)
        {
            List<TimelineClip> timelineClips = new List<TimelineClip>();
            for (int i = 0; i < selection.Length; i++) {
                if (selection[i] is TrackAsset) {
                    TrackAsset trackAsset = selection[i] as TrackAsset;
                    TimelineClip newClip = trackAsset.CreateDefaultClip();
                    newClip.start = TimelineUtilitiesCore.CurrentTime;
                    PopulateClip(trackAsset, newClip);
                    timelineClips.Add(newClip);
                }
            }

            TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);

            return timelineClips.ToArray();
        }

        static PlayableAsset PopulateClip(TrackAsset parentTrack, TimelineClip timelineClip)
		{
            UnityEngine.Object sourceObject = null;
            foreach (PlayableBinding playableBinding in parentTrack.outputs) {
                sourceObject = TimelineEditor.inspectedDirector.GetGenericBinding(playableBinding.sourceObject);
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
            }

            return null;
		}

        static TrackAsset GetDestinationTrackFromSelection(UnityEngine.Object[] destinationSelection)
        {
            TrackAsset destinationTrack = null;
            for (int q = 0; q < destinationSelection.Length; q++) {
                if (destinationSelection[q] is TrackAsset) {
                    destinationTrack = destinationSelection[q] as TrackAsset;
                    break;
                }
            }
            return destinationTrack;
        }

        static GroupTrack TriggerCreateGroupTrack(UnityEngine.Object[] childSelection)
        {
            GroupTrack groupTrack = TimelineEditor.inspectedAsset.CreateTrack(typeof(GroupTrack), null, typeof(GroupTrack).Name) as GroupTrack;
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

            TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);

            return groupTrack;
        }

        class TrackData
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
