/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Sequencing.Touch
{   
    [Serializable]
    public class InvertSwitchLegacy : SimpleSwitchLegacy
    {
//        [ValueDropdown("invertSwitchValues")]
//		[FoldoutGroup("Invert Axis Variables", 2)]
//        public string switchType;
//
//        private string[] invertSwitchValues = new string[]{
//            "Activate Y Invert",
//            "Deactivate Y Invert",
//            "Activate X Invert",
//            "Deactivate X Invert"
//        };
//		
//        [ValidateInput("IsPopulated")]
//		[FoldoutGroup("Invert Axis Variables", 2)]
//        public BoolReference invertAxis;
//
//        [ValidateInput("IsPopulated")]
//        [FoldoutGroup("Invert Axis Variables", 2)]
//        public FloatReference invertTransitionSpread = new FloatReference();
//
//		public override void UpdateActiveAxes()
//		{
//            switch(switchType) {
//                case "Activate Y Invert":
//                    if(invertAxis.Value == false) {
//						if (parentClip.currentTime >= (_inflectionPoint.Value - invertTransitionSpread.Value)
//						    && parentClip.currentTime < _inflectionPoint.Value) {
//							invertAxis.Variable.SetValue(true);
//						}
//                    } else {
//                        // We double the invertTransitionSpread in the second condition so that we can keep the axis in question
//                        // inverted throughout the transition, then flip it after the threshold is passed
//                        if (parentClip.currentTime < (_inflectionPoint.Value - invertTransitionSpread.Value)
//                           && parentClip.currentTime >= (_inflectionPoint.Value - invertTransitionSpread.Value * 2)) {
//                            invertAxis.Variable.SetValue(false);
//                        }
//                    }
//                    break;
//
//                case "Deactivate Y Invert":
//                    if (invertAxis.Value == true) {
//                        if (parentClip.currentTime >= (_inflectionPoint.Value - invertTransitionSpread.Value)
//                            && parentClip.currentTime < _inflectionPoint.Value) {
//                            invertAxis.Variable.SetValue(false);
//                        }
//                    }
//                    else {
//                        // See note above
//                        if (parentClip.currentTime < _inflectionPoint.Value - invertTransitionSpread.Value
//                            && parentClip.currentTime >= (_inflectionPoint.Value - invertTransitionSpread.Value * 2)) {
//                            invertAxis.Variable.SetValue(true);
//                        }
//                    }
//                    break;
//
//                case "Activate X Invert":
//                    if (invertAxis.Value == false) {
//                        // We double the invertTransitionSpread in the second condition so that we can keep the axis in question
//                        // inverted throughout the transition, then flip it after the threshold is passed
//                        if (parentClip.currentTime >= (_inflectionPoint.Value - invertTransitionSpread.Value) &&
//                            parentClip.currentTime < (_inflectionPoint.Value)) {
//                            invertAxis.Variable.SetValue(true);
//                        }
//                    }
//                    else {
//                        if (parentClip.currentTime < _inflectionPoint.Value - invertTransitionSpread.Value
//                            && parentClip.currentTime >= (_inflectionPoint.Value - invertTransitionSpread.Value * 2)) {
//                            invertAxis.Variable.SetValue(false);
//                        }
//                    }
//                    break;
//
//                case "Deactivate X Invert":
//                    if(invertAxis.Value == true) {
//                        // See note above
//                        if (parentClip.currentTime >= _inflectionPoint.Value - invertTransitionSpread.Value
//                            && parentClip.currentTime < _inflectionPoint.Value) {
//                            invertAxis.Variable.SetValue(false);
//                        }
//                    } else {                  
//                        if (parentClip.currentTime < (_inflectionPoint.Value - invertTransitionSpread.Value)
//                            && parentClip.currentTime >= (_inflectionPoint.Value - invertTransitionSpread.Value * 2)) {
//                            invertAxis.Variable.SetValue(true);
//                        }
//                    }
//                    break;
//            }
//            base.UpdateActiveAxes();
//		}
//
//		private static bool IsPopulated(BoolReference attribute)
//        {
//            return Utils.IsPopulated(attribute);
//        }
//
//        private static bool IsPopulated(FloatReference attribute)
//        {
//            return Utils.IsPopulated(attribute);
//        }
	}	
}