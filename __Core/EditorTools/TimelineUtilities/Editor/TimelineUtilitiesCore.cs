using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEditor.Timeline;

namespace AltSalt
{
    public static class TimelineUtilitiesCore
    {
        static FloatReference currentTime = new FloatReference();

        public static float CurrentTime {

            get {
                PopulateTimeReference();
                return currentTime.Value;
            }

        }

        public static FloatReference GetCurrentTimeReference()
        {
            PopulateTimeReference();
            return currentTime;
        }

        public static FloatReference PopulateTimeReference()
        {
            if(currentTime.Variable == null) {
                currentTime.Variable = Utils.GetFloatVariable("TimelineCurrentTime");
            }
            return currentTime;
        }

        public static void RefreshTimelineWindow()
        {
            TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);
        }
    }
}