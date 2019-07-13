using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEditor.Timeline;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace AltSalt
{
    public static class ClipTools
    {
        public static int selectionCount = 1;

        static SerializedObject serializedObject;

        static IntegerField newSelectionCount;

        static bool callTransposeUnselectedClips = false;
        static float durationMultiplier = 1;
        static float targetDuration = 1;
        static float targetSpacing = 0;
        static bool initialized = false;

        static Rect windowPosition;
        static VisualElement windowRoot;
        static TimelineUtilitiesWindow parentWindow;

        public delegate TimelineClip[] TransposeClipsCallback(TimelineClip[] selectedClips, TimelineClip[] sourceClips, double offset, double timeReference);

        static void RenderClipTools(Rect position, VisualElement rootVisualElement)
        {
            windowPosition = position;
            windowRoot = rootVisualElement;
            serializedObject = new SerializedObject(parentWindow);

            Debug.Log(parentWindow.newSelectionCount);

            AssetDatabase.Refresh();

            // Reference to the root of the window.
            var root = rootVisualElement;

            

            root.Clear();

            // Associates a stylesheet to our root. Thanks to inheritance, all root’s
            // children will have access to it.

            root.styleSheets.Add(Resources.Load<StyleSheet>("TimelineUtilities_Style"));

            // Loads and clones our VisualTree (eg. our UXML structure) inside the root.
            var clipToolsTree = Resources.Load<VisualTreeAsset>("TimelineUtilities_ClipTools");
            VisualElement clipToolsFromXML = clipToolsTree.CloneTree();

            root.Add(clipToolsFromXML);
            root.Bind(serializedObject);
            
            Button refreshButton = root.Query<Button>("RefreshWindow");
            refreshButton.clickable.clicked += () => RenderClipTools(windowPosition, windowRoot);

            IntegerField selectionCountField = root.Query<IntegerField>("SelectionCountField");
            newSelectionCount = selectionCountField;

            Button incrementButton = root.Query<Button>("IncrementSelectionCount");
            incrementButton.clickable.clicked += () => IncrementSelectionCount();

            Button decrementButton = root.Query<Button>("DecrementSelectionCount");
            decrementButton.clickable.clicked += () => DecrementSelectionCount();
        }

        static void IncrementSelectionCount()
        {
            Debug.Log(parentWindow.newSelectionCount);
            //newSelectionCount.value += 1;
        }

        static void DecrementSelectionCount()
        {
            //newSelectionCount.value -= 1;
        }

        public static void ShowClipTools(Rect position, VisualElement rootVisualElement, TimelineUtilitiesWindow parent)
        {
            if(initialized == false) {
                parentWindow = parent;
                RenderClipTools(position, rootVisualElement);
                initialized = true;
            }

            return;

            GUIStyle labelOffsetCenterStyle = new GUIStyle("Label");
            labelOffsetCenterStyle.contentOffset = new Vector2(0, 0);
            labelOffsetCenterStyle.alignment = TextAnchor.MiddleCenter;

            GUIStyle miniLabel = new GUIStyle("miniLabel");

            GUIStyle miniToggle = new GUIStyle("toggle");
            miniToggle.fontSize = miniLabel.fontSize;

            GUIStyle labelCenterStyle = new GUIStyle("Label");
            labelCenterStyle.alignment = TextAnchor.MiddleCenter;

            GUIStyle textFieldCenterStyle = new GUIStyle("textField");
            textFieldCenterStyle.alignment = TextAnchor.MiddleCenter;

            GUIStyle miniButton = new GUIStyle("miniButtonRight");
            miniButton.alignment = TextAnchor.MiddleCenter;

            GUIStyle centeredTitleStyle = Utils.AltSaltSkin.GetStyle("centeredTitle");
            GUIStyle infinityStyle = Utils.AltSaltSkin.GetStyle("infinitySymbol");

            GUILayout.Space(10);

            GUILayout.Label("selection", centeredTitleStyle);

            EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("<", miniButton)) {
                    TimelineEditor.selectedClips = SelectEndingBefore(Selection.objects);
                    TimelineUtilitiesCore.RefreshTimelineContentsModified();
                }
                if (GUILayout.Button("<|", miniButton)) {
                    TimelineEditor.selectedClips = SelectStartingBefore(Selection.objects);
                    TimelineUtilitiesCore.RefreshTimelineContentsModified();
                }
                GUILayout.Label("~", infinityStyle);
                if (GUILayout.Button("|>", miniButton)) {
                    TimelineEditor.selectedClips = SelectEndingAfter(Selection.objects);
                    TimelineUtilitiesCore.RefreshTimelineContentsModified();
                }
                if (GUILayout.Button(">", miniButton)) {
                    TimelineEditor.selectedClips = SelectStartingAfter(Selection.objects);
                    TimelineUtilitiesCore.RefreshTimelineContentsModified();
                }
                
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("<", miniButton)) {
                    TimelineEditor.selectedClips = AddPrevClipToSelection(TimelineEditor.selectedClips, TimelineUtilitiesCore.CurrentTime, selectionCount);
                    TimelineUtilitiesCore.RefreshTimelineContentsModified();
                }
                if (GUILayout.Button("-", labelOffsetCenterStyle, GUILayout.Width(25))) {
                    selectionCount -= 1;
                    if(selectionCount < 1) {
                        selectionCount = 1;
                    }
                }
                EditorGUI.BeginChangeCheck();
                    selectionCount = EditorGUILayout.IntField(selectionCount, textFieldCenterStyle, GUILayout.Width(25));
                if(EditorGUI.EndChangeCheck() == true && selectionCount < 1) {
                    selectionCount = 1;
                }
                if (GUILayout.Button("+", labelOffsetCenterStyle, GUILayout.Width(25))) {
                    selectionCount += 1;
                }
                if (GUILayout.Button(">", miniButton)) {
                    TimelineEditor.selectedClips = AddNextClipToSelection(TimelineEditor.selectedClips, TimelineUtilitiesCore.CurrentTime, selectionCount);
                    TimelineUtilitiesCore.RefreshTimelineContentsModified();
                }
                
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(15);

            GUILayout.Label("manipulation", centeredTitleStyle);

            GUILayout.Space(5);

            callTransposeUnselectedClips = EditorGUILayout.Toggle("transpose unselected clips :", callTransposeUnselectedClips, miniToggle);

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Time :", miniLabel, GUILayout.Width(position.width * .25f));

                GUILayout.Label(TimelineUtilitiesCore.CurrentTime.ToString("N"), labelCenterStyle, GUILayout.Width(position.width * .2f));

                if (GUILayout.Button("Set", miniButton, GUILayout.Width(position.width * .225f))) {
                    TimelineEditor.selectedClips = SetToPlayhead(TimelineEditor.selectedClips, TimelineUtilitiesCore.CurrentTime, new ClipTimeSort(), callTransposeUnselectedClips, GetTimelineClips().ToArray(), TransposeTargetClips);
                    TimelineUtilitiesCore.RefreshTimelineContentsModified();
                }

                if (GUILayout.Button("Set (<>)", miniButton, GUILayout.Width(position.width * .225f))) {
                    TimelineEditor.selectedClips = TransposeToPlayhead(TimelineEditor.selectedClips, TimelineUtilitiesCore.CurrentTime, new ClipTimeSort(), callTransposeUnselectedClips, GetTimelineClips().ToArray(), TransposeTargetClips);
                    TimelineUtilitiesCore.RefreshTimelineContentsModified();
                }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();

                GUILayout.Label("", GUILayout.Width(position.width * .25f));

                GUILayout.Label("", GUILayout.Width(position.width * .2f));

                if (GUILayout.Button("Expand", miniButton, GUILayout.Width(position.width * .225f))) {
                    TimelineEditor.selectedClips = TransposeToPlayhead(TimelineEditor.selectedClips, TimelineUtilitiesCore.CurrentTime, new ClipTimeSort(), callTransposeUnselectedClips, GetTimelineClips().ToArray(), TransposeTargetClips);
                    TimelineUtilitiesCore.RefreshTimelineContentsModified();
                }


                if (GUILayout.Button("Expand (<>)", miniButton, GUILayout.Width(position.width * .225f))) {
                    TimelineEditor.selectedClips = TransposeToPlayhead(TimelineEditor.selectedClips, TimelineUtilitiesCore.CurrentTime, new ClipTimeSort(), callTransposeUnselectedClips, GetTimelineClips().ToArray(), TransposeTargetClips);
                    TimelineUtilitiesCore.RefreshTimelineContentsModified();
                }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Multiply :", miniLabel, GUILayout.Width(position.width * .25f));

                EditorGUI.BeginChangeCheck();
                durationMultiplier = EditorGUILayout.FloatField(durationMultiplier, textFieldCenterStyle, GUILayout.Width(position.width * .2f));
                if (EditorGUI.EndChangeCheck() == true && durationMultiplier <= 0) {
                    durationMultiplier = 0.1f;
                }

                if (GUILayout.Button("X", miniButton, GUILayout.Width(position.width * .225f))) {
                    TimelineEditor.selectedClips = MultiplyClips(TimelineEditor.selectedClips, durationMultiplier, new ClipTimeSort(), callTransposeUnselectedClips, GetTimelineClips().ToArray(), TransposeTargetClips);
                    TimelineUtilitiesCore.RefreshTimelineContentsModified();
                }
                if (GUILayout.Button("X (<>)", miniButton, GUILayout.Width(position.width * .225f))) {
                    TimelineEditor.selectedClips = MultiplyAndTranspose(TimelineEditor.selectedClips, durationMultiplier, new ClipTimeSort(), callTransposeUnselectedClips, GetTimelineClips().ToArray(), TransposeTargetClips);
                    TimelineUtilitiesCore.RefreshTimelineContentsModified();
                }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Duration :", miniLabel, GUILayout.Width(position.width * .25f));

                EditorGUI.BeginChangeCheck();
                targetDuration = EditorGUILayout.FloatField(targetDuration, textFieldCenterStyle, GUILayout.Width(position.width * .2f));
                if (EditorGUI.EndChangeCheck() == true && targetDuration <= 0) {
                    targetDuration = 0.1f;
                }

                if (GUILayout.Button("Set", miniButton, GUILayout.Width(position.width * .225f))) {
                    TimelineEditor.selectedClips = SetDuration(TimelineEditor.selectedClips, targetDuration, new ClipTimeSort(), callTransposeUnselectedClips, GetTimelineClips().ToArray(), TransposeTargetClips);
                    TimelineUtilitiesCore.RefreshTimelineContentsModified();
                }
                if (GUILayout.Button("Set (<>)", miniButton, GUILayout.Width(position.width * .225f))) {
                    TimelineEditor.selectedClips = SetDurationAndTranspose(TimelineEditor.selectedClips, targetDuration, new ClipTimeSort(), callTransposeUnselectedClips, GetTimelineClips().ToArray(), TransposeTargetClips);
                    TimelineUtilitiesCore.RefreshTimelineContentsModified();
                }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Spacing :", miniLabel, GUILayout.Width(position.width * .25f));

                targetSpacing = EditorGUILayout.FloatField(targetSpacing, textFieldCenterStyle, GUILayout.Width(position.width * .2f));

                if (GUILayout.Button("Set", miniButton, GUILayout.Width(position.width * .225f))) {
                    SetSpacing(TimelineEditor.selectedClips, targetSpacing, new ClipTimeSort(), callTransposeUnselectedClips, GetTimelineClips().ToArray(), TransposeTargetClips);
                    TimelineUtilitiesCore.RefreshTimelineContentsModified();
                }
                if (GUILayout.Button("+ / -", miniButton, GUILayout.Width(position.width * .225f))) {
                    AddSubtractSpacing(TimelineEditor.selectedClips, GetTimelineClips().ToArray(), targetSpacing, new ClipTimeSort());
                    TimelineUtilitiesCore.RefreshTimelineContentsModified();
                }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Sequence", miniButton)) {
                    SetSequentialOrder(TimelineEditor.selectedClips, GetTimelineClips().ToArray(), new ClipTimeSequentialSort(GetAllTracks()), callTransposeUnselectedClips, TransposeTargetClips);
                    TimelineUtilitiesCore.RefreshTimelineContentsModified();
                }

                if (GUILayout.Button("Reverse", miniButton)) {
                    SetSequentialOrderReverse(TimelineEditor.selectedClips, GetTimelineClips().ToArray(), new ClipTimeSequentialSort(GetAllTracks()), callTransposeUnselectedClips, TransposeTargetClips);
                    TimelineUtilitiesCore.RefreshTimelineContentsModified();
                }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);

            if (GUILayout.Button("Deselect All", miniButton)) {
                DeselectAll();
            }

            GUILayout.Space(5);

            if (GUILayout.Button("Refresh Timeline Window", miniButton)) {
                TimelineUtilitiesCore.RefreshTimelineRedrawWindow();
            }

            GUILayout.Space(5);

            if (GUILayout.Button("Refresh Layout", miniButton)) {
                RenderClipTools(position, rootVisualElement);
            }
        }

        static void DeselectAll()
        {
            TimelineEditor.selectedClips = new TimelineClip[0];
            Selection.objects = new UnityEngine.Object[0];
            TimelineUtilitiesCore.RefreshTimelineContentsModified();
        }

        static TimelineClip[] SelectEndingBefore(Object[] selection)
        {
            List<TimelineClip> clipSelection = new List<TimelineClip>();
            foreach (TimelineClip clip in GetTimelineClips(selection)) {
                if (clip.end < TimelineUtilitiesCore.CurrentTime) {
                    clipSelection.Add(clip);
                }
            }

            return clipSelection.ToArray();
        }

        static TimelineClip[] SelectStartingAfter(Object[] selection)
        {
            List<TimelineClip> clipSelection = new List<TimelineClip>();
            foreach (TimelineClip clip in GetTimelineClips(selection)) {
                if (clip.start > TimelineUtilitiesCore.CurrentTime) {
                    clipSelection.Add(clip);
                }
            }

            return clipSelection.ToArray();
        }

        static TimelineClip[] SelectEndingAfter(Object[] selection)
        {
            List<TimelineClip> clipSelection = new List<TimelineClip>();
            foreach (TimelineClip clip in GetTimelineClips(selection)) {
                if (clip.end > TimelineUtilitiesCore.CurrentTime) {
                    clipSelection.Add(clip);
                }
            }

            return clipSelection.ToArray();
        }

        static TimelineClip[] SelectStartingBefore(Object[] selection)
        {
            List<TimelineClip> clipSelection = new List<TimelineClip>();
            foreach (TimelineClip clip in GetTimelineClips(selection)) {
                if (clip.start < TimelineUtilitiesCore.CurrentTime) {
                    clipSelection.Add(clip);
                }
            }

            return clipSelection.ToArray();
        }

        static TimelineClip[] AddNextClipToSelection(TimelineClip[] selectedClips, float timeReference, int clipCount = 0)
        {
            List<TimelineClip> newSelection = new List<TimelineClip>();
            newSelection.AddRange(selectedClips);

            for(int i=0; i<clipCount; i++) {
                TimelineClip nextClip = GetNextClip(newSelection, GetTimelineClips(), timeReference);
                newSelection.Add(nextClip);
            }

            return newSelection.ToArray();
        }

        static TimelineClip GetNextClip(IEnumerable<TimelineClip> selectedClips, IEnumerable<TimelineClip> sourceClips, float timeReference)
        {
            TimelineClip nextClip = null;

            List<TimelineClip> currentSelectionList = new List<TimelineClip>();
            currentSelectionList.AddRange(selectedClips);

            List<TimelineClip> sourceClipsList = new List<TimelineClip>();
            sourceClipsList.AddRange(sourceClips);

            for (int i = 0; i < sourceClipsList.Count; i++) {

                TimelineClip clipToCompare = sourceClipsList[i];

                if(currentSelectionList.Contains(clipToCompare) || clipToCompare.start < timeReference) {
                    continue;
                }

                if (nextClip == null) {
                    nextClip = clipToCompare;
                } else if (Mathf.Abs((float)clipToCompare.start - TimelineUtilitiesCore.CurrentTime) < Mathf.Abs((float)nextClip.start - TimelineUtilitiesCore.CurrentTime)) {
                    nextClip = clipToCompare;
                }

            }

            return nextClip;
        }

        static TimelineClip[] AddPrevClipToSelection(TimelineClip[] selectedClips, float timeReference, int clipCount = 0)
        {
            List<TimelineClip> newSelection = new List<TimelineClip>();
            newSelection.AddRange(selectedClips);

            for (int i = 0; i < clipCount; i++) {
                TimelineClip previousClip = GetPreviousClip(newSelection, GetTimelineClips(), timeReference);
                newSelection.Add(previousClip);
            }

            return newSelection.ToArray();
        }

        static TimelineClip GetPreviousClip(IEnumerable<TimelineClip> selectedClips, IEnumerable<TimelineClip> sourceClips, float timeReference)
        {
            TimelineClip previousClip = null;

            List<TimelineClip> currentSelectionList = new List<TimelineClip>();
            currentSelectionList.AddRange(selectedClips);

            List<TimelineClip> sourceClipsList = new List<TimelineClip>();
            sourceClipsList.AddRange(sourceClips);

            for (int i = 0; i < sourceClipsList.Count; i++) {

                TimelineClip clipToCompare = sourceClipsList[i];

                if (currentSelectionList.Contains(clipToCompare) || clipToCompare.start > timeReference) {
                    continue;
                }

                if (previousClip == null) {
                    previousClip = clipToCompare;
                } else if (Mathf.Abs((float)clipToCompare.start - TimelineUtilitiesCore.CurrentTime) < Mathf.Abs((float)previousClip.start - TimelineUtilitiesCore.CurrentTime)) {
                    previousClip = clipToCompare;
                }

            }

            return previousClip;
        }

        static List<TimelineClip> GetTimelineClips(Object[] selection)
        {
            List<TimelineClip> selectedTrackClips = new List<TimelineClip>();
            bool trackAssetSelected = false;

            if(selection != null && selection.Length > 0) {
                for (int i=0; i<selection.Length; i++) {
                    if(selection[i] is TrackAsset) {
                        trackAssetSelected = true;
                        TrackAsset trackAsset = selection[i] as TrackAsset;
                        if(trackAsset.GetChildTracks() != null) {
                            selectedTrackClips.AddRange(trackAsset.GetClips());
                        }
                    }
                }
            }

            if(trackAssetSelected == true) {
                return selectedTrackClips;
            } else {
                return GetTimelineClips();
            }
        }

        static List<TimelineClip> GetTimelineClips()
        {
            IEnumerable<PlayableBinding> playableBindings = TimelineEditor.inspectedAsset.outputs;

            List<TimelineClip> allClips = new List<TimelineClip>();

            foreach (PlayableBinding playableBinding in playableBindings) {
                TrackAsset trackAsset = playableBinding.sourceObject as TrackAsset;

                // Skip playable bindings that don't contain track assets (e.g. markers)
                if (trackAsset == null || trackAsset.hasClips == false) {
                    continue;
                }

                allClips.AddRange(trackAsset.GetClips());
            }
            return allClips;
        }

        static TimelineClip[] SetToPlayhead(TimelineClip[] selectedClips, float timeReference, Comparer<TimelineClip> comparer, bool executeTranposeCallback = false, TimelineClip[] sourceClips = null, TransposeClipsCallback transposeClipsCallback = null)
        {
            List<TimelineClip> selectedClipsList = new List<TimelineClip>();
            selectedClipsList.AddRange(selectedClips);
            selectedClipsList.Sort(comparer);

            double startTime = timeReference;
            double difference = 0;

            if (selectedClipsList.Count > 0) {
                startTime = selectedClipsList[0].start;
                difference = timeReference - selectedClipsList[0].start;
            }

            for (int i=0; i<selectedClipsList.Count; i++) {
                Undo.RecordObject(selectedClips[i].parentTrack, "set clip(s) start time");
                selectedClips[i].start = timeReference;
            }

            if(executeTranposeCallback == true) {
                transposeClipsCallback(selectedClips, sourceClips, difference, startTime);
            }

            return selectedClips;
        }

        static TimelineClip[] TransposeToPlayhead(TimelineClip[] selectedClips, float timeReference, Comparer<TimelineClip> comparer, bool executeTranposeCallback = false, TimelineClip[] sourceClips = null, TransposeClipsCallback transposeClipsCallback = null)
        {
            List<TimelineClip> selectedClipsList = new List<TimelineClip>();
            selectedClipsList.AddRange(selectedClips);
            selectedClipsList.Sort(comparer);

            double startTime = timeReference;
            double difference = 0;

            if (selectedClipsList.Count > 0) {
                startTime = selectedClipsList[0].start;
                difference = timeReference - selectedClipsList[0].start;
            }

            for (int i = 0; i < selectedClipsList.Count; i++) {
                Undo.RecordObject(selectedClipsList[i].parentTrack, "transpose clip(s) to start time");
                selectedClipsList[i].start += difference;

                if(i == selectedClipsList.Count - 1) {
                    selectedClipsList[0].start = timeReference;
                }
            }

            if (executeTranposeCallback == true) {
                transposeClipsCallback(selectedClips, sourceClips, difference, startTime);
            }

            return selectedClipsList.ToArray();
        }

        static TimelineClip[] MultiplyClips(TimelineClip[] selectedClips, float multiplier, Comparer<TimelineClip> comparer, bool executeTranposeCallback = false, TimelineClip[] sourceClips = null, TransposeClipsCallback transposeClipsCallback = null)
        {
            if (multiplier.Equals(0)) {
                Debug.LogWarning("Multiplying clips by 0! This is not allowed");
                return selectedClips;
            }

            double previousDifference = 0;

            List<TimelineClip> selectedClipsList = new List<TimelineClip>();
            selectedClipsList.AddRange(selectedClips);
            selectedClipsList.Sort(comparer);

            for (int i = 0; i < selectedClipsList.Count; i++) {

                TimelineClip selectedClip = selectedClipsList[i];
                double originalDuration = selectedClip.duration;

                Undo.RecordObject(selectedClip.parentTrack, "multiply clip(s)");
                selectedClip.duration *= multiplier;

                if (executeTranposeCallback == true) {

                    double adjustedDifference = selectedClip.duration - originalDuration;

                    // Subtract any overlap in multiplied clips so we only add newly created length to the offset
                    if (i != 0 && selectedClip.start < selectedClips[i - 1].end) {
                        adjustedDifference -= previousDifference;
                    }

                    transposeClipsCallback(selectedClips, sourceClips, adjustedDifference, selectedClip.start);
                    previousDifference = selectedClip.duration - originalDuration;
                }
            }

            return selectedClips;
        }

        static TimelineClip[] MultiplyAndTranspose(TimelineClip[] selectedClips, float multiplier, Comparer<TimelineClip> comparer, bool executeTranposeCallback = false, TimelineClip[] sourceClips = null, TransposeClipsCallback transposeClipsCallback = null)
        {
            if (multiplier.Equals(0)) {
                Debug.LogWarning("Multiplying clips by 0! This is not allowed");
                return selectedClips;
            }

            List<TimelineClip> selectedClipsList = new List<TimelineClip>();
            selectedClipsList.AddRange(selectedClips);
            selectedClipsList.Sort(comparer);

            for(int i=0; i<selectedClipsList.Count; i++) {
                TimelineClip selectedClip = selectedClipsList[i];

                double originalDuration = selectedClip.duration;

                Undo.RecordObject(selectedClip.parentTrack, "multiply and transpose clip(s)");
                selectedClip.duration *= multiplier;

                double difference = selectedClip.duration - originalDuration;

                if(executeTranposeCallback == false) {

                    for (int q = 0; q < selectedClipsList.Count; q++) {

                        TimelineClip clip = selectedClipsList[q];

                        if (clip == selectedClip) {
                            continue;
                        }

                        if (clip.start > selectedClip.start) {
                            Undo.RecordObject(clip.parentTrack, "multiply and transpose clip(s)");
                            clip.start += difference;
                        }
                    }

                } else {
                    TimelineClip[] selectedClipArray = { selectedClip };
                    transposeClipsCallback(selectedClipArray, sourceClips, difference, selectedClip.start);
                }
            }

            return selectedClips;
        }

        static TimelineClip[] SetDuration(TimelineClip[] selectedClips, float duration, Comparer<TimelineClip> comparer, bool executeTranposeCallback = false, TimelineClip[] sourceClips = null, TransposeClipsCallback transposeClipsCallback = null)
        {
            if (duration.Equals(0)) {
                Debug.LogWarning("Setting duration to 0! This is not allowed");
                return selectedClips;
            }

            double previousDifference = 0;

            List<TimelineClip> selectedClipsList = new List<TimelineClip>();
            selectedClipsList.AddRange(selectedClips);
            selectedClipsList.Sort(comparer);

            for (int i = 0; i < selectedClipsList.Count; i++) {

                TimelineClip selectedClip = selectedClipsList[i];
                double originalDuration = selectedClip.duration;

                Undo.RecordObject(selectedClip.parentTrack, "set clip(s) duration");
                selectedClip.duration = duration;

                if (executeTranposeCallback == true) {

                    double adjustedDifference = selectedClip.duration - originalDuration;

                    // Subtract any overlap in multiplied clips so we only add newly created length to the offset
                    if (i != 0 && selectedClip.start < selectedClips[i - 1].end) {
                        adjustedDifference -= previousDifference;
                    }

                    transposeClipsCallback(selectedClips, sourceClips, adjustedDifference, selectedClip.start);
                    previousDifference = selectedClip.duration - originalDuration;
                }
            }

            return selectedClips;
        }

        static TimelineClip[] SetDurationAndTranspose(TimelineClip[] selectedClips, float duration, Comparer<TimelineClip> comparer, bool executeTranposeCallback = false, TimelineClip[] sourceClips = null, TransposeClipsCallback transposeClipsCallback = null)
        {
            if (duration.Equals(0)) {
                Debug.LogWarning("Setting duration to 0! This is not allowed");
                return selectedClips;
            }

            List<TimelineClip> selectedClipsList = new List<TimelineClip>();
            selectedClipsList.AddRange(selectedClips);
            selectedClipsList.Sort(comparer);

            for (int i = 0; i < selectedClipsList.Count; i++) {

                TimelineClip selectedClip = selectedClipsList[i];

                double originalDuration = selectedClip.duration;

                Undo.RecordObject(selectedClip.parentTrack, "set duration and transpose clip(s)");
                selectedClip.duration = duration;

                double difference = selectedClip.duration - originalDuration;

                if (executeTranposeCallback == false) {

                    for (int q = 0; q < selectedClipsList.Count; q++) {

                        TimelineClip clip = selectedClipsList[q];

                        if (clip == selectedClip) {
                            continue;
                        }

                        if (clip.start > selectedClip.start) {
                            Undo.RecordObject(clip.parentTrack, "set duration and transpose clip(s)");
                            clip.start += difference;
                        }
                    }

                } else {
                    TimelineClip[] selectedClipArray = { selectedClip };
                    transposeClipsCallback(selectedClipArray, sourceClips, difference, selectedClip.start);
                }
            }

            return selectedClips;
        }

        static TimelineClip[] SetSequentialOrder(TimelineClip[] selectedClips, TimelineClip[] sourceClips, Comparer<TimelineClip> comparer, bool executeTranposeCallback = false, TransposeClipsCallback transposeClipsCallback = null)
        {
            List<TimelineClip> selectedClipsList = new List<TimelineClip>();
            selectedClipsList.AddRange(selectedClips);
            selectedClipsList.Sort(comparer);

            List<TimelineClip> sourceClipsList = new List<TimelineClip>();
            sourceClipsList.AddRange(sourceClips);
            sourceClipsList.Sort(comparer);

            double difference = 0;

            for (int i=0; i<selectedClipsList.Count; i++) {

                TimelineClip selectedClip = selectedClipsList[i];

                double newStartTime = selectedClip.end;

                for(int q=0; q<selectedClipsList.Count; q++) {

                    TimelineClip clip = selectedClipsList[q];

                    if (clip == selectedClip) {
                        continue;
                    }

                    if(i == 0 || clip.start > selectedClip.start || Equals(clip.start, selectedClip.start)) {
                        Undo.RecordObject(clip.parentTrack, "set clips sequentially");
                        difference += newStartTime - clip.start;
                        clip.start = newStartTime;                    
                    }
                }
            }

            if(executeTranposeCallback == true) {
                transposeClipsCallback(selectedClipsList.ToArray(), sourceClipsList.ToArray(), difference, selectedClipsList[0].end);
            }

            return selectedClips;
        }

        static TimelineClip[] SetSequentialOrderReverse(TimelineClip[] selectedClips, TimelineClip[] sourceClips, Comparer<TimelineClip> comparer, bool executeTranposeCallback = false, TransposeClipsCallback transposeClipsCallback = null)
        {
            List<TimelineClip> selectedClipsList = new List<TimelineClip>();
            selectedClipsList.AddRange(selectedClips);
            selectedClipsList.Sort(comparer);

            List<TimelineClip> sourceClipsList = new List<TimelineClip>();
            sourceClipsList.AddRange(sourceClips);
            sourceClipsList.Sort(comparer);

            double difference = 0;

            for (int i = selectedClipsList.Count - 1; i >= 0; i--) {

                TimelineClip selectedClip = selectedClipsList[i];

                double newStartTime = selectedClip.end;

                for (int q = 0; q < selectedClipsList.Count; q++) {

                    TimelineClip clip = selectedClipsList[q];

                    if (clip == selectedClip) {
                        continue;
                    }

                    if (i == selectedClipsList.Count - 1 || clip.start > selectedClip.start || Equals(clip.start, selectedClip.start)) {
                        Undo.RecordObject(clip.parentTrack, "set clips sequentially (reverse)");
                        difference += newStartTime - clip.start;
                        clip.start = newStartTime;
                    }
                }
            }

            if (executeTranposeCallback == true) {
                transposeClipsCallback(selectedClipsList.ToArray(), sourceClipsList.ToArray(), difference, selectedClipsList[selectedClipsList.Count - 1].end);
            }

            return selectedClips;
        }


        static List<TrackAsset> GetAllTracks()
        {
            List<TrackAsset> trackAssets = new List<TrackAsset>();
            foreach (TrackAsset rootTrack in TimelineEditor.inspectedAsset.GetRootTracks()) {
                trackAssets.AddRange(GetChildTracks(rootTrack));
            }
            return trackAssets;
        }

        static List<TrackAsset> GetChildTracks(TrackAsset trackAsset)
        {
            List<TrackAsset> trackAssets = new List<TrackAsset>();
            trackAssets.Add(trackAsset);
            foreach(TrackAsset childTrack in trackAsset.GetChildTracks()) {
                trackAssets.AddRange(GetChildTracks(childTrack));
            }
            return trackAssets;
        }

        static TimelineClip[] SetSpacing(TimelineClip[] selectedClips, float spacing, Comparer<TimelineClip> comparer, bool executeTranposeCallback = false, TimelineClip[] sourceClips = null, TransposeClipsCallback transposeClipsCallback = null)
        {
            List<TimelineClip> selectedClipsList = new List<TimelineClip>();
            selectedClipsList.AddRange(selectedClips);
            selectedClipsList.Sort(comparer);

            List<TimelineClip> sourceClipsList = new List<TimelineClip>();
            sourceClipsList.AddRange(sourceClips);
            sourceClipsList.Sort(comparer);

            for (int i = 0; i < selectedClipsList.Count; i++) {

                TimelineClip selectedClip = selectedClipsList[i];

                double endTimePlusSpacing = selectedClip.end + spacing;

                for (int q = 0; q < selectedClipsList.Count; q++) {

                    TimelineClip clip = selectedClipsList[q];

                    if (clip == selectedClip) {
                        continue;
                    }

                    double initialStartTime = clip.start;

                    if (clip.start > selectedClip.start || Equals(clip.start, selectedClip.start)) {
                        Undo.RecordObject(clip.parentTrack, "set clip(s) spacing");
                        clip.start = endTimePlusSpacing;

                        if (executeTranposeCallback == true) {
                            double difference = clip.start - initialStartTime;
                            transposeClipsCallback(selectedClipsList.ToArray(), sourceClipsList.ToArray(), difference, initialStartTime);
                        }
                    }
                }
            }

            return selectedClips;
        }

        static TimelineClip[] AddSubtractSpacing(TimelineClip[] selectedClips, TimelineClip[] sourceClips, float spacing, Comparer<TimelineClip> comparer)
        {
            List<TimelineClip> selectedClipsList = new List<TimelineClip>();
            selectedClipsList.AddRange(selectedClips);
            selectedClipsList.Sort(comparer);

            List<TimelineClip> sourceClipsList = new List<TimelineClip>();
            sourceClipsList.AddRange(sourceClips);
            sourceClipsList.Sort(comparer);

            for (int i = 0; i < selectedClipsList.Count; i++) {

                TimelineClip selectedClip = selectedClipsList[i];

                for (int q = 0; q < sourceClipsList.Count; q++) {

                    TimelineClip clip = sourceClipsList[q];

                    if (clip == selectedClip) {
                        continue;
                    }

                    if (clip.start > selectedClip.start || Equals(clip.start, selectedClip.start)) {
                        Undo.RecordObject(clip.parentTrack, "add / subtract clip(s) spacing");
                        clip.start += spacing;
                    }
                }
            }

            return selectedClips;
        }

        static TimelineClip[] TransposeTargetClips(TimelineClip[] omittedClips, TimelineClip[] targetClips, double offset, double timeReference)
        {
            List<TimelineClip> selectedClipsList = new List<TimelineClip>();
            selectedClipsList.AddRange(omittedClips);

            List<TimelineClip> sourceClipsList = new List<TimelineClip>();

            for (int i = 0; i < targetClips.Length; i++) {

                TimelineClip sourceClip = targetClips[i];

                if (selectedClipsList.Contains(sourceClip)) {
                    continue;
                }

                if (sourceClip.start > timeReference) {
                    Undo.RecordObject(sourceClip.parentTrack, "transpose target clip(s)");
                    sourceClip.start += offset;
                    sourceClipsList.Add(sourceClip);
                }
            }

            return sourceClipsList.ToArray();
        }

        public class ClipTimeSort : Comparer<TimelineClip>
        {
            public override int Compare(TimelineClip x, TimelineClip y)
            {
                return x.start.CompareTo(y.start);    
            }
        }

        public class ClipTimeSequentialSort : Comparer<TimelineClip>
        {
            List<TrackAsset> orderedTrackList = new List<TrackAsset>();

            public ClipTimeSequentialSort(List<TrackAsset> orderedTrackList)
            {
                this.orderedTrackList = orderedTrackList;
            }

            public override int Compare(TimelineClip x, TimelineClip y)
            {
                int xTrackPosition = 0;
                int yTrackPosition = 0;

                for (int i = 0; i < orderedTrackList.Count; i++) {
                    if (x.parentTrack == orderedTrackList[i]) {
                        xTrackPosition = i;
                    }
                    if (y.parentTrack == orderedTrackList[i]) {
                        yTrackPosition = i;
                    }
                }

                if (xTrackPosition.CompareTo(yTrackPosition) != 0) {
                    return xTrackPosition.CompareTo(yTrackPosition);
                } else {
                    return x.start.CompareTo(y.start);
                }
            }
        }
        
    }
}
