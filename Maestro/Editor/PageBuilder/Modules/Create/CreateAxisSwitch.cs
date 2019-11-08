using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace AltSalt.Maestro
{
	public class CreateAxisSwitch : EditorWindow {

//        public AxisSwitch axisSwitchComponent;
//
//
//		int inputSwitchID = 1;
//		int axisSwitchID = 1;
//
//        string rawPath = "";
//        string directoryPath = "";
//
//        string errorMessage = "";
//
//        string[] assetsToCreate =
//        {"axis_inflection_point"};
//
//        GUIStyle redFont = new GUIStyle();
//        
//        [MenuItem("Tools/AltSalt/Create Axis Switch")]
//        static void Init()
//        {
//            EditorWindow window = GetWindow(typeof(CreateAxisSwitch));
//            window.Show();
//        }
//
//		private void OnEnable()
//		{
//            titleContent = new GUIContent("C. AxisSwitch");
//		}
//
//		void OnGUI()
//        {
//            redFont.normal.textColor = Color.magenta;
//            redFont.margin.left = 6;
//
//            GUILayout.Space(20);
//
//            if(errorMessage.Length > 0) {            
//				GUILayout.Label(errorMessage, redFont);
//            }
//			
//			GUILayout.Label("Please populate an ID number for your new Axis Switch.");
//			
//			GUILayout.Space(20);
//			
//			GUILayout.BeginHorizontal();
//			axisSwitchID = EditorGUILayout.IntField("Axis Switch ID: ", inputSwitchID);
//			
//			if (GUILayout.Button("+", GUILayout.Width(50))) {
//				IncrementID();
//			}
//			
//			if (GUILayout.Button("-", GUILayout.Width(50))) {
//				DecrementID();
//			}
//			GUILayout.EndHorizontal();
//			
//            GUILayout.Space(20);
//
//            if (directoryPath.Length > 0) {
//                GUILayout.Label("Will create axisSwitch" + axisSwitchID + " in '" + directoryPath + "'");
//            }
//            else {
//                GUILayout.Label("No path selected.");
//            }
//
//            if(GUILayout.Button("Select folder")) {
//                OpenFolderDialogue();   
//            }
//            
//            GUILayout.Space(20);
//            
//
//            if (GUILayout.Button("Create")) {
//				CreateSwitch();
//            }
//			
//        }
//		
//		void IncrementID()
//		{
//            errorMessage = "";
//			inputSwitchID++;
//			this.Repaint();
//		}
//		
//		void DecrementID()
//		{
//            errorMessage = "";
//            inputSwitchID--;
//			this.Repaint();
//		}
//		
//		void OpenFolderDialogue()
//		{
//            errorMessage = "";
//            rawPath = EditorUtility.OpenFolderPanel("Select target folder", "", "");
//            int substringPos = rawPath.IndexOf("Assets");
//            if(substringPos == -1) {
//                throw new Exception("Assets folder not found! Please select a path inside of the Assets folder.");
//            }
//            directoryPath = rawPath.Substring(substringPos);
//            this.Repaint();
//		}
//		
//		void CreateSwitch()
//		{
//            if(directoryPath.Length < 1) {
//                errorMessage = "Error: No path chosen. Please choose a directory before proceeding.";
//                this.Repaint();
//                throw new Exception(errorMessage);
//            }
//
//            if(Directory.Exists(rawPath + "/axisSwitch" + axisSwitchID)) {
//                errorMessage = "Error: axisSwitch " + axisSwitchID + " already exists at the chosen path! Please select a new ID.";
//                this.Repaint();
//                throw new Exception(errorMessage);
//            }
//
//            errorMessage = "";
//            this.Repaint();
//
//            string[] guids = AssetDatabase.FindAssets("AxisSwitch t:prefab", new string[]{"Assets/AltSalt"});
//            string axisSwitchPath = AssetDatabase.GUIDToAssetPath(guids[0]);
//
//            Debug.Log (axisSwitchPath);
//
//            GameObject newAxisSwitch = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath(axisSwitchPath, typeof(GameObject))) as GameObject;
//            newAxisSwitch.name = "AxisSwitch" + axisSwitchID;
//            axisSwitchComponent = newAxisSwitch.GetComponent<AxisSwitch>();
//
//            string basePath = directoryPath + "/axisSwitch" + axisSwitchID + "/";
//
//            AssetDatabase.CreateFolder(directoryPath, "axisSwitch" + axisSwitchID);
//            for (int i = 0; i < assetsToCreate.Length; i++) {
//                FloatVariable floatVar = ScriptableObject.CreateInstance("FloatVariable") as FloatVariable;
//                AssetDatabase.CreateAsset(floatVar, basePath + assetsToCreate[i] + axisSwitchID + ".asset");
//            }
//
//            // TO DO - make this look nicer. Right now there needs to be a correct relationships established between
//            // the axisSwitch game object and the assets created in the loop above, but I don't want to invest the time
//            // into making that dynamic right now, so I'll leave it hardcoded.
//            //axisSwitchComponent.swipeOriginAxisMin.Variable = AssetDatabase.LoadAssetAtPath(basePath + "axis_swipe_originMin" + axisSwitchID + ".asset", typeof(FloatVariable)) as FloatVariable;
//            //axisSwitchComponent.swipeOriginAxisMax.Variable = AssetDatabase.LoadAssetAtPath(basePath + "axis_swipe_originMax" + axisSwitchID + ".asset", typeof(FloatVariable)) as FloatVariable;
//            //axisSwitchComponent.swipeTransitionMin.Variable = AssetDatabase.LoadAssetAtPath(basePath + "axis_swipe_transMin" + axisSwitchID + ".asset", typeof(FloatVariable)) as FloatVariable;
//            //axisSwitchComponent.swipeTransitionMax.Variable = AssetDatabase.LoadAssetAtPath(basePath + "axis_swipe_transMax" + axisSwitchID + ".asset", typeof(FloatVariable)) as FloatVariable;
//            //axisSwitchComponent.swipeDestAxisMin.Variable = AssetDatabase.LoadAssetAtPath(basePath + "axis_swipe_destMin" + axisSwitchID + ".asset", typeof(FloatVariable)) as FloatVariable;
//            //axisSwitchComponent.swipeDestAxisMax.Variable = AssetDatabase.LoadAssetAtPath(basePath + "axis_swipe_destMax" + axisSwitchID + ".asset", typeof(FloatVariable)) as FloatVariable;
//            //axisSwitchComponent.momentumOriginAxisMin.Variable = AssetDatabase.LoadAssetAtPath(basePath + "axis_moment_originMin" + axisSwitchID + ".asset", typeof(FloatVariable)) as FloatVariable;
//            //axisSwitchComponent.momentumOriginAxisMax.Variable = AssetDatabase.LoadAssetAtPath(basePath + "axis_moment_originMax" + axisSwitchID + ".asset", typeof(FloatVariable)) as FloatVariable;
//            //axisSwitchComponent.momentumDestAxisMin.Variable = AssetDatabase.LoadAssetAtPath(basePath + "axis_moment_destMin" + axisSwitchID + ".asset", typeof(FloatVariable)) as FloatVariable;
//            //axisSwitchComponent.momentumDestAxisMax.Variable = AssetDatabase.LoadAssetAtPath(basePath + "axis_moment_destMax" + axisSwitchID + ".asset", typeof(FloatVariable)) as FloatVariable;
//		}
	}
}