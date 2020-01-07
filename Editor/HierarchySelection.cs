using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro
{
    public class HierarchySelection : ModuleWindow
    {
        protected override ModuleWindow Configure(ControlPanel controlPanel, string uxmlPath)
        {
            base.Configure(controlPanel, uxmlPath);

            toggleData.Clear();

            var buttons = moduleWindowUXML.Query<Button>();
            buttons.ForEach(SetupButton);

            UpdateDisplay();
            ControlPanel.selectionChangedDelegate += UpdateDisplay;

            return this;
        }
        
        void OnDestroy()
        {
            ControlPanel.selectionChangedDelegate -= UpdateDisplay;
        }
        
        private VisualElementToggleData _toggleData = new VisualElementToggleData();

        private VisualElementToggleData toggleData
        {
            get => _toggleData;
            set => _toggleData = value;
        }

        private UnityEngine.Object[] _savedSelection = new Object[0];

        private Object[] savedSelection
        {
            get => _savedSelection;
            set => _savedSelection = value;
        }

        private List<GameObject> _objectsToCopy = new List<GameObject>();

        private List<GameObject> objectsToCopy
        {
            get => _objectsToCopy;
            set => _objectsToCopy = value;
        }

        enum ButtonNames
        {
            SaveSelection,
            LoadSelection,
            SelectAllChildren,
            SelectAllParents,
            AddAllChildrenToSelection,
            AddAllParentsToSelection,
            ShallowCopyGameObjects,
            DeepCopyGameObjects,
            PasteGameObjects,
            SelectOddsInSelection,
            SelectEvensInSelection
        }
        
        enum EnableCondition
        {
            SelectionSaved,
            ObjectSelected,
            GameObjectSelected,
            GameObjectsCopied
        }

        private void UpdateDisplay()
        {
            if(Selection.objects.Length > 0) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.ObjectSelected, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.ObjectSelected, false);
            }
            
            if (savedSelection.Length > 0) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.SelectionSaved, true);
            }
            else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.SelectionSaved, false);
            }
            
            if(Selection.gameObjects.Length > 0) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.GameObjectSelected, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.GameObjectSelected, false);
            }
            
            if(objectsToCopy.Count > 0) {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.GameObjectsCopied, true);
            } else {
                ModuleUtils.ToggleVisualElements(toggleData, EnableCondition.GameObjectsCopied, false);
            }
        }

        private Button SetupButton(Button button)
        {
            switch (button.name) {

                case nameof(ButtonNames.SaveSelection):
                    button.clickable.clicked += () => { savedSelection = Selection.objects; };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.ObjectSelected,
                        button);
                    break;

                case nameof(ButtonNames.LoadSelection):
                    button.clickable.clicked += () => { Selection.objects = savedSelection; };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.SelectionSaved,
                        button);
                    break;

                case nameof(ButtonNames.SelectAllChildren):
                    button.clickable.clicked += () =>
                    {
                        List<GameObject> newSelection = new List<GameObject>();
                        GameObject[] gameObjectChildren = Utils.GetChildGameObjects(Selection.gameObjects);

                        newSelection.AddRange(gameObjectChildren);

                        Selection.objects = newSelection.ToArray();
                        ModuleUtils.FocusHierarchyWindow();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.GameObjectSelected,
                        button);
                    break;
                
                case nameof(ButtonNames.SelectAllParents):
                    button.clickable.clicked += () =>
                    {
                        List<GameObject> newSelection = new List<GameObject>();
                        GameObject[] gameObjectParents = Utils.GetParentGameObjects(Selection.gameObjects);

                        Selection.objects = gameObjectParents;
                        ModuleUtils.FocusHierarchyWindow();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.GameObjectSelected,
                        button);
                    break;

                case nameof(ButtonNames.AddAllChildrenToSelection):
                    button.clickable.clicked += () =>
                    {
                        List<GameObject> newSelection = new List<GameObject>();
                        GameObject[] gameObjectChildren = Utils.GetChildGameObjects(Selection.gameObjects);

                        newSelection.AddRange(Selection.gameObjects);
                        newSelection.AddRange(gameObjectChildren);

                        Selection.objects = newSelection.ToArray();
                        ModuleUtils.FocusHierarchyWindow();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.GameObjectSelected,
                        button);
                    break;
                
                case nameof(ButtonNames.AddAllParentsToSelection):
                    button.clickable.clicked += () =>
                    {
                        List<GameObject> newSelection = new List<GameObject>();
                        GameObject[] gameObjectsAndParents = Utils.GetParentGameObjects(Selection.gameObjects, true);

                        Selection.objects = gameObjectsAndParents;
                        ModuleUtils.FocusHierarchyWindow();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.GameObjectSelected,
                        button);
                    break;

                case nameof(ButtonNames.ShallowCopyGameObjects):
                    button.clickable.clicked += () =>
                    {
                        for (int z = 0; z < objectsToCopy.Count; z++) {
                            DestroyImmediate(objectsToCopy[z]);
                        }

                        objectsToCopy.Clear();
                        Array.Sort(Selection.gameObjects, new Utils.GameObjectSort());
                        for (int i = 0; i < Selection.gameObjects.Length; i++) {
                            GameObject copy = Utils.DuplicateGameObject(Selection.gameObjects[i], true);
                            copy.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild |
                                             HideFlags.HideInHierarchy;
                            Undo.RegisterCreatedObjectUndo(copy, "Copy game object");
                            objectsToCopy.Add(copy);
                        }
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.GameObjectSelected,
                        button);
                    break;
                
                case nameof(ButtonNames.DeepCopyGameObjects):
                    button.clickable.clicked += () =>
                    {
                        for (int z = 0; z < objectsToCopy.Count; z++) {
                            DestroyImmediate(objectsToCopy[z]);
                        }

                        objectsToCopy.Clear();
                        Array.Sort(Selection.gameObjects, new Utils.GameObjectSort());
                        
                        GameObject[] copiedHierarchy = Utils.DuplicateHierarchy(Selection.gameObjects);
                        for (int i = 0; i < copiedHierarchy.Length; i++) {
                            copiedHierarchy[i].hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild |
                                                           HideFlags.HideInHierarchy;    
                        }
                        objectsToCopy.AddRange(copiedHierarchy);
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.GameObjectSelected,
                        button);
                    break;

                case nameof(ButtonNames.PasteGameObjects):
                    button.clickable.clicked += () =>
                    {
                        List<GameObject> newSelection = new List<GameObject>();
                            
                        for (int i = 0; i < Selection.gameObjects.Length; i++) {
                            
                            GameObject[] pastedHierarchy = Utils.DuplicateHierarchy(objectsToCopy.ToArray());

                            for (int j = 0; j < pastedHierarchy.Length; j++) {
                                newSelection.Add(pastedHierarchy[j]);
                                pastedHierarchy[j].hideFlags = HideFlags.None;
                            }

                            GameObject[] rootObjects = Utils.GetRootGameObjects(Utils.SortGameObjectSelection(pastedHierarchy));

                            for (int j = 0; j < rootObjects.Length; j++) {
                                Undo.SetTransformParent(rootObjects[j].transform, Selection.gameObjects[i].transform,
                                    "Set parent on pasted object");    
                            }
                        }

                        Selection.objects = newSelection.ToArray();
                        ModuleUtils.FocusHierarchyWindow();
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.GameObjectsCopied,
                        button);
                    break;
                
                case nameof(ButtonNames.SelectOddsInSelection):
                    button.clickable.clicked += () =>
                        {
                            Selection.objects = SelectOddsInSelection(Selection.gameObjects);
                        };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.GameObjectSelected,
                        button);
                    break;
                
                case nameof(ButtonNames.SelectEvensInSelection):
                    button.clickable.clicked += () =>
                    {
                        Selection.objects = SelectEvensInSelection(Selection.gameObjects);
                    };
                    ModuleUtils.AddToVisualElementToggleData(toggleData, EnableCondition.GameObjectSelected,
                        button);
                    break;
            }

            return button;
        }

        public static GameObject[] SelectOddsInSelection(GameObject[] selection)
        {
            List<GameObject> newSelection = new List<GameObject>();

            for (int i = 0; i < selection.Length; i++) {
                int sibindex = selection[i].transform.GetSiblingIndex();

                if (sibindex % 2 == 0) {
                    newSelection.Add(selection[i]);
                }
            }

            return newSelection.ToArray();
        }
        
        public static GameObject[] SelectEvensInSelection(GameObject[] selection)
        {
            List<GameObject> newSelection = new List<GameObject>();

            for (int i = 0; i < selection.Length; i++) {
                int sibindex = selection[i].transform.GetSiblingIndex();

                if (sibindex % 2 == 1) {
                    newSelection.Add(selection[i]);
                }
            }

            return newSelection.ToArray();
        }
        
        public static void ShowSelectionTools()
        {
            if (GUILayout.Button("Select Content Object")) {
                SelectContentObject();
            }

            if (GUILayout.Button("Set Focus on Inspector")) {
                FocusOnHierarchy();
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("< Select Previous")) {
                SelectPrevious(Selection.activeTransform);
            }

            if (GUILayout.Button("Select Next >")) {
                SelectNext(Selection.activeTransform);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("< Step Backward")) {
                StepBackward(Selection.activeTransform);
            }

            if (GUILayout.Button("Step Forward >")) {
                StepForward(Selection.activeTransform);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("∧ Traverse Up")) {
                TraverseUp(Selection.activeTransform);
            }

            if (GUILayout.Button("Traverse Down ∨")) {
                TraverseDown(Selection.activeTransform);
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Deselect all")) {
                Selection.activeGameObject = null;
            }

            if (GUILayout.Button("Delete Selection")) {
                DeleteTarget(Selection.gameObjects);
            }
        }

        static void SelectContentObject()
        {
            GameObject[] contentObjects = GameObject.FindGameObjectsWithTag("content");

            if (contentObjects.Length == 0) {
                EditorUtility.DisplayDialog("Content object not found", "Please create an object with the tag, 'content.'", "Ok");
            } else if (contentObjects.Length > 1) {
                for (int i = 0; i < contentObjects.Length; i++) {
                    if (Selection.activeGameObject == contentObjects[i]) {
                        continue;
                    }
                    Selection.activeGameObject = contentObjects[i];
                    break;
                }
            } else {
                Selection.activeGameObject = contentObjects[0];
            }
            FocusOnHierarchy();
        }

        static void SelectPrevious(Transform sourceTransform)
        {
            if (sourceTransform == null) {
                DisplayMissingTransformMessage();
                return;
            }

            int newIndex = sourceTransform.GetSiblingIndex() - 1;

            if (newIndex < 0) {
                Selection.activeTransform = sourceTransform.parent;
            } else {
                Transform newSelection = sourceTransform.parent.GetChild(newIndex);
                Selection.activeTransform = newSelection;
            }
            FocusOnHierarchy();
        }

        static void SelectNext(Transform sourceTransform)
        {
            if (sourceTransform == null) {
                DisplayMissingTransformMessage();
                return;
            }

            if (Selection.activeGameObject.tag == "content") {
                StepForward(sourceTransform);
                return;
            }

            int newIndex = sourceTransform.GetSiblingIndex() + 1;

            if (newIndex >= sourceTransform.parent.childCount) {
                SelectNext(sourceTransform.parent);
            } else {
                Transform newSelection = sourceTransform.parent.GetChild(newIndex);
                Selection.activeTransform = newSelection;
            }
            FocusOnHierarchy();
        }

        static void StepBackward(Transform sourceTransform)
        {
            if (sourceTransform == null) {
                DisplayMissingTransformMessage();
                return;
            }

            int previousSibIndex = sourceTransform.GetSiblingIndex() - 1;

            if (previousSibIndex < 0) {
                Selection.activeTransform = sourceTransform.parent;
                return;
            }

            Transform previousTransform = sourceTransform.parent.GetChild(previousSibIndex);
            int prevChildCount = previousTransform.childCount;

            if (prevChildCount == 0) {
                Selection.activeTransform = previousTransform;
            } else {
                Transform newSelection = GetRootChild(previousTransform.GetChild(prevChildCount - 1));

                Transform GetRootChild(Transform source)
                {
                    int childCount = source.childCount;

                    if (childCount == 0) {
                        return source;
                    } else {
                        return GetRootChild(source.GetChild(childCount - 1));
                    }
                }

                Selection.activeTransform = newSelection;
            }
            FocusOnHierarchy();
        }

        static void StepForward(Transform sourceTransform)
        {
            if (sourceTransform == null) {
                DisplayMissingTransformMessage();
                return;
            }

            int childCount = sourceTransform.childCount;

            if (childCount > 0) {
                Selection.activeTransform = sourceTransform.GetChild(0);
            } else {
                SelectNext(sourceTransform);
            }
            FocusOnHierarchy();
        }

        static void TraverseUp(Transform sourceTransform)
        {
            if (sourceTransform == null) {
                DisplayMissingTransformMessage();
                return;
            }

            Selection.activeTransform = sourceTransform.parent;
            FocusOnHierarchy();
        }

        static void TraverseDown(Transform sourceTransform)
        {
            if (sourceTransform == null) {
                DisplayMissingTransformMessage();
                return;
            }

            int newSelectionIndex = sourceTransform.parent.GetSiblingIndex() + 1;

            if (sourceTransform.parent.parent == null) {
                Selection.activeTransform = sourceTransform.parent;
            } else if (newSelectionIndex < sourceTransform.parent.parent.childCount) {
                Selection.activeTransform = sourceTransform.parent.parent.GetChild(newSelectionIndex);
            } else {
                Transform newSelection = GetNextRoot(sourceTransform.parent.parent);

                Transform GetNextRoot(Transform source)
                {
                    int targetSelectionIndex = source.GetSiblingIndex() + 1;

                    if (source.parent == null) {
                        return source;
                    } else if (targetSelectionIndex < source.parent.childCount) {
                        return source.parent.GetChild(targetSelectionIndex);
                    } else {
                        return GetNextRoot(source.parent);
                    }
                }

                Selection.activeTransform = newSelection;
            }
            FocusOnHierarchy();
        }

        static void DeleteTarget(GameObject targetObject)
        {
            Undo.RecordObject(targetObject, "Delete object");
            Object.Destroy(targetObject);
        }

        static void DeleteTarget(GameObject[] targetObjects)
        {
            for (int i = 0; i < targetObjects.Length; i++) {
                Undo.DestroyObjectImmediate(targetObjects[i]);
            }
        }

        static void FocusOnHierarchy()
        {
            EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
        }

        static void DisplayMissingTransformMessage()
        {
            EditorUtility.DisplayDialog("No object selected", "Please select an object", "Ok");
        }
    }
}