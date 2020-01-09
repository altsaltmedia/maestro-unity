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

namespace AltSalt.Maestro.Sensors
{
    public class SwipeMonitor : MonoBehaviour
    {
        // Invert Axis variable
        [Required]
        [SerializeField]
        private BoolReference _invertYInput = new BoolReference();

        private bool invertYInput => _invertYInput.GetValue(this.gameObject);

        [Required]
        [SerializeField]
        private BoolReference _invertXInput = new BoolReference();

        private bool invertXInput => _invertXInput.GetValue(this.gameObject);

        // Swipe variables
        [Required]
        [FoldoutGroup("Swipe Variables")]
        public SimpleEventTrigger OnTouchStartEvent;

        [Required]
        [FoldoutGroup("Swipe Variables")]
        public SimpleEventTrigger OnLongTouchEvent;

        [Required]
        [FoldoutGroup("Swipe Variables")]
        public SimpleEventTrigger OnSwipeEvent;

        [Required]
        [FoldoutGroup("Swipe Variables")]
        public SimpleEventTrigger OnSwipeEndEvent;

        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Swipe Variables")]
        [InfoBox("SwipeForce is generated and modified dynamically at run time", InfoMessageType.Info)]
        public V2Reference touchStartPosition;

        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Swipe Variables")]
        public BoolReference isFlicked;

        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Swipe Variables")]
        [InfoBox("SwipeForce is generated and modified dynamically at run time", InfoMessageType.Info)]
        [SerializeField]
        private V2Reference _swipeForce;

        private Vector2 swipeForce
        {
            set => _swipeForce.GetVariable(this.gameObject).SetValue(value);
        }

        [SerializeField]
        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Swipe Variables")]
        [FormerlySerializedAs("swipeMinMax")]
        private FloatReference _swipeMinMax = new FloatReference(120f);

        private float swipeMinMax => _swipeMinMax.GetValue(this.gameObject);

        [SerializeField]
        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Swipe Variables")]
        [FormerlySerializedAs("xSensitivity")]
		private FloatReference _xSensitivity = new FloatReference(.0015f);

        private float xSensitivity => _xSensitivity.GetValue(this.gameObject);

        [SerializeField]
        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Swipe Variables")]
        [FormerlySerializedAs("ySensitivity")]
        private FloatReference _ySensitivity = new FloatReference(.0015f);

        private float ySensitivity => _ySensitivity.GetValue(this.gameObject);

        [SerializeField]
        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Swipe Variables")]
        [FormerlySerializedAs("zSensitivity")]
        private FloatReference _zSensitivity = new FloatReference(.0015f);

        private float zSensitivity => _zSensitivity.GetValue(this.gameObject);

        [SerializeField]
        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Swipe Variables")]
        [FormerlySerializedAs("flickThreshold")]
        private FloatReference _flickThreshold = new FloatReference(1000);

        private float flickThreshold => _flickThreshold.GetValue(this.gameObject);

        private bool isSwiping = false;

        // Momentum variables
        [ValidateInput(nameof(IsPopulated))]
        [FoldoutGroup("Momentum Variables")]
        [SerializeField]
        private SimpleEventTrigger _momentumUpdate;

        private SimpleEventTrigger momentumUpdate => _momentumUpdate;

        [Required]
        [FoldoutGroup("Momentum Variables")]
        public SimpleEventTrigger MomentumDepleted;

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
        [InfoBox("Momentum is generated and modified dynamically at run time", InfoMessageType.Info)]
        [SerializeField]
		private V2Reference _swipeMonitorMomentum;

        private Vector2 swipeMonitorMomentum
        {
            get => _swipeMonitorMomentum.GetValue(this.gameObject);
            set => _swipeMonitorMomentum.GetVariable(this.gameObject).SetValue(value);
        }

        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Momentum Variables")]
        [InfoBox("Momentum Cache is generated and modified dynamically at run time", InfoMessageType.Info)]
        [SerializeField]
        private V2Reference _swipeMonitorMomentumCache;

        private Vector2 swipeMonitorMomentumCache
        {
            get => _swipeMonitorMomentumCache.GetValue(this.gameObject);
            set => _swipeMonitorMomentumCache.GetVariable(this.gameObject).SetValue(value);
        }

        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Momentum Variables")]
        [SerializeField]
        [FormerlySerializedAs("momentumMinMax")]
        private FloatReference _momentumMinMax = new FloatReference();

        private float momentumMinMax => _momentumMinMax.GetValue(this.gameObject);

        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Momentum Variables")]
        [SerializeField]
        private FloatReference _momentumDecay = new FloatReference(.935f);

        private float momentumDecay
        {
            get => _momentumDecay.GetValue(this.gameObject);
            set => _momentumDecay.GetVariable(this.gameObject).SetValue(value);
        }

        [SerializeField]
        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Momentum Variables")]
        [FormerlySerializedAs("momentumSensitivity")]
        private FloatReference _momentumSensitivity = new FloatReference();

        private float momentumSensitivity => _momentumSensitivity.GetValue(this.gameObject);

        [SerializeField]
        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Momentum Variables")]
        [FormerlySerializedAs("gestureTimeMultiplier")]
        private FloatReference _gestureTimeMultiplier = new FloatReference();

        private float gestureTimeMultiplier => _gestureTimeMultiplier.GetValue(this.gameObject);

        [SerializeField]
        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Momentum Variables")]
        [FormerlySerializedAs("cancelMomentumTimeThreshold")]
        private FloatReference _cancelMomentumTimeThreshold = new FloatReference();

        private float cancelMomentumTimeThreshold => _cancelMomentumTimeThreshold.GetValue(this.gameObject);

        [SerializeField]
        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Momentum Variables")]
        [FormerlySerializedAs("cancelMomentumMagnitudeThreshold")]
        private FloatReference _cancelMomentumMagnitudeThreshold = new FloatReference();

        private float cancelMomentumMagnitudeThreshold => _cancelMomentumMagnitudeThreshold.GetValue(this.gameObject);

        [SerializeField]
        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Momentum Variables")]
        [FormerlySerializedAs("pauseMomentumThreshold")]
        private FloatReference _pauseMomentumThreshold = new FloatReference();

        private float pauseMomentumThreshold => _pauseMomentumThreshold.GetValue(this.gameObject);

        private bool _hasMomentum = false;

        private bool hasMomentum
        {
            get => _hasMomentum;
            set => _hasMomentum = value;
        }

        // Debug variables
        [Required]
        [FoldoutGroup("Debug Variables")]
		public SimpleEventTrigger UpdateVarsDebug;

        [FoldoutGroup("Debug Variables")]
        public FloatReference swipeMagnitudeDebug = new FloatReference();

        [FoldoutGroup("Debug Variables")]
        public V2Reference swipeVectorDebug = new V2Reference();

        [FoldoutGroup("Debug Variables")]
        public V2Reference swipeDeltaDebug = new V2Reference();

        [FoldoutGroup("Debug Variables")]
        public FloatReference gestureActionTimeDebug = new FloatReference();
        
        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private StringReference _swipeDirection;

        private string swipeDirection
        {
            get => _swipeDirection.value;
            set => _swipeDirection.variable.SetValue(value);
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

        // Update is called once per frame
        private void Update()
        {
            if(hasMomentum == true) {
                // All momentum is executed through momentumForce. However, we calculate the momentumForce
                // via a momentumCache, which is modified dynamically based on swipe input, decay value, etc.
                swipeMonitorMomentum = new Vector2(swipeMonitorMomentumCache.x, swipeMonitorMomentumCache.y);
				
				momentumUpdate.RaiseEvent(this.gameObject);
				
				if (swipeMonitorMomentum.sqrMagnitude < .00001f) {
                    MomentumDepleted.RaiseEvent(this.gameObject);
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
            touchStartPosition.GetVariable(this.gameObject).SetValue(gesture.position);
            OnTouchStartEvent.RaiseEvent(this.gameObject);
        }

        public void OnTouchDown(Gesture gesture)
        {
            if (!isSwiping && gesture.actionTime >= pauseMomentumThreshold) {
                OnLongTouchEvent.RaiseEvent(this.gameObject);
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
            Vector3 swipeDistance = Utils.ConvertV2toV3(swipeVector);
            if (hasMomentum == true) {
                int swipeSign = Utils.GetV3Sign(swipeDistance);
                int momentumSign = Utils.GetV3Sign(swipeMonitorMomentum);

                if (swipeSign != momentumSign) {
                    HaltMomentum();
                }
            }

            Vector3 newSwipeForce = NormalizeVectorInfo(swipeVector, swipeMinMax);

            swipeForce = newSwipeForce;
            OnSwipeEvent.RaiseEvent(this.gameObject);
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
                isFlicked.GetVariable(this.gameObject).SetValue(true);
            } else {
                isFlicked.GetVariable(this.gameObject).SetValue(false);
            }

            // Debug
            /**/ swipeMagnitudeDebug.GetVariable(this.gameObject).SetValue(gesture.deltaPosition.sqrMagnitude);
            /**/ swipeVectorDebug.GetVariable(this.gameObject).SetValue(gesture.swipeVector);
            /**/ swipeDeltaDebug.GetVariable(this.gameObject).SetValue(gesture.deltaPosition);
            /**/ gestureActionTimeDebug.GetVariable(this.gameObject).SetValue(gesture.actionTime);
            /**/ UpdateVarsDebug.RaiseEvent(this.gameObject);


            // Cancel momentum on certain long swipe gestures with low delta at the end of the movement.
            if(gesture.deltaPosition.sqrMagnitude < cancelMomentumMagnitudeThreshold && gesture.actionTime > cancelMomentumTimeThreshold) {
                MomentumDepleted.RaiseEvent(this.gameObject);
                OnSwipeEndEvent.RaiseEvent(this.gameObject);
                return;
            }

            Vector2 swipeVector = gesture.position - gesture.startPosition;

            // We use momentumMinMax to clamp the value, but as of this writing that
            // value is set to 1000, which means that most swipes won't get clamped at all.
            // I leave this functionality here in case we do need to use it at some point.
            Vector3 swipeEndForce = NormalizeVectorInfo(swipeVector, momentumMinMax);

            // Using swipeEndForce, run through a function that
            // allows us to adjust momentum sensitivity, if desired
            Vector3 swipeMomentum = Utils.ExponentiateV3(swipeEndForce, momentumSensitivity);

            // Our swipe time generally comes back less than 1 - so let's multiply
            // by 100, because dividing by a decimal makes our swipe too intense
            float normalizedGestureTime = gesture.actionTime * gestureTimeMultiplier;

            AddMomentum(swipeMomentum / normalizedGestureTime);

			isSwiping = false;
            
            OnSwipeEndEvent.RaiseEvent(this.gameObject);
        }

        public void AddMomentum(Vector3 momentumVector)
        {
            Vector3 momentumAdd = new Vector3(swipeMonitorMomentumCache.x + momentumVector.x, swipeMonitorMomentumCache.y + momentumVector.y);
            swipeMonitorMomentumCache = momentumAdd;
			hasMomentum = true;
        }

        private Vector3 NormalizeVectorInfo(Vector2 rawVector, float minMax)
        {
            // Clamp swipe values based on max/min threshold
            Vector2 clampedVector = Utils.ClampVectorValue(rawVector, minMax);

            // Our values come back as a Vector2 coordinate value. Our first app
            // made use of forces on all 3 axes, so let's convert to a V3 - 
            // we don't currently need the Z, but can refactor at a later date.
            Vector3 v3Vector = Utils.ConvertV2toV3(clampedVector);

            // Due to the peculiarities of working with Unity's timeline system,
            // the X value always comes in opposite of what we need. In some instances,
            // scrolling vertically for example, we also need to inver the Y.
            Vector3 correctedV3;
            
            if(invertYInput == true) {
                if(invertXInput == true) {
                    correctedV3 = Utils.InvertV3Values(v3Vector, new string[]{"x", "y"});
                } else {
                    correctedV3 = Utils.InvertV3Values(v3Vector, new string[]{"y"});
                }
            } else {
                if (invertXInput == true) {
                    correctedV3 = Utils.InvertV3Values(v3Vector, new string[]{"x"});
                } else {
                    correctedV3 = v3Vector;
                }
            }

            // Normalize information based on sensitivity, otherwise our values come back too high
            Vector3 v3Force = new Vector3(correctedV3.x * xSensitivity, correctedV3.y * ySensitivity, correctedV3.z * zSensitivity);

            return v3Force;
        }

        public void HaltMomentum()
        {
            hasMomentum = false;
            swipeMonitorMomentumCache = new Vector3(0, 0, 0);
            swipeMonitorMomentum = new Vector2(0, 0);
        }

        private static bool IsPopulated(SimpleEventTrigger attribute)
        {
            return Utils.IsPopulated(attribute);
        }
        
        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

        private static bool IsPopulated(V2Reference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

        private static bool IsPopulated(V3Reference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
        
        private static bool IsPopulated(StringReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}