using UnityEngine;
using UnityEngine.Timeline;

namespace AltSalt.Maestro
{
    public abstract class TimelineTriggerClip : RegisterablePlayableAsset, ITimelineClipAsset
    {
        // This is here for implementation reference only. DO NOT attempt to use or access this when
        // creating subclasses; it won't work. Instead, create a new instance of your behaviour in question.
        // private LerpToTargetBehaviour template = new LerpToTargetBehaviour();
        [HideInInspector]
        public TrackAssetConfig _trackAssetConfig;

        public TrackAssetConfig trackAssetConfig
        {
            get => _trackAssetConfig;
            set => _trackAssetConfig = value;
        }
        
        private double _startTime;

        public double startTime
        {
            get => _startTime;
            set => _startTime = value;
        }

        private double _endTime;

        public double endTime
        {
            get => _endTime;
            set => _endTime = value;
        }

        public override double duration => 1d;

        public ClipCaps clipCaps => ClipCaps.None;
        
        //        This is here for reference - it should be overriden in your child class
//        so that the instance of your behaviour has all the necessary variables.
//
//        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
//        {
//
//            template.startTime = startTime;
//            template.endTime = endTime;
//            template.clipAsset = this;
//            template.directorObject = directorObject;
//
//            var playable = ScriptPlayable<LerpToTargetBehaviour>.Create(graph, template);
//            return playable;
//        }
    }
}