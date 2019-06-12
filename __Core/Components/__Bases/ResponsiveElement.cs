using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using System.IO;
using UnityEngine.SceneManagement;
using SimpleJSON;

namespace AltSalt
{
    
    [ExecuteInEditMode]
    public abstract class ResponsiveElement : SerializableElement
    {
        [Required]
        [SerializeField]
        protected AppSettings appSettings;

        [Required]
        [SerializeField]
        protected SimpleEvent screenResized;

        [Required]
        [SerializeField]
        protected SimpleEvent layoutUpdate;

        [ValidateInput("IsPopulated")]
        [SerializeField]
        protected FloatReference screenWidth = new FloatReference();

        [ValidateInput("IsPopulated")]
        [SerializeField]
        protected FloatReference screenHeight = new FloatReference();

        [ValidateInput("IsPopulated")]
        [SerializeField]
        protected FloatReference aspectRatio = new FloatReference();

        [SerializeField]
        [OnValueChanged("PopulateDefaultBreakpointValues")]
        protected bool hasBreakpoints;

        protected int breakpointIndex;

        [ShowIf("hasBreakpoints")]
        [PropertySpace]
        [InfoBox("You should always have x+1 breakpoint values; e.g., for 1 breakpoint at 1.34, you must specify 2 breakpoint values - one for aspect ratios wider than 1.34, and another for aspect ratios narrower than or equal to 1.34.")]
        [InfoBox("Breakpoint examples: To target devices narrow than iPad (aspect ratio 1.33), specify a breakpoint of 1.4; to target narrower than iPhone (1.77), specify a breakpoint of 1.78.")]
        [ValidateInput("IsPopulated")]
        [OnValueChanged("UpdateBreakpointDependencies")]
        public List<float> aspectRatioBreakpoints = new List<float>();

        protected SimpleEventListener layoutUpdateListener;
        protected bool layoutListenerCreated = false;

        protected virtual void Start()
        {
            ExecuteLayoutUpdate();
        }

        void OnEnable()
        {
#if UNITY_EDITOR
            PopulateDependencies();
            PopulateNonSerializedProperties();
            resizedListenerCreated = false;
            ExecuteLayoutUpdate();
#endif
            CreateLayoutListener();
        }

        void OnDisable()
        {
            if (layoutListenerCreated == true) {
                DisableLayoutListener();
            }
#if UNITY_EDITOR
            if (resizedListenerCreated == true) {
                DisableResizedListener();
            }
#endif
        }

        void CreateLayoutListener()
        {
            if (layoutListenerCreated == false) {
                layoutUpdateListener = new SimpleEventListener(layoutUpdate, gameObject);
                layoutUpdateListener.OnTargetEventExecuted += ExecuteLayoutUpdate;
                layoutListenerCreated = true;
            }
        }

        void DisableLayoutListener()
        {
            if (layoutUpdateListener != null) {
                layoutUpdateListener.DestroyListener();
                layoutUpdateListener = null;
                layoutListenerCreated = false;
            }
        }

        void ExecuteLayoutUpdate()
        {
            if(appSettings.modifyLayoutActive.Value == true) {
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
                    breakpointIndex = Utils.GetValueIndexInList(aspectRatio.Value, aspectRatioBreakpoints);
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

        protected SimpleEventListener screenResizedListener;
        protected bool resizedListenerCreated = false;

        protected void PopulateDefaultBreakpointValues()
        {
            PopulateDefaultBreakpoint();
            UpdateBreakpointDependencies();
        }

        void PopulateDefaultBreakpoint()
        {
            if(hasBreakpoints == true && aspectRatioBreakpoints.Count == 0) {
                decimal tempVal = Convert.ToDecimal(aspectRatio.Value);
                tempVal = Math.Round(tempVal, 2);
                aspectRatioBreakpoints.Add((float)tempVal + .01f);
            }
        }

        protected virtual void UpdateBreakpointDependencies() {
            if(aspectRatioBreakpoints.Count == 0) {
                hasBreakpoints = false;
            }
        }

        public override void Reset()
        {
            base.Reset();
            PopulateDependencies();
            PopulateNonSerializedProperties();
        }

        void PopulateDependencies()
        {
            if (appSettings == null) {
                appSettings = Utils.GetAppSettings();
            }

            if (screenResized == null) {
                screenResized = Utils.GetSimpleEvent("ScreenResized");
            }

            if (layoutUpdate == null) {
                layoutUpdate = Utils.GetSimpleEvent("LayoutUpdate");
            }

            if (screenWidth.Variable == null) {
                screenWidth.Variable = Utils.GetFloatVariable("ScreenWidth");
            }

            if (screenHeight.Variable == null) {
                screenHeight.Variable = Utils.GetFloatVariable("ScreenHeight");
            }

            if (aspectRatio.Variable == null) {
                aspectRatio.Variable = Utils.GetFloatVariable("AspectRatio");
            }
        }

        void PopulateNonSerializedProperties()
        {
            nonserializedProperties.Clear();
            nonserializedProperties.Add("id");
            nonserializedProperties.Add("nonserializedProperties");
            nonserializedProperties.Add("modifySettings");
            nonserializedProperties.Add("appSettings");
            nonserializedProperties.Add("screenResized");
            nonserializedProperties.Add("layoutUpdate");
            nonserializedProperties.Add("screenWidth");
            nonserializedProperties.Add("screenHeight");
            nonserializedProperties.Add("aspectRatio");
        }

        protected override void OnRenderObject()
        {
            base.OnRenderObject();

             //Uncomment this if it's ever necessary to repopulate missing dependencies
             PopulateDependencies();
             PopulateNonSerializedProperties();

            CreateResizedListener();
            if (resizedListenerCreated == true && appSettings.debugEventsActive.Value == false) {
                DisableResizedListener();
            }

        }

        void CreateResizedListener()
        {
            if (resizedListenerCreated == false && appSettings.debugEventsActive.Value == true) {
                screenResizedListener = new SimpleEventListener(screenResized, this.gameObject);
                screenResizedListener.OnTargetEventExecuted += ExecuteResponsiveAction;
                resizedListenerCreated = true;
            }
        }

        void DisableResizedListener()
        {
            if (screenResizedListener != null) {
                screenResizedListener.DestroyListener();
                screenResizedListener = null;
                resizedListenerCreated = false;
            }
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
            string directoryPath = Utils.GetDirectory(new string[] { "/Resources", "/Layouts", "/" + SceneManager.GetActiveScene().name, "/" + modifySettings.activeLayout.name });
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

        protected virtual void LogBreakpointError()
        {
            Debug.LogError("Please specify either 1.) target values for saving OR 2.) breakpoints and corresponding values on " + this.name, this);
        }
#endif

        [HorizontalGroup("Data Handler", 0.5f)]
        [InfoBox("Load and populate data based on current scene and active layout.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        [PropertyOrder(7)]
        public void LoadData()
        {
            var jsonTextFile = Resources.Load<TextAsset>("Layouts/" + sceneName + "/" + modifySettings.activeLayout.name + "/" + this.name + id.ToString());
            if (jsonTextFile != null) {
                Debug.Log("Populating " + this.name + " with stored data for " + modifySettings.activeLayout.name + " layout", this);
                JsonUtility.FromJsonOverwrite(jsonTextFile.ToString(), this);
            }
        }

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

        private static bool IsPopulated(List<float> attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
	
}