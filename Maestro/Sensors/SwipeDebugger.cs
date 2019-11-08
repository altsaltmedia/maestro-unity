using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HedgehogTeam.EasyTouch;

namespace AltSalt.Maestro
{
    public class SwipeDebugger : MonoBehaviour
    {

        ////   Subscribe to events
        void OnEnable()
        {
            EasyTouch.On_SwipeStart += On_SwipeStart;
            EasyTouch.On_Swipe += On_Swipe;
            EasyTouch.On_SwipeEnd += On_SwipeEnd;
            EasyTouch.On_TouchStart += On_TouchStart;
            EasyTouch.On_TouchDown += On_TouchDown;
            EasyTouch.On_TouchUp += On_TouchUp;
        }

        //// Unsubscribe from events
        void OnDestroy()
        {
            EasyTouch.On_SwipeStart -= On_SwipeStart;
			EasyTouch.On_Swipe -= On_Swipe;
            EasyTouch.On_SwipeEnd -= On_SwipeEnd;
            EasyTouch.On_TouchStart -= On_TouchStart;
            EasyTouch.On_TouchDown -= On_TouchDown;
            EasyTouch.On_TouchUp -= On_TouchUp;
        }

        public void On_SwipeStart(Gesture gesture)
        {
            Debug.Log("on swipe start");
        }

        public void On_Swipe(Gesture gesture)
        {
            Debug.Log("on swipe");
        }

        public void On_SwipeEnd(Gesture gesture)
        {
            Debug.Log("on swipe end");
        }

        public void On_TouchStart(Gesture gesture)
        {
            Debug.Log("on touch start");
        }

        public void On_TouchDown(Gesture gesture)
        {
            Debug.Log("on touch down");
        }

        public void On_TouchUp(Gesture gesture)
        {
            Debug.Log("on touch up");
        }



    }

}