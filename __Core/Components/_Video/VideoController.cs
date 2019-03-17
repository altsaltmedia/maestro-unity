using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Video;

namespace AltSalt {

#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class VideoController : MonoBehaviour {
        
        [Required]
        public SimpleEvent ClipsModifiedEvent;
        
        // Swipe variables
        [Required]
        [FoldoutGroup("Swipe Variables")]
        public Axis xSwipeAxis;
        
        [Required]
        [FoldoutGroup("Swipe Variables")]
        public Axis ySwipeAxis;
        
        [Required]
        [FoldoutGroup("Swipe Variables")]
        public Axis zSwipeAxis;
        
        [Required]
        [FoldoutGroup("Swipe Variables")]
        public Axis transitionAxis;
        
        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Swipe Variables")]
        public V3Reference swipeForce;
        
        // Momentum variables
        [Required]
        [FoldoutGroup("Momentum Variables")]
        public Axis xMomentumAxis;
        
        [Required]
        [FoldoutGroup("Momentum Variables")]
        public Axis yMomentumAxis;
        
        [Required]
        [FoldoutGroup("Momentum Variables")]
        public Axis zMomentumAxis;
        
        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Momentum Variables")]
        public V3Reference momentumForce;


        VideoPlayer video;
        bool isActive = true;

        void Start()
        {
            video = GetComponent<VideoPlayer>();
            video.Prepare();
            //video.Play();
        }

        //void Initiate()
        //{
        //    isActive = true;
        //}

        //public void UpdateClipWithSwipe()
        //{
        //    //Debug.Log("registering the swipe");
        //    if (isActive == true) {
        //        float clipModifier = 0;
                
        //        if (ySwipeAxis.Active) {
        //            //Debug.Log("y axis");
        //            //Debug.Log(swipeForce.y);
        //            clipModifier += swipeForce.Variable.Value.y;
        //        }
        //        if (xSwipeAxis.Active) {
        //            //Debug.Log("x axis");
        //            //Debug.Log(swipeForce.x);
        //            clipModifier += swipeForce.Variable.Value.x;
        //        }
        //        if (zSwipeAxis.Active) {
        //            //Debug.Log("z axis");
        //            //Debug.Log(swipeForce.z);
        //            clipModifier += swipeForce.Variable.Value.z;
        //        }

        //        //video.time = video.time += clipModifier;
        //        //clipModifier = clipModifier;


        //        video.time += (clipModifier * 50);

        //        //int stepModifier = (int)Math.Round(clipModifier);
        //        //video.frame += 
        //        ////for (int i = 1; i <= stepModifier; i++) {
        //        ////    video.StepForward(); 
        //        ////}
        //        //Debug.Log("Clip modifier");
        //        //Debug.Log(clipModifier);
        //        //Debug.Log("Video time");
        //        //Debug.Log(video.time);
        //        ClipsModifiedEvent.Raise();
        //    }

        //}
        
        //public void UpdateClipWithMomentum()
        //{
        //    if (isActive == true) {
        //        float clipModifier = 0;
                
        //        if (xMomentumAxis.Active) {
        //            //Debug.Log("x axis");
        //            //Debug.Log(swipeForce.x);
        //            clipModifier += momentumForce.Variable.Value.x;
        //        }
        //        if (yMomentumAxis.Active) {
        //            //Debug.Log("y axis");
        //            //Debug.Log(swipeForce.y);
        //            clipModifier += momentumForce.Variable.Value.y;
        //        }
        //        if (zMomentumAxis.Active) {
        //            //Debug.Log("z axis");
        //            //Debug.Log(swipeForce.z);
        //            clipModifier += momentumForce.Variable.Value.z;
        //        }
                
        //        video.time += (clipModifier * 10);
        //        //video.StepForward();
        //        ClipsModifiedEvent.Raise();
        //    }
        //}

        //private static bool IsPopulated(V3Reference attribute)
        //{
        //    return Utils.IsPopulated(attribute);
        //}

        //private static bool IsPopulated(List<Sequence> attribute)
        //{
        //    return Utils.IsPopulated(attribute);
        //}
    }
    
}