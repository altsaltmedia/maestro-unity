using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;

namespace AltSalt.Maestro.Layout
{
    public class ResponsiveTextSize : ResponsiveText, IResponsiveSaveable
    {      
        public List<float> breakpointTextSize = new List<float>();

#if UNITY_EDITOR
        
        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateBreakpointDependencies();
        }
        
        protected override void UpdateBreakpointDependencies()
        {
            base.UpdateBreakpointDependencies();
            if (breakpointTextSize.Count == 0) {
                breakpointTextSize.Add(textMeshPro.fontSize);
            }
            
            if (breakpointTextSize.Count <= aspectRatioBreakpoints.Count) {
                Utils.ExpandList(breakpointTextSize, aspectRatioBreakpoints.Count);
            }
        }

        
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        [InfoBox("Saves the position at the current breakpoint index.")]
        public void SaveValue()
        {
            int targetBreakpointIndex = 0;
            
            if (aspectRatioBreakpoints.Count < 1) {
                targetBreakpointIndex = 0;
            }
            else {
                targetBreakpointIndex = Utils.GetValueIndexInList(sceneAspectRatio, aspectRatioBreakpoints);
            }

            Utils.ExpandList(breakpointTextSize, targetBreakpointIndex);
            breakpointTextSize[targetBreakpointIndex] = textMeshPro.fontSize;
        }
#endif

        public override void ExecuteResponsiveAction()
        {
            base.ExecuteResponsiveAction();
            if (hasBreakpoints == true) {
                SetValue(breakpointIndex);
            }
            else {
                SetValue(0);
            }
        }

        public void SetValue(int activeIndex)
        {
            if (activeIndex >= breakpointTextSize.Count) {
                LogBreakpointError();
				return;
            }
#if UNITY_EDITOR
            Undo.RegisterCompleteObjectUndo(textMeshPro, "set responsive text size");
#endif
            textMeshPro.fontSize = breakpointTextSize[activeIndex];
        }
    }
}