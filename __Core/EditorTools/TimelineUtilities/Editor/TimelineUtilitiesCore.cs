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
        public static List<TimelineClip> timelineClips = new List<TimelineClip>();

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

        public static void RefreshTimelineContentsAddedOrRemoved()
        {
            TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);
        }

        public static void RefreshTimelineContentsModified()
        {
            TimelineEditor.Refresh(RefreshReason.ContentsModified);
        }

        public static void RefreshTimelineRedrawWindow()
        {
            TimelineEditor.Refresh(RefreshReason.WindowNeedsRedraw);
        }
    }
}