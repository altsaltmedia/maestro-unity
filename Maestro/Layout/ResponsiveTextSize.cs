using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

namespace AltSalt.Maestro.Layout
{
    public class ResponsiveTextSize : ResponsiveText, IResponsiveSaveable
    {      
        public List<float> breakpointTextSize = new List<float>();

#if UNITY_EDITOR
        
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        [InfoBox("Saves the position at the current breakpoint index.")]
        public void SaveValue()
        {
            int targetBreakpointIndex = 0;
            
            if (aspectRatioBreakpoints.Count < 1) {
                targetBreakpointIndex = 0;
            }
            else {
                targetBreakpointIndex = Utils.GetValueIndexInList(sceneAspectRatio.Value, aspectRatioBreakpoints);
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