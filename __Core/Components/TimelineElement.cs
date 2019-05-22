using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AltSalt {

    public class TimelineElement : MonoBehaviour
    {

#if UNITY_EDITOR
        
        [InfoBox("Saves the position at the current breakpoint index.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void AddRectTransformPosTrack()
        {
        
        }
        
        
        [InfoBox("Saves the position at the current breakpoint index.")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void AddTMProColorTrack()
        {
        
        }
#endif
    }
    
}