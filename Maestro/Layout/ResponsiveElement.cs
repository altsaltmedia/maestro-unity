using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using System.IO;
using UnityEngine.SceneManagement;
using SimpleJSON;

namespace AltSalt.Maestro.Layout
{
    [ExecuteInEditMode]
    public abstract class ResponsiveElement : SerializableElement, IResponsive
    {
        [Required]
        [SerializeField]
        protected AppSettings appSettings;

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        protected FloatReference sceneWidth = new FloatReference();

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        protected FloatReference sceneHeight = new FloatReference();

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        protected FloatReference sceneAspectRatio = new FloatReference();

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        ComplexEventTrigger responsiveElementEnable = new ComplexEventTrigger();

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        ComplexEventTrigger responsiveElementDisable = new ComplexEventTrigger();

        public string Name {
            get {
                return this.name;
            }
        }

        [SerializeField]
#if UNITY_EDITOR
        [OnValueChanged(nameof(PopulateDefaultBreakpointValues))]
#endif
        protected bool hasBreakpoints;
        public bool HasBreakpoints {
            get {
                return hasBreakpoints;
            }
            set {
                hasBreakpoints = value;
            }
        }

        [SerializeField]
#if UNITY_EDITOR
        [OnValueChanged(nameof(ResetResponsiveElementData))]
#endif
        protected int priority;
        public int Priority {
            get {
                return priority;
            }
        }

        [SerializeField]
        protected bool logElementOnLayoutUpdate;
        public bool LogElementOnLayoutUpdate {
            get {
                if (appSettings.logResponsiveElementActions.Value == true) {
                    return true;
                } else {
                    return logElementOnLayoutUpdate;
                }
            }
        }

        public Scene ParentScene {
            get {
                return gameObject.scene;
            }
        }

        protected int breakpointIndex;

        [ShowIf(nameof(hasBreakpoints))]
        [PropertySpace]
        [InfoBox("You should always have x+1 breakpoint values; e.g., for 1 breakpoint at 1.34, you must specify 2 breakpoint values - one for aspect ratios wider than 1.34, and another for aspect ratios narrower than or equal to 1.34.")]
        [InfoBox("Breakpoint examples: To target devices narrow than iPad (aspect ratio 1.33), specify a breakpoint of 1.4; to target narrower than iPhone (1.77), specify a breakpoint of 1.78.")]
        [ValidateInput(nameof(IsPopulated))]
#if UNITY_EDITOR
        [OnValueChanged(nameof(UpdateBreakpointDependencies))]
#endif
        public List<float> aspectRatioBreakpoints = new List<float>();
        public List<float> AspectRatioBreakpoints {
            get {
                return aspectRatioBreakpoints;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
#if UNITY_EDITOR
            PopulateDependencies();
            PopulateNonSerializedProperties();
#endif
            responsiveElementEnable.RaiseEvent(this.gameObject, this);
        }

#if UNITY_EDITOR
        //void OnDisable()
        //{
        //    responsiveElementDisable.RaiseEvent(this.gameObject, this);
        //}
#endif

        public void CallExecuteLayoutUpdate(UnityEngine.Object callingObject)
        {
            if (LogElementOnLayoutUpdate == true) {
                Debug.Log("CallExecuteLayoutUpdate triggered!");
                Debug.Log("Calling object : " + callingObject.name, callingObject);
                Debug.Log("Triggered object : " + Name, gameObject);
                Debug.Log("Component : " + this.GetType().Name, gameObject);
                Debug.Log("--------------------------");
            }

            if (appSettings.modifyLayoutActive.Value == true) {
                LoadData();
            }
            ExecuteResponsiveAction();
        }

        protected void GetBreakpointIndex()
        {
            if (hasBreakpoints == true) {
                if (aspectRatioBreakpoints.Count < 1) {
#if UNITY_EDITOR
                    LogBreakpointError();
#endif
                    return;
                } else {
                    breakpointIndex = Utils.GetValueIndexInList(sceneAspectRatio.Value, aspectRatioBreakpoints);
                }
            } else {
                breakpointIndex = 0;
            }
        }

        [InfoBox("Trigger responsive action.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        [PropertyOrder(8)]
        public virtual void ExecuteResponsiveAction()
        {
            GetBreakpointIndex();
        }

#if UNITY_EDITOR

        public List<float> AddBreakpoint(float targetBreakpoint)
        {
            Undo.RecordObject(this, "add breakpoint");
            return ResponsiveUtilsCore.AddBreakpointToResponsiveElement(this, targetBreakpoint);
        }

        void ResetResponsiveElementData()
        {
            responsiveElementDisable.RaiseEvent(this.gameObject, this);
            responsiveElementEnable.RaiseEvent(this.gameObject, this);
        }

        protected void PopulateDefaultBreakpointValues()
        {
            PopulateDefaultBreakpoint();
            UpdateBreakpointDependencies();
        }

        void PopulateDefaultBreakpoint()
        {
            if(hasBreakpoints == true && aspectRatioBreakpoints.Count == 0) {
                decimal tempVal = Convert.ToDecimal(sceneAspectRatio.Value);
                tempVal = Math.Round(tempVal, 2);
                aspectRatioBreakpoints.Add((float)tempVal + .01f);
            }
        }

        protected virtual void UpdateBreakpointDependencies() {
            if(aspectRatioBreakpoints.Count == 0) {
                hasBreakpoints = false;
            }
        }

        public virtual void Reset()
        {
            PopulateDependencies();
            PopulateNonSerializedProperties();
        }

        protected virtual void PopulateDependencies()
        {
            if (appSettings == null) {
                appSettings = Utils.GetAppSettings();
            }

            if (sceneWidth.Variable == null) {
                sceneWidth.Variable = Utils.GetFloatVariable(nameof(VarDependencies.SceneWidth));
            }

            if (sceneHeight.Variable == null) {
                sceneHeight.Variable = Utils.GetFloatVariable(nameof(VarDependencies.SceneHeight));
            }

            if (sceneAspectRatio.Variable == null) {
                sceneAspectRatio.Variable = Utils.GetFloatVariable(nameof(VarDependencies.SceneAspectRatio));
            }

            if(responsiveElementEnable.ComplexEventTarget == null) {
                responsiveElementEnable.ComplexEventTarget = Utils.GetComplexEvent(nameof(VarDependencies.ResponsiveElementEnable));
            }

            if (responsiveElementDisable.ComplexEventTarget == null) {
                responsiveElementDisable.ComplexEventTarget = Utils.GetComplexEvent(nameof(VarDependencies.ResponsiveElementDisable));
            }
        }

        void PopulateNonSerializedProperties()
        {
            nonserializedProperties.Clear();
            nonserializedProperties.Add(nameof(id));
            nonserializedProperties.Add(nameof(nonserializedProperties));
            nonserializedProperties.Add(nameof(modifySettings));
            nonserializedProperties.Add(nameof(appSettings));
            nonserializedProperties.Add(nameof(sceneWidth));
            nonserializedProperties.Add(nameof(sceneHeight));
            nonserializedProperties.Add(nameof(sceneAspectRatio));
            nonserializedProperties.Add(nameof(responsiveElementEnable));
            nonserializedProperties.Add(nameof(responsiveElementDisable));
        }

        protected virtual void OnRenderObject()
        {
            //Uncomment this if it's ever necessary to repopulate missing dependencies
            //PopulateDependencies();
            //PopulateNonSerializedProperties();
            
        }

        [HorizontalGroup("Data Handler", 0.5f)]
        [InfoBox("Saves data based on current scene and active layout.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        [PropertyOrder(7)]
        public void SaveData()
        {
            Initialize();
            string data = JsonUtility.ToJson(this, true);
            var tempObject = JSON.Parse(data);
            for(int i=0; i<nonserializedProperties.Count; i++) {
                tempObject.Remove(nonserializedProperties[i]);
            }
            data = tempObject.ToString(2);
            string directoryPath = Utils.GetDirectory(new string[] { "/Resources", "/Layouts", "/" + SceneManager.GetActiveScene().name, "/" + modifySettings._activeLayoutConfig.name });
            string fileName = this.name + id.ToString();
            string filePath = Utils.GetFilePath(directoryPath, fileName, ".json");

            if(File.Exists(filePath)) {
                if (EditorUtility.DisplayDialog("Overwrite existing file?",
                "This will overwrite the existing data at " + Utils.GetFilePath(SceneManager.GetActiveScene().name, fileName) + ".", "Proceed", "Cancel")) {
                    File.WriteAllText(filePath, data);
                    AssetDatabase.Refresh();
                }
            } else {
                File.WriteAllText(filePath, data);
                AssetDatabase.Refresh();
            }
        }

        public string LogAddBreakpointMessage(float targetBreakpoint)
        {
            string message = "Added breakpoint of " + targetBreakpoint.ToString("F2") + " to " + this.name + " " + this.GetType().Name;
            Debug.Log(message, this);
            return message;
        }
#endif
        protected virtual void LogBreakpointError()
        {
            Debug.LogError("Please specify either 1.) target values for saving OR 2.) breakpoints and corresponding values on " + this.name, this);
        }

        [HorizontalGroup("Data Handler", 0.5f)]
        [InfoBox("Load and populate data based on current scene and active layout.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        [PropertyOrder(7)]
        public void LoadData()
        {
            var jsonTextFile = Resources.Load<TextAsset>("Layouts/" + sceneName + "/" + modifySettings._activeLayoutConfig.name + "/" + this.name + id.ToString());
            if (jsonTextFile != null) {
                Debug.Log("Populating " + this.name + " with stored data for " + modifySettings._activeLayoutConfig.name + " layout", this);
                JsonUtility.FromJsonOverwrite(jsonTextFile.ToString(), this);
            }
        }

        protected static bool IsPopulated(ComplexEventTrigger attribute)
        {
            return Utils.IsPopulated(attribute);
        }

        protected static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

        private static bool IsPopulated(List<float> attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }

}