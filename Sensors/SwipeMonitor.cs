/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System;
using UnityEngine;
using HedgehogTeam.EasyTouch;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro.Sensors
{
    
    public class SwipeMonitor : MonoBehaviour
    {
        
        [SerializeField]
        private AppSettings _appSettings;

        private AppSettings appSettings
        {
            get
            {
                if (_appSettings == null) {
                    _appSettings = Utils.GetAppSettings();
                }
                return _appSettings;
            }
            set => _appSettings = value;
        }

        [SerializeField]
        private UserDataKeyReference _userDataKey = new UserDataKeyReference();

        private UserDataKey userDataKey => _userDataKey.GetVariable() as UserDataKey;
        

        // User defined

        private float ySensitivity => appSettings.GetYSensitivity(this.gameObject, userDataKey);
        
        private bool invertYInput => appSettings.GetInvertYInput(this.gameObject, userDataKey);
        
        private float xSensitivity => appSettings.GetXSensitivity(this.gameObject, userDataKey);
        
        private bool invertXInput => appSettings.GetInvertXInput(this.gameObject, userDataKey);
        
        
        // Input Group
        
        [SerializeField]
        private InputGroupKeyReference _inputGroupKey = new InputGroupKeyReference();

        private InputGroupKey inputGroupKey => _inputGroupKey.GetVariable() as InputGroupKey;
        
        private bool isSwiping
        {
            get => appSettings.GetIsSwiping(this.gameObject, inputGroupKey);
            set => appSettings.SetIsSwiping(this.gameObject, inputGroupKey, value);
        }
        
        private Vector2 swipeForce
        {
            set => appSettings.SetSwipeForce(this.gameObject, inputGroupKey, value);
        }

        private Vector2 touchStartPosition
        {
            set => appSettings.SetTouchStartPosition(this.gameObject, inputGroupKey, value);
        }
        
        private float swipeMinMax => appSettings.GetSwipeMinMax(this.gameObject, inputGroupKey);
        
        private float flickThreshold => appSettings.GetFlickThreshold(this.gameObject, inputGroupKey);
        
        private bool isFlicked
        {
            set => appSettings.SetIsFlicked(this.gameObject, inputGroupKey, value);
        }

        private SimpleEventTrigger onTouchStart => appSettings.GetOnTouchStart(this.gameObject, inputGroupKey);

        private SimpleEventTrigger onLongTouch => appSettings.GetOnLongTouch(this.gameObject, inputGroupKey);

        private SimpleEventTrigger onSwipe => appSettings.GetOnSwipe(this.gameObject, inputGroupKey);

        private SimpleEventTrigger onSwipeEnd => appSettings.GetOnSwipeEnd(this.gameObject, inputGroupKey);

        private SimpleEventTrigger momentumUpdate => appSettings.GetMomentumUpdate(this.gameObject, inputGroupKey);
        
        private SimpleEventTrigger momentumDepleted => appSettings.GetMomentumDepleted(this.gameObject, inputGroupKey);
        
        private Vector2 swipeMonitorMomentum
        {
            get => appSettings.GetSwipeMonitorMomentum(this.gameObject, inputGroupKey);
            set => appSettings.SetSwipeMonitorMomentum(this.gameObject, inputGroupKey, value);
        }

        private Vector2 swipeMonitorMomentumCache
        {
            get => appSettings.GetSwipeMonitorMomentumCache(this.gameObject, inputGroupKey);
            set => appSettings.SetSwipeMonitorMomentumCache(this.gameObject, inputGroupKey, value);
        }
        
        private float momentumMinMax => appSettings.GetMomentumMinMax(this.gameObject, inputGroupKey);

        private float momentumDecay => appSettings.GetMomentumDecay(this.gameObject, inputGroupKey);

        private float momentumSensitivity => appSettings.GetMomentumSensitivity(this.gameObject, inputGroupKey);

        private float gestureTimeMultiplier => appSettings.GetGestureTimeMultiplier(this.gameObject, inputGroupKey);

        private float cancelMomentumTimeThreshold => appSettings.GetCancelMomentumTimeThreshold(this.gameObject, inputGroupKey);

        private float cancelMomentumMagnitudeThreshold => appSettings.GetCancelMomentumMagnitudeThreshold(this.gameObject, inputGroupKey);

        private float pauseMomentumThreshold => appSettings.GetPauseMomentumTimeThreshold(this.gameObject, inputGroupKey);

        private string swipeDirection
        {
            set => appSettings.SetSwipeDirection(this.gameObject, inputGroupKey, value);
        }
        
        private bool _hasMomentum = false;

        private bool hasMomentum
        {
            get => _hasMomentum;
            set => _hasMomentum = value;
        }

        [ShowInInspector]
        private Vector2[] _swipeHistory = new Vector2[10];

        private Vector2[] swipeHistory
        {
            get => _swipeHistory;
            set => _swipeHistory = value;
        }

        [ShowInInspector]
        [ReadOnly]
        private int _swipeHistoryIndex;

        private int swipeHistoryIndex
        {
            get => _swipeHistoryIndex;
            set => _swipeHistoryIndex = value;
        }
        
        private static SwipeMonitor ResetSwipeHistory(SwipeMonitor swipeMonitor)
        {
            swipeMonitor.swipeHistory = new Vector2[10];
            swipeMonitor.swipeHistoryIndex = 0;
            return swipeMonitor;
        }
        
#if UNITY_EDITOR
        private void OnEnable()
        {
            if (appSettings == null) {
                appSettings = Utils.GetAppSettings();
            }
        }
#endif

        // Update is called once per frame
        private void Update()
        {
            if(hasMomentum == true) {
                // All momentum is executed through momentumForce. However, we calculate the momentumForce
                // via a momentumCache, which is modified dynamically based on swipe input, decay value, etc.
                swipeMonitorMomentum = new Vector2(swipeMonitorMomentumCache.x, swipeMonitorMomentumCache.y);
				
				momentumUpdate.RaiseEvent(this.gameObject);
				
				if (swipeMonitorMomentum.sqrMagnitude < .00001f) {
                    momentumDepleted.RaiseEvent(this.gameObject);
                    swipeMonitorMomentumCache = new Vector2(0, 0);
                    hasMomentum = false;
				} else {
					swipeMonitorMomentumCache = new Vector2(swipeMonitorMomentumCache.x * momentumDecay, swipeMonitorMomentumCache.y * momentumDecay);            
				}
            }
        }

        public void OnTouchStart(Gesture gesture)
        {
            ResetSwipeHistory(this);
            touchStartPosition = gesture.position;
            onTouchStart.RaiseEvent(this.gameObject);
        }

        public void OnTouchDown(Gesture gesture)
        {
            if (!isSwiping && gesture.actionTime >= pauseMomentumThreshold) {
                onLongTouch.RaiseEvent(this.gameObject);
                HaltMomentum();
            }
        }

        public void OnSwipeStart(Gesture gesture)
        {            
            isSwiping = true;
        }

        /*
        * @OnSwipe
        * 
        * Translates the camera linearly on finger drag
        * 
        */
        public void OnSwipe(Gesture gesture)
        {
            Vector2 swipeVector = gesture.deltaPosition;
            swipeDirection = GetSwipeDirection(this, swipeVector);

            // Get swipe value and compare w/ previous one to see
            // if we're reversing direction. If so, halt momentum.
            
            if (hasMomentum == true) {
                int swipeSign = Utils.GetV2Sign(swipeVector);
                int momentumSign = Utils.GetV2Sign(swipeMonitorMomentum);

                if (swipeSign != momentumSign) {
                    HaltMomentum();
                }
            }

            Vector2 newSwipeForce = NormalizeVectorInfo(swipeVector, swipeMinMax);

            swipeForce = newSwipeForce;
            onSwipe.RaiseEvent(this.gameObject);
        }
        
        private static string GetSwipeDirection(SwipeMonitor swipeMonitor, Vector2 deltaPosition)
        {
            UpdateSwipeHistory(swipeMonitor, deltaPosition);
            Vector2 vectorDirection = Utils.GetVector2Direction(swipeMonitor.swipeHistory, swipeMonitor.invertXInput,
                swipeMonitor.invertYInput);
                
            if (Mathf.Abs(vectorDirection.x) > Mathf.Abs(vectorDirection.y)) {
                return vectorDirection.x > 0 ? nameof(SwipeDirection.xPositive) : nameof(SwipeDirection.xNegative);
            }
            
            return vectorDirection.y > 0 ? nameof(SwipeDirection.yPositive) : nameof(SwipeDirection.yNegative);
        }

        private static Vector2[] UpdateSwipeHistory(SwipeMonitor swipeMonitor, Vector2 deltaPosition)
        {
            if (swipeMonitor.swipeHistoryIndex < swipeMonitor.swipeHistory.Length - 1) {
                swipeMonitor.swipeHistory[swipeMonitor.swipeHistoryIndex] = deltaPosition;
            }

            swipeMonitor.swipeHistoryIndex++;
            if (swipeMonitor.swipeHistoryIndex > swipeMonitor.swipeHistory.Length - 1) {
                swipeMonitor.swipeHistoryIndex = 0;
            }
            
            return swipeMonitor.swipeHistory;
        }

        public void OnSwipeEnd(Gesture gesture)
        {
            // Raise flick event
            if (gesture.deltaPosition.sqrMagnitude > flickThreshold) {
                isFlicked = true;
            } else {
                isFlicked = false;
            }

//            // Debug
//            /**/ swipeMagnitudeDebug.GetVariable(this.gameObject).SetValue(gesture.deltaPosition.sqrMagnitude);
//            /**/ swipeVectorDebug.GetVariable(this.gameObject).SetValue(gesture.swipeVector);
//            /**/ swipeDeltaDebug.GetVariable(this.gameObject).SetValue(gesture.deltaPosition);
//            /**/ gestureActionTimeDebug.GetVariable(this.gameObject).SetValue(gesture.actionTime);
//            /**/ UpdateVarsDebug.RaiseEvent(this.gameObject);


            // Cancel momentum on certain long swipe gestures with low delta at the end of the movement.
            if(gesture.deltaPosition.sqrMagnitude < cancelMomentumMagnitudeThreshold && gesture.actionTime > cancelMomentumTimeThreshold) {
                momentumDepleted.RaiseEvent(this.gameObject);
                onSwipeEnd.RaiseEvent(this.gameObject);
                return;
            }

            Vector2 swipeVector = gesture.position - gesture.startPosition;

            // We use momentumMinMax to clamp the value, but as of this writing that
            // value is set to 1000, which means that most swipes won't get clamped at all.
            // I leave this functionality here in case we do need to use it at some point.
            Vector2 swipeEndForce = NormalizeVectorInfo(swipeVector, momentumMinMax);

            // Using swipeEndForce, run through a function that
            // allows us to adjust momentum sensitivity, if desired
            Vector2 swipeMomentum = Utils.ExponentiateV2(swipeEndForce, momentumSensitivity);

            // Our swipe time generally comes back less than 1 - so let's multiply
            // by 100, because dividing by a decimal makes our swipe too intense
            float normalizedGestureTime = gesture.actionTime * gestureTimeMultiplier;

            AddMomentum(swipeMomentum / normalizedGestureTime);

			isSwiping = false;
            
            onSwipeEnd.RaiseEvent(this.gameObject);
        }

        public void AddMomentum(Vector2 momentumVector)
        {
            Vector2 momentumAdd = new Vector2(swipeMonitorMomentumCache.x + momentumVector.x, swipeMonitorMomentumCache.y + momentumVector.y);
            swipeMonitorMomentumCache = momentumAdd;
			hasMomentum = true;
        }

        private Vector2 NormalizeVectorInfo(Vector2 rawVector, float minMax)
        {
            // Clamp swipe values based on max/min threshold
            Vector2 clampedVector = Utils.ClampVectorValue(rawVector, minMax);

            // Due to the peculiarities of working with Unity's timeline system,
            // the X value always comes in opposite of what we need. In some instances,
            // scrolling vertically for example, we also need to inver the Y.
            Vector2 correctedV2;
            
            if(invertYInput == true) {
                if(invertXInput == true) {
                    correctedV2 = Utils.InvertV2Values(clampedVector, new string[]{"x", "y"});
                } else {
                    correctedV2 = Utils.InvertV2Values(clampedVector, new string[]{"y"});
                }
            } else {
                if (invertXInput == true) {
                    correctedV2 = Utils.InvertV2Values(clampedVector, new string[]{"x"});
                } else {
                    correctedV2 = clampedVector;
                }
            }

            // Normalize information based on sensitivity, otherwise our values come back too high
            Vector2 v2Force = new Vector3(correctedV2.x * xSensitivity, correctedV2.y * ySensitivity);

            return v2Force;
        }

        public void HaltMomentum()
        {
            hasMomentum = false;
            swipeMonitorMomentumCache = new Vector2(0, 0);
            swipeMonitorMomentum = new Vector2(0, 0);
        }
        
    }
}