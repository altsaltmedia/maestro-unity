using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

namespace AltSalt
{

    #if UNITY_EDITOR
    [ExecuteInEditMode]
    #endif
    public abstract class ResponsiveElement : MonoBehaviour {

        public AppSettings appSettings;

        [Required]
        public SimpleEvent screenResized;

        [ValidateInput("IsPopulated")]
        public FloatReference screenWidth = new FloatReference();

        [ValidateInput("IsPopulated")]
        public FloatReference screenHeight = new FloatReference();

        [ValidateInput("IsPopulated")]
        public FloatReference aspectRatio = new FloatReference();

        public bool hasBreakpoints;

        [ShowIf("hasBreakpoints")]
        [PropertySpace]
        [InfoBox("You should always have x+1 breakpoint values; e.g., for 1 breakpoint at 1.34, you must specify 2 breakpoint values - one for aspect ratios wider than 1.34, and another for aspect ratios narrower than or equal to 1.34.")]
        [InfoBox("Breakpoint examples: To target devices narrow than iPad (aspect ratio 1.33), specify a breakpoint of 1.4; to target narrower than iPhone (1.77), specify a breakpoint of 1.78.")]
        public List<float> aspectRatioBreakpoints = new List<float>();

        protected virtual void Start()
        {
            ExecuteResponsiveAction();
        }

        protected abstract void ExecuteResponsiveAction();


#if UNITY_EDITOR

        protected SimpleEventListener simpleEventListener;
        protected bool editorListenerCreated = false;

        public virtual void Reset()
        {
            string[] guids;
            string path;

            if (appSettings == null) {
                appSettings = Utils.GetAppSettings();
            }

            if (screenWidth.Variable == null) {
                screenHeight.Variable = aspectRatio.Variable = Utils.GetFloatVariable("ScreenWidth");
            }

            if(screenHeight.Variable == null) {
                screenHeight.Variable = aspectRatio.Variable = Utils.GetFloatVariable("ScreenHeight");
            }

            if(aspectRatio.Variable == null) {
                aspectRatio.Variable = Utils.GetFloatVariable("AspectRatio");
            }

            if(screenResized == null) {
                screenResized = Utils.GetSimpleEvent("ScreenResized");
            }
        }

        void OnEnable()
        {
            editorListenerCreated = false;
        }

        protected virtual void OnRenderObject()
        {
            //Uncomment these lines to fix repopulate AppSettings on responsive objects
            //if the values are lost for some reason
            // 
            //if (appSettings == null) {
            //    appSettings = Utils.GetAppSettings();
            //}

            if (editorListenerCreated == false && appSettings.editorDebugEventsActive.Value == true) {
                simpleEventListener = new SimpleEventListener(screenResized, this.gameObject);
                simpleEventListener.OnTargetEventExecuted += ExecuteResponsiveAction;
                editorListenerCreated = true;
            }

            if(editorListenerCreated == true && appSettings.editorDebugEventsActive == false) {
                DisableListener();
            }
        }

        void OnDisable()
        {
            if(editorListenerCreated == true && appSettings.editorDebugEventsActive == true) {
                DisableListener();
            }
        }

        void DisableListener()
        {
            if(simpleEventListener != null) {
                simpleEventListener.DestroyListener();
                simpleEventListener = null;
                editorListenerCreated = false;
            }
        }

#endif

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
	}
	
}