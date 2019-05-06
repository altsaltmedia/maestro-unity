/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using UnityEngine;
using HedgehogTeam.EasyTouch;
using Sirenix.OdinInspector;

namespace AltSalt
{
    public class SwipeMonitor : MonoBehaviour
    {
        // Invert Axis variable
        [Required]
        public BoolReference invertYAxis;

        [Required]
        public BoolReference invertXAxis;

        // Swipe variables
        [Required]
        [FoldoutGroup("Swipe Variables")]
        public SimpleEvent OnTouchStartEvent;

        [Required]
        [FoldoutGroup("Swipe Variables")]
        public SimpleEvent OnLongTouchEvent;

        [Required]
        [FoldoutGroup("Swipe Variables")]
        public SimpleEvent OnSwipeEvent;

        [Required]
        [FoldoutGroup("Swipe Variables")]
        public SimpleEvent OnSwipeEndEvent;

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
        public V3Reference swipeForce;

        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Swipe Variables")]
        public FloatReference swipeMinMax = new FloatReference(120f);

        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Swipe Variables")]
		public FloatReference xSensitivity = new FloatReference(.0015f);

        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Swipe Variables")]
        public FloatReference ySensitivity = new FloatReference(.0015f);

        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Swipe Variables")]
        public FloatReference zSensitivity = new FloatReference(.0015f);

        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Swipe Variables")]
        public FloatReference flickThreshold = new FloatReference(1000);

        private bool isSwiping = false;

        // Momentum variables
        [Required]
        [FoldoutGroup("Momentum Variables")]
        public SimpleEvent MomentumUpdate;

        [Required]
        [FoldoutGroup("Momentum Variables")]
        public SimpleEvent MomentumDepleted;

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
        [InfoBox("MomentumForce is generated and modified dynamically at run time", InfoMessageType.Info)]
		public V3Reference momentumForce;

        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Momentum Variables")]
        [InfoBox("MomentumCache is generated and modified dynamically at run time", InfoMessageType.Info)]
		public V3Reference momentumCache;

        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Momentum Variables")]
        public FloatReference momentumMinMax = new FloatReference();

        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Momentum Variables")]
        public FloatReference momentumDecay = new FloatReference(.935f);

        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Momentum Variables")]
        public FloatReference momentumSensitivity = new FloatReference();

        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Momentum Variables")]
        public FloatReference gestureTimeMultiplier = new FloatReference();

        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Momentum Variables")]
        public FloatReference cancelMomentumTimeThreshold = new FloatReference();

        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Momentum Variables")]
        public FloatReference cancelMomentumMagnitudeThreshold = new FloatReference();

        [ValidateInput("IsPopulated")]
        [FoldoutGroup("Momentum Variables")]
        public FloatReference pauseMomentumThreshold = new FloatReference();

        private bool momentumActive = false;

        // Debug variables
        [Required]
        [FoldoutGroup("Debug Variables")]
		public SimpleEvent UpdateVarsDebug;

        [FoldoutGroup("Debug Variables")]
        public FloatReference swipeMagnitudeDebug = new FloatReference();

        [FoldoutGroup("Debug Variables")]
        public V2Reference swipeVectorDebug = new V2Reference();

        [FoldoutGroup("Debug Variables")]
        public V2Reference swipeDeltaDebug = new V2Reference();

        [FoldoutGroup("Debug Variables")]
        public FloatReference gestureActionTimeDebug = new FloatReference();

        // Update is called once per frame
        void Update()
        {
            if(momentumActive == true) {
                // All momentum is executed through momentumForce. However, we calculate the momentumForce
                // via a momentumCache, which is modified dynamically based on swipe input, decay value, etc.
                momentumForce.Variable.SetValue(new Vector3(momentumCache.Value.x, momentumCache.Value.y, momentumCache.Value.z));
				
				MomentumUpdate.Raise();
				
				if (momentumForce.Value.sqrMagnitude < .00001f) {
                    MomentumDepleted.Raise();
                    momentumCache.Variable.SetValue(new Vector3(0, 0, 0));
                    momentumActive = false;
				} else {
					momentumCache.Variable.SetValue(new Vector3(momentumCache.Value.x * momentumDecay.Value, momentumCache.Value.y * momentumDecay.Value, momentumCache.Value.z * momentumDecay.Value));            
				}
            }
        }

        public void OnTouchStart(Gesture gesture)
        {
            touchStartPosition.Variable.SetValue(gesture.position);
            OnTouchStartEvent.Raise();
        }

        public void OnTouchDown(Gesture gesture)
        {
            if (!isSwiping && gesture.actionTime >= pauseMomentumThreshold) {
                OnLongTouchEvent.Raise();
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

            // Get swipe value and compare w/ previous one to see
            // if we're reversing direction. If so, halt momentum.
            Vector3 swipeDistance = Utils.ConvertV2toV3(swipeVector);
            if (momentumActive == true) {
                int swipeDirection = Utils.GetV3Direction(swipeDistance);
                int momentumDirection = Utils.GetV3Direction(momentumForce);

                if (swipeDirection != momentumDirection) {
                    HaltMomentum();
                }
            }

            Vector3 newSwipeForce = NormalizeVectorInfo(swipeVector, swipeMinMax);

            swipeForce.Variable.SetValue(newSwipeForce);
            OnSwipeEvent.Raise();
        }

        public void OnSwipeEnd(Gesture gesture)
        {
            // Raise flick event
            if (gesture.deltaPosition.sqrMagnitude > flickThreshold.Value) {
                isFlicked.Variable.SetValue(true);
            } else {
                isFlicked.Variable.SetValue(false);
            }

            OnSwipeEndEvent.Raise();

            // Debug
            /**/ swipeMagnitudeDebug.Variable.SetValue(gesture.deltaPosition.sqrMagnitude);
            /**/ swipeVectorDebug.Variable.SetValue(gesture.swipeVector);
            /**/ swipeDeltaDebug.Variable.SetValue(gesture.deltaPosition);
            /**/ gestureActionTimeDebug.Variable.SetValue(gesture.actionTime);
            /**/ UpdateVarsDebug.Raise();


            // Cancel momentum on certain long swipe gestures with low delta at the end of the movement.
            if(gesture.deltaPosition.sqrMagnitude < cancelMomentumMagnitudeThreshold && gesture.actionTime > cancelMomentumTimeThreshold) {
                MomentumDepleted.Raise();
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
        }

        public void AddMomentum(Vector3 momentumVector)
        {
            Vector3 momentumAdd = new Vector3(momentumCache.Value.x + momentumVector.x, momentumCache.Value.y + momentumVector.y, momentumCache.Value.z + momentumVector.z);
            momentumCache.Variable.SetValue(momentumAdd);
			momentumActive = true;
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
            if(invertYAxis) {
                if(invertXAxis) {
                    correctedV3 = Utils.InvertV3Values(v3Vector, new string[]{"x", "y"});
                } else {
                    correctedV3 = Utils.InvertV3Values(v3Vector, new string[]{"y"});
                }
            } else {
                if (invertXAxis) {
                    correctedV3 = Utils.InvertV3Values(v3Vector, new string[]{"x"});
                } else {
                    correctedV3 = v3Vector;
                }
            }

            // Normalize information based on sensitivity, otherwise our values come back too high
            Vector3 v3Force = new Vector3(correctedV3.x * xSensitivity.Value, correctedV3.y * ySensitivity.Value, correctedV3.z * zSensitivity.Value);

            return v3Force;
        }

        public void HaltMomentum()
        {
            momentumActive = false;
            momentumCache.Variable.SetValue(new Vector3(0, 0, 0));
            momentumForce.Variable.SetValue(new Vector3(0, 0, 0));
        }

        public void UpdateMomentum(EventPayload eventPayload)
        {
            string fromAxis = eventPayload.GetStringValue(AxisDestination.fromAxis);
            string toAxis = eventPayload.GetStringValue(AxisDestination.toAxis);
		
   //         Debug.Log("cached momentum at update");
			//Debug.Log(momentumCache.Value.ToString("F8"));
			//Debug.Log("update momentum");
			//Debug.Log(eventPayload.GetStringValue("fromAxis"));
			//Debug.Log(eventPayload.GetStringValue("toAxis"));

            // Replace the momentum cache's new active axis with the previous active axis's momentum
            momentumCache.Variable.Value[Utils.GetAxisId(toAxis)] = momentumCache.Variable.Value[Utils.GetAxisId(fromAxis)];

            //Debug.Log(momentumCache.Value.ToString("F8"));
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
    }
}