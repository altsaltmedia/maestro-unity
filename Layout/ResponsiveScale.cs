using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Layout
{
    [ExecuteInEditMode]
    public class ResponsiveScale : ResponsiveRectTransform, IResponsiveSaveable
    {

        [InfoBox("Sets scale to manually configured values based on breakpoint.")]
        public List<Vector2> breakpointLocalScale = new List<Vector2>();


#if UNITY_EDITOR
        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateBreakpointDependencies();
        }
        
        protected override void UpdateBreakpointDependencies()
        {
            base.UpdateBreakpointDependencies();
            if (breakpointLocalScale.Count == 0) {
                breakpointLocalScale.Add(new Vector2(rectTransform.localScale.x, rectTransform.localScale.y));
            }
            
            if (breakpointLocalScale.Count <= aspectRatioBreakpoints.Count) {
                Utils.ExpandList(breakpointLocalScale, aspectRatioBreakpoints.Count);
            }
        }
        
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        [InfoBox("Saves the position at the current breakpoint index.")]
        public void SaveValue()
        {
            if (aspectRatioBreakpoints.Count < 1) {
                Debug.LogError("You must specify at least one breakpoint to save element values.", this);
                return;
            }

            int targetBreakpointIndex = Utils.GetValueIndexInList(sceneAspectRatio, aspectRatioBreakpoints);

            Utils.ExpandList(breakpointLocalScale, targetBreakpointIndex);
            breakpointLocalScale[targetBreakpointIndex] = rectTransform.localScale;
        }
#endif

        public override void ExecuteResponsiveAction()
        {
            base.ExecuteResponsiveAction();
            if(hasBreakpoints == true) {
                SetValue(breakpointIndex);
            } else {
                SetValue(0);
            }
        }

        public void SetValue(int activeIndex)
        {
#if UNITY_EDITOR
            if (activeIndex >= breakpointLocalScale.Count) {
                LogBreakpointError();
                return;
            }
            Undo.RegisterCompleteObjectUndo(rectTransform, "set responsive scale");
#endif
            rectTransform.localScale = breakpointLocalScale[activeIndex];
        }

    }
    
}