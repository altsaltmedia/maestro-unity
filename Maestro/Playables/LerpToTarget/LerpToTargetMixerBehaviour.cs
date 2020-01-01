/***********************************************

Copyright © 2018 AltSalt Media, LLC.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AltSalt.Maestro
{
    public abstract class LerpToTargetMixerBehaviour : PlayableBehaviour
    {
        private double _currentTime;

        protected double currentTime
        {
            get => _currentTime;
            set => _currentTime = value;
        }

        // Utility vars - specified here to prevent garbage collection
        private int _inputCount;

        protected int inputCount
        {
            get => _inputCount;
            set => _inputCount = value;
        }

        private float _inputWeight;

        protected float inputWeight
        {
            get => _inputWeight;
            set => _inputWeight = value;
        }

        private float _modifier;

        protected float modifier
        {
            get => _modifier;
            set => _modifier = value;
        }
        
        private GameObject _directorObject;

        public GameObject directorObject
        {
            get => _directorObject;
            set => _directorObject = value;
        }
        
        public BoolReference _scrubberActive = new BoolReference();

        public bool scrubberActive
        {
            get => _scrubberActive.Value;
            set => _scrubberActive.Variable.SetValue(value);
        }
        
        private TrackAsset _parentTrack;

        public TrackAsset parentTrack
        {
            get => _parentTrack;
            set => _parentTrack = value;
        }

        //public IEnumerable<IMarker> markers;
        //public List<IMarker> markers;

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            base.PrepareFrame(playable, info);
            currentTime = playable.GetGraph().GetRootPlayable(0).GetTime();
        }

//        This ProcessFrame() method should be implemented and overridden in inheriting mixer behaviours
//
//        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
//        {
//            
//        }
    }   
}