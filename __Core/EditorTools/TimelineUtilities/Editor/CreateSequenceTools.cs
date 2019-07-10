using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.IO;

namespace AltSalt
{
    public static class CreateSequenceTools
    {
        public static PageBuilderReferences pageBuilderReferences;

        static GameObject sequenceControllerObject;
        static SequenceList sequenceList;
        static string sequenceName = "";

        public static void ShowSequenceTools()
        {
            pageBuilderReferences = Utils.GetScriptableObject("PageBuilderReferences") as PageBuilderReferences;

            GUILayout.Space(10);

            //if (GUILayout.Button("Create standard director")) {
            //    CreateStandardDirector();
            //}

            EditorGUILayout.LabelField("Create a sequence controller:");

            if (GUILayout.Button("Create sequence touch applier")) {
                CreateSequenceTouchApplier(Selection.activeTransform);
            }

            if (GUILayout.Button("Create sequence autoplayer")) {
                CreateSequenceAutoplayer(Selection.activeTransform);
            }

            GUILayout.Space(10);


            EditorGUILayout.BeginHorizontal();

                if (Event.current.commandName == "ObjectSelectorUpdated") {
                    sequenceControllerObject = EditorGUIUtility.GetObjectPickerObject() as GameObject;
                }

                if (sequenceControllerObject == null) {
                    EditorGUILayout.LabelField("No sequence controller selected");
                } else {
                    EditorGUILayout.LabelField(sequenceControllerObject.name + " is selected");
                }

                if (GUILayout.Button("Select sequence controller")) {
                    SelectSequenceController();
                }

            EditorGUILayout.EndHorizontal();

            
            EditorGUI.BeginDisabledGroup(CheckListDependencies());
                if (GUILayout.Button("Create sequence list")) {
                    CreateSequenceList();
                }
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(10);

            sequenceList = EditorGUILayout.ObjectField("Target sequence list", sequenceList, typeof(SequenceList), false) as SequenceList;
            sequenceName = EditorGUILayout.TextField("New sequence name", sequenceName);

            EditorGUI.BeginDisabledGroup(CheckSwipeDependencies());
                if (GUILayout.Button("Create swipe director")) {
                    CreateSwipeDirector(Selection.activeTransform, sequenceList, sequenceName);
                }
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(10);

            
        }

        //void CreateStandardDirector()
        //{

        //}

        static bool CheckListDependencies()
        {
            if (sequenceControllerObject == null) {
                return true;
            }

            return false;
        }

        static bool CheckSwipeDependencies()
        {
            if(sequenceList == null || sequenceName == string.Empty) {
                return true;
            }

            return false;
        }

        static void SelectSequenceController()
        {
            EditorGUIUtility.ShowObjectPicker<SequenceController>(sequenceControllerObject, true, "t:" + typeof(SequenceController).Name, 1);
        }

        /* DIRECTOR TOOLS */

        static GameObject CreateSequenceTouchApplier(Transform parentTransform)
        {
            GameObject sequenceTouchApplier = PrefabUtility.InstantiatePrefab(pageBuilderReferences.sequenceTouchApplier) as GameObject;
            Undo.RegisterCreatedObjectUndo(sequenceTouchApplier, "Create sequence touch applier");

            if (parentTransform != null) {
                sequenceTouchApplier.transform.SetParent(parentTransform);
            }
            return sequenceTouchApplier;
        }

        static GameObject CreateSequenceAutoplayer(Transform parentTransform)
        {
            GameObject sequenceAutoplayer = PrefabUtility.InstantiatePrefab(pageBuilderReferences.sequenceAutoplayer) as GameObject;
            Undo.RegisterCreatedObjectUndo(sequenceAutoplayer, "Create sequence autoplayer");

            if (parentTransform != null) {
                sequenceAutoplayer.transform.SetParent(parentTransform);
            }
            return sequenceAutoplayer;
        }


        static GameObject CreateSwipeDirector(Transform parentTransform, SequenceList parentSequenceList, string elementName)
        {
            GameObject swipeDirector = PrefabUtility.InstantiatePrefab(pageBuilderReferences.swipeDirector) as GameObject;
            Undo.RegisterCreatedObjectUndo(swipeDirector, "Create swipe director");

            if (parentTransform != null) {
                swipeDirector.transform.SetParent(parentTransform);
            }

            if (elementName.Length > 0) {
                swipeDirector.name = elementName;
            }

            string parentDirectoryPath = Utils.GetAssetPathFromObject(parentSequenceList);
            string filePath = Path.GetDirectoryName(parentDirectoryPath) + "/" + elementName;

            swipeDirector = AddSequenceToSwipeDirector(swipeDirector, parentSequenceList, filePath);
            swipeDirector = AddTimelineAssetToSwipeDirector(swipeDirector, filePath);

            Selection.activeTransform = swipeDirector.transform;
            return swipeDirector;
        }

        static GameObject AddSequenceToSwipeDirector(GameObject swipeDirector, SequenceList targetSequenceList, string filePath)
        {
            Sequence newSequence = CreateSequence(filePath);
            if(newSequence != null) {
                swipeDirector.GetComponent<DirectorUpdater>().sequence = newSequence;
                targetSequenceList.sequences.Add(newSequence);
            }
            return swipeDirector;
        }

        static Sequence CreateSequence(string filePath)
        {
            Sequence newSequence = ScriptableObject.CreateInstance(typeof(Sequence)) as Sequence;

            if (EditorUtility.DisplayDialog("Set sequence to active?", "Would you like to set sequence " + Path.GetFileName(filePath) + " to active?", "Yes", "No")) {
                newSequence.Active = true;
            } else {
                newSequence.Active = false;
            }

            string finalPath = filePath + ".asset";
            
            if (File.Exists(Path.GetFullPath(finalPath))) {
                if (EditorUtility.DisplayDialog("Overwrite existing file?", "This will overwrite the existing file at " + finalPath, "Proceed", "Cancel")) {
                    AssetDatabase.CreateAsset(newSequence, finalPath);
                } else {
                    EditorUtility.DisplayDialog("No sequence populated", "Sequence creation cancelled.", "Ok");
                    return null;
                }
            } else {
                AssetDatabase.CreateAsset(newSequence, finalPath);
            }

            return newSequence;
        }

        static GameObject AddTimelineAssetToSwipeDirector(GameObject swipeDirector, string filePath)
        {
            TimelineAsset newTimelineAsset = CreateTimelineAsset(filePath);
            if (newTimelineAsset != null) {
                swipeDirector.GetComponent<PlayableDirector>().playableAsset = newTimelineAsset;
            }
            return swipeDirector;
        }

        static TimelineAsset CreateTimelineAsset(string filePath)
        {
            TimelineAsset newTimelineAsset = ScriptableObject.CreateInstance(typeof(TimelineAsset)) as TimelineAsset;
            newTimelineAsset.editorSettings.fps = 100f;
            string finalPath = filePath + ".playable";

            if (File.Exists(Path.GetFullPath(finalPath))) {
                if (EditorUtility.DisplayDialog("Overwrite existing file?", "This will overwrite the existing file at " + finalPath, "Proceed", "Cancel")) {
                    AssetDatabase.CreateAsset(newTimelineAsset, finalPath);
                } else {
                    EditorUtility.DisplayDialog("No sequence populated", "Timeline asset creation cancelled.", "Ok");
                    return null;
                }
            } else {
                AssetDatabase.CreateAsset(newTimelineAsset, finalPath);
            }

            return newTimelineAsset;
        }

        static SequenceList CreateSequenceList()
        {
            string filePath = EditorUtility.SaveFilePanelInProject("Create new sequence list", "", "asset", "Please enter a file name for the new sequence list");
            SequenceList newSequenceList = ScriptableObject.CreateInstance(typeof(SequenceList)) as SequenceList;
            AssetDatabase.CreateAsset(newSequenceList, filePath);

            sequenceControllerObject.GetComponent<SequenceController>().sequenceLists.Add(newSequenceList);

            return newSequenceList;
        }


        /* TRACK TOOLS */


    }
}
