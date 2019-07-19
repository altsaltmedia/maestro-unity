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

        static float newClipDuration = .5f;
        static bool allowBlankTracks = false;

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

                GUILayout.Space(10);

                EditorGUILayout.BeginHorizontal();
                    allowBlankTracks = EditorGUILayout.Toggle("Allow blank tracks", allowBlankTracks);
                    if (GUILayout.Button("Refresh")) {
                        // This button just here as a visual cue - any click inside window will refresh buttons
                    }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(5);

                EditorGUI.BeginDisabledGroup(DisableComponentTrackButton(Selection.gameObjects, typeof(TMP_Text), allowBlankTracks));
                    if (GUILayout.Button("New TextMeshPro Color Track(s)")) {
                        Selection.objects = TriggerCreateTrack(Selection.gameObjects, typeof(TMProColorTrack), Selection.objects);
                    }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(DisableComponentTrackButton(Selection.gameObjects, typeof(SpriteRenderer), allowBlankTracks));
                    if (GUILayout.Button("New Sprite Color Track(s)")) {
                        Selection.objects = TriggerCreateTrack(Selection.gameObjects, typeof(SpriteColorTrack), Selection.objects);
                    }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(DisableComponentTrackButton(Selection.gameObjects, typeof(RectTransform), allowBlankTracks));
                    if (GUILayout.Button("New RectTransform Position Track(s)")) {
                        Selection.objects = TriggerCreateTrack(Selection.gameObjects, typeof(RectTransformPosTrack), Selection.objects);
                    }
                EditorGUI.EndDisabledGroup();

                GUILayout.Space(10);

                EditorGUI.BeginDisabledGroup(DisableGroupTrackButton(Selection.gameObjects, allowBlankTracks));
                    if (GUILayout.Button("New Group Track(s)")) {
                        Selection.activeObject = TriggerCreateGroupTrack(Selection.objects);
                    }
                EditorGUI.EndDisabledGroup();

                GUILayout.Space(10);

                EditorGUI.BeginDisabledGroup(!TracksSelected());
                    EditorGUI.BeginChangeCheck();
                        newClipDuration = EditorGUILayout.FloatField("New clip duration :", newClipDuration);
                    if(EditorGUI.EndChangeCheck() == true && newClipDuration < 0) {
                        newClipDuration = 0;
                    }

                    if (GUILayout.Button("Create Clip(s)")) {
                        TimelineUtilitiesCore.timelineClips = CreateClips(Selection.objects, newClipDuration);
                        TimelineEditor.selectedClips = TimelineUtilitiesCore.timelineClips.ToArray();
                        TimelineEditor.Refresh(RefreshReason.ContentsModified);
                    }
                EditorGUI.EndDisabledGroup();
            }
        }

        public static List<TrackData> CopyTracks(UnityEngine.Object[] sourceObjects)
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

        public static TrackAsset[] PasteTracks(UnityEngine.Object[] destinationSelection, List<TrackData> sourceTrackData)
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

        public static TrackAsset[] TriggerCreateTrack(UnityEngine.Object[] sourceObjects, Type trackType, UnityEngine.Object[] destinationSelection)
        {
            TrackAsset[] trackAssets = new TrackAsset[sourceObjects.Length];
            TrackAsset parentTrack = GetDestinationTrackFromSelection(destinationSelection);

            if (sourceObjects.Length > 0) {
                for (int i = 0; i < sourceObjects.Length; i++) {
                    TrackAsset newTrack = CreateNewTrack(parentTrack, trackType);
                    trackAssets[i] = newTrack;
                    PopulateTrackAsset(newTrack, sourceObjects[i]);
                }
            } else {
                CreateNewTrack(parentTrack, trackType);
            }
            TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);
            return trackAssets;
        }

        public static TrackAsset[] TriggerCreateTrack(GameObject[] sourceGameObjects, Type trackType, UnityEngine.Object[] destinationSelection)
        {
            TrackAsset[] trackAssets = new TrackAsset[sourceGameObjects.Length];
            TrackAsset parentTrack = GetDestinationTrackFromSelection(destinationSelection);

            if (sourceGameObjects.Length > 0) {
                Array.Sort(sourceGameObjects, new Utils.GameObjectSort());
                for (int i = 0; i < sourceGameObjects.Length; i++) {
                    TrackAsset newTrack = CreateNewTrack(parentTrack, trackType);
                    trackAssets[i] = newTrack;
                    PopulateTrackAsset(newTrack, sourceGameObjects[i]);
                }
            } else {
                CreateNewTrack(parentTrack, trackType);
            }
            TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);
            return trackAssets;
        }

        static TrackAsset CreateNewTrack(TrackAsset parentTrack, Type trackType)
        {
            TrackAsset trackAsset;    
            trackAsset = TimelineEditor.inspectedAsset.CreateTrack(trackType, parentTrack, trackType.Name);
            return trackAsset;
        }

        static TrackAsset PopulateTrackAsset(TrackAsset targetTrack, UnityEngine.Object targetObject)
        {
            foreach (PlayableBinding playableBinding in targetTrack.outputs) {

                switch(targetTrack.GetType().Name) {

                    case nameof(TMProColorTrack):
                        TimelineEditor.inspectedDirector.SetGenericBinding(playableBinding.sourceObject, ((GameObject)targetObject).GetComponent<TMP_Text>());
                        break;

                    case nameof(RectTransformPosTrack):
                        TimelineEditor.inspectedDirector.SetGenericBinding(playableBinding.sourceObject, ((GameObject)targetObject).GetComponent<RectTransform>());
                        break;

                    case nameof(SpriteColorTrack):
                        TimelineEditor.inspectedDirector.SetGenericBinding(playableBinding.sourceObject, ((GameObject)targetObject).GetComponent<SpriteRenderer>());
                        break;

                    case nameof(LerpFloatVarTrack):
                        TimelineEditor.inspectedDirector.SetGenericBinding(playableBinding.sourceObject, targetObject);
                        break;

                    default:
                        Debug.LogError("Track type not supported");
                        break;
                }
            }

            return targetTrack;
        }

        public static List<TimelineClip> CreateClips(UnityEngine.Object[] selection, float duration)
        {
            List<TimelineClip> timelineClips = new List<TimelineClip>();
            for (int i = 0; i < selection.Length; i++) {
                if (selection[i] is TrackAsset) {
                    TrackAsset trackAsset = selection[i] as TrackAsset;
                    TimelineClip newClip = trackAsset.CreateDefaultClip();
                    newClip.start = TimelineUtilitiesCore.CurrentTime;
                    newClip.duration = duration;
                    PopulateClip(trackAsset, newClip);
                    timelineClips.Add(newClip);
                }
            }

            TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);

            return timelineClips;
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

        public static GroupTrack TriggerCreateGroupTrack(UnityEngine.Object[] childSelection)
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

        static bool DisableComponentTrackButton(GameObject[] currentSelection, Type targetType, bool overrideValue)
        {
            if(overrideValue == true) {
                return false;
            }

            return !TargetComponentSelected(currentSelection, targetType);
        }

        static bool DisableGroupTrackButton(GameObject[] currentSelection, bool overrideValue)
        {
            if (overrideValue == true) {
                return false;
            }

            return TargetTypeSelected(currentSelection, typeof(TrackAsset));
        }

        static bool TargetComponentSelected(GameObject[] currentSelection, Type targetType)
        {
            for(int i=0; i<currentSelection.Length; i++) {
                if(currentSelection[i].GetComponent(targetType) != null) {
                    return true;
                }
            }
            return false;
        }

        static bool TargetTypeSelected(GameObject[] currentSelection, Type targetType)
        {
            for (int i = 0; i < currentSelection.Length; i++) {
                if (currentSelection[i].GetType().Name == targetType.Name) {
                    return true;
                }
            }
            return false;
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
