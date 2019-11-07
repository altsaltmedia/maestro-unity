using System;
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Sequencing.Touch
{
    [Serializable]
    public class AxisExtents : TouchExtents
    {
        [SerializeField]
        [TitleGroup("$"+nameof(propertiesTitle))]
        private Axis _swipeAxis;

        public Axis swipeAxis
        {
            get => _swipeAxis;
            private set => _swipeAxis = value;
        }

        [SerializeField]
        [TitleGroup("$"+nameof(propertiesTitle))]
        private Axis _momentumAxis;

        public Axis momentumAxis
        {
            get => _momentumAxis;
            private set => _momentumAxis = value;
        }
            
        [SerializeField]
        [TitleGroup("$"+nameof(propertiesTitle))]
        private bool _inverted;

        public bool inverted
        {
            get => _inverted;
            private set => _inverted = value;
        }

        [SerializeField]
        [FoldoutGroup("$"+nameof(debugTitle))]
        private double _markerMasterTime;

        public double markerMasterTime
        {
            get => _markerMasterTime;
            protected set => _markerMasterTime = value;
        }
        
        protected override string timeThresholdsTitle => "Axis Transition Thresholds";

        public AxisExtents(AxisMonitor axisMonitor, Touch_Data touchData, AxisMarker axisMarker)
        {
            this.axisMonitor = axisMonitor;
            this.sequence = touchData.sequence;
            this.inverted = axisMarker.inverted;
            this.description = axisMarker.description;
            this.markerMasterTime = MasterSequence.LocalToMasterTime(touchData.sequence.sequenceConfig.masterSequence, touchData.sequence, axisMarker.time);
            
            if (axisMarker.axisType == AxisType.Y)
            {
                this.swipeAxis = axisMonitor.touchController.ySwipeAxis;
                this.momentumAxis = axisMonitor.touchController.yMomentumAxis;
            }
            else
            {
                this.swipeAxis = axisMonitor.touchController.xSwipeAxis;
                this.momentumAxis = axisMonitor.touchController.xMomentumAxis;
            }
        }


        public override TouchExtents Configure(TouchExtents previousTouchExtents, TouchExtents nextTouchExtents)
        {
            if (previousTouchExtents != null) {

                this.startTime = previousTouchExtents.endTime;
                this.previousTouchExtents = previousTouchExtents;

                if (previousTouchExtents is AxisExtents previousAxisExtents) {

                    if (previousAxisExtents.inverted == true) {
                        this.startTransitionThreshold = this.markerMasterTime + this.axisMonitor.invertTransitionSpread;
                    }
                    else {
                        this.startTransitionThreshold = this.markerMasterTime + this.axisMonitor.swipeTransitionSpread;
                    }
                }
                else {
                    this.startTransitionThreshold = this.startTime;
                }
            } else {
                this.startTime = this.markerMasterTime;
                this.startTransitionThreshold = this.markerMasterTime;
            }

            if (nextTouchExtents != null) {

                this.nextTouchExtents = nextTouchExtents;
                
                switch (nextTouchExtents) {
                    
                    case TouchForkExtents forkExtents:
                        this.endTime = forkExtents.startTime;
                        this.endTransitionThreshold = forkExtents.startTime;
                        break;
                    
                    case AxisExtents nextAxisExtents:
                    {
                        this.endTime = nextAxisExtents.markerMasterTime;

                        if (nextAxisExtents.inverted == true) {
                            this.endTransitionThreshold =
                                nextAxisExtents.markerMasterTime - this.axisMonitor.invertTransitionSpread;
                        }
                        else {
                            this.endTransitionThreshold =
                                nextAxisExtents.markerMasterTime - this.axisMonitor.swipeTransitionSpread;
                        }

                        break;
                    }
                }
            } else {
                this.endTime = double.MaxValue;
                this.endTransitionThreshold = double.MaxValue;
            }

            return this;
        }
    }
}