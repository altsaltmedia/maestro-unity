/***********************************************

Copyright © 2018 AltSalt Media, LLC.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;
using UnityEngine.Timeline;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Timeline;
#endif

namespace AltSalt
{
    public class ResponsiveLerpToTargetBehaviour : PlayableBehaviour, IResponsive {

        [ValidateInput(nameof(IsPopulated))]
        public FloatReference sceneAspectRatio = new FloatReference();

        public SimpleEventListener resizedListener;

        [PropertySpace]
        [InfoBox("You should always have x+1 breakpoint values; e.g., for 1 breakpoint at 1.34, you must specify 2 breakpoint values - one for aspect ratios wider than 1.34, and another for aspect ratios narrower than or equal to 1.34.")]
        [InfoBox("Breakpoint examples: To target devices narrow than iPad (aspect ratio 1.33), specify a breakpoint of 1.4; to target narrower than iPhone (1.77), specify a breakpoint of 1.78.")]
        public List<float> aspectRatioBreakpoints = new List<float>();
        public List<float> AspectRatioBreakpoints {
            get {
                return aspectRatioBreakpoints;
            }
        }

        public string Name {
            get {
                return this.ToString();
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
        int priority;
        public int Priority {
            get {
                return priority;
            }
        }

        public EasingFunction.Ease ease = EasingFunction.Ease.EaseInOutQuad;

        [HideInInspector]
        public double startTime;

        [HideInInspector]
        public double endTime;

        [HideInInspector]
        public TrackAsset parentTrack;

        [HideInInspector]
        public PlayableAsset clipAsset;

        [HideInInspector]
        public EasingFunction.Function easingFunction;

        public override void OnGraphStart(Playable playable)
        {
#if UNITY_EDITOR
            if (sceneAspectRatio.Variable == null) {
                sceneAspectRatio.Variable = Utils.GetFloatVariable(nameof(VarDependencies.SceneAspectRatio));
            }
            resizedListener = new SimpleEventListener(Utils.GetSimpleEvent(nameof(VarDependencies.ScreenResized)), clipAsset, TimelineEditor.inspectedDirector.gameObject.scene.name);
            resizedListener.OnTargetEventExecuted += ExecuteResponsiveAction;
            PopulateBreakpointDependencies();
#endif
            base.OnGraphStart(playable);
            easingFunction = EasingFunction.GetEasingFunction(ease);
            ExecuteResponsiveAction();
        }

        public override void OnGraphStop(Playable playable)
        {
            base.OnGraphStop(playable);
            resizedListener.OnTargetEventExecuted -= ExecuteResponsiveAction;
        }

        public virtual void ExecuteResponsiveAction() {
            // Override this in child classes to define custom responsive actions
        }

#if UNITY_EDITOR
        protected virtual void PopulateBreakpointDependencies()
        {
            // Override this in child classes to define custom responsive actions
        }

        protected void PopulateDefaultBreakpointValues()
        {
            PopulateDefaultBreakpoint();
            UpdateBreakpointDependencies();
        }

        void PopulateDefaultBreakpoint()
        {
            if (hasBreakpoints == true && aspectRatioBreakpoints.Count == 0) {
                decimal tempVal = Convert.ToDecimal(sceneAspectRatio.Value);
                tempVal = Math.Round(tempVal, 2);
                aspectRatioBreakpoints.Add((float)tempVal + .01f);
            }
        }

        protected virtual void UpdateBreakpointDependencies()
        {
            if (aspectRatioBreakpoints.Count == 0) {
                hasBreakpoints = false;
            }
        }

        public List<float> AddBreakpoint(float targetBreakpoint)
        {
            Undo.RecordObject(clipAsset, "Add breakpoint to clip(s)");
            return Utils.AddBreakpointToResponsiveElement(this, targetBreakpoint);
        }

        public string LogAddBreakpointMessage(float targetBreakpoint)
        {
            string message = "Added breakpoint of " + targetBreakpoint.ToString("F2") + " to " + this.ToString() + " " + this.GetType().Name;
            Debug.Log(message);
            return message;   
        }

        protected virtual void LogBreakpointWarning()
        {
            Debug.LogWarning("Please specify either 1.) target values for saving OR 2.) breakpoints and corresponding values on " + this.ToString());
        }
#endif

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}