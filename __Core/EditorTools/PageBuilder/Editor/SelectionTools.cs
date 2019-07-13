using UnityEngine;
using UnityEditor;

namespace AltSalt
{
    public static class SelectionTools
    {
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