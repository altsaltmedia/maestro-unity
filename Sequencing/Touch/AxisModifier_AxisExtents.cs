using System;
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Sequencing.Touch
{
    [Serializable]
    public class AxisModifier_AxisExtents : AxisModifier_TouchExtents
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
        [TitleGroup("$"+nameof(propertiesTitle))]
        private double _markerMasterTime;

        public double markerMasterTime
        {
            get => _markerMasterTime;
            protected set => _markerMasterTime = value;
        }

        [SerializeField]
        [TitleGroup("$"+nameof(timeThresholdsTitle))]
        [PropertyOrder(6)]
        private AxisModifier_AxisExtents _previousAxisExtents;

        public AxisModifier_AxisExtents previousAxisExtents
        {
            get => _previousAxisExtents;
            protected set => _previousAxisExtents = value;
        }

        [SerializeField]
        [TitleGroup("$"+nameof(timeThresholdsTitle))]
        [PropertyOrder(7)]
        private AxisModifier_AxisExtents _nextAxisExtents;

        public AxisModifier_AxisExtents nextAxisExtents
        {
            get => _nextAxisExtents;
            protected set => _nextAxisExtents = value;
        }
        
        protected override string timeThresholdsTitle => "Transition Thresholds";

        protected override bool ShowStartTransitionThreshold()
        {
            return true;
        }
        
        protected override bool ShowEndTransitionThreshold()
        {
            return true;
        }

        public AxisModifier_AxisExtents(AxisModifier axisModifier, Touch_Data touchData, AxisModifier_AxisMarker axisMarker)
        {
            this.axisModifier = axisModifier;
            this.sequence = touchData.sequence;
            this.inverted = axisMarker.inverted;
            this.description = axisMarker.description;
            this.markerMasterTime = MasterSequence.LocalToMasterTime(touchData.sequence.sequenceConfig.masterSequence, touchData.sequence, axisMarker.time);
            
            if (axisMarker.axisType == AxisType.Y)
            {
                this.swipeAxis = axisModifier.touchController.ySwipeAxis;
                this.momentumAxis = axisModifier.touchController.yMomentumAxis;
            }
            else
            {
                this.swipeAxis = axisModifier.touchController.xSwipeAxis;
                this.momentumAxis = axisModifier.touchController.xMomentumAxis;
            }
        }


        public AxisModifier_TouchExtents Configure(AxisModifier_TouchExtents previousAxisExtents, AxisModifier_TouchExtents nextAxisExtents)
        {

            if (previousAxisExtents != null) {

                this.startTime = previousAxisExtents.endTime;

                if (previousAxisExtents is AxisModifier_AxisExtents prevExtents) {
                        
                    this.previousAxisExtents = prevExtents;

                    if (prevExtents.inverted == true) {
                        this.startTransitionThreshold = this.markerMasterTime + this.axisModifier.invertTransitionSpread;
                    }
                    else {
                        this.startTransitionThreshold = this.markerMasterTime + this.axisModifier.swipeTransitionSpread;
                    }
                }
            } else {
                this.startTime = this.markerMasterTime;
                this.startTransitionThreshold = this.markerMasterTime;
            }

            if (nextAxisExtents != null) {
                
                switch (nextAxisExtents) {
                    
                    case AxisModifier_ForkExtents _:
                        this.endTime = nextAxisExtents.startTime;
                        this.endTransitionThreshold = nextAxisExtents.startTime;
                        break;
                    
                    case AxisModifier_AxisExtents nextExtents:
                    {
                        this.endTime = nextExtents.markerMasterTime;
                        this.nextAxisExtents = nextExtents;

                        if (nextExtents.inverted == true) {
                            this.endTransitionThreshold =
                                nextExtents.markerMasterTime - this.axisModifier.invertTransitionSpread;
                        }
                        else {
                            this.endTransitionThreshold =
                                nextExtents.markerMasterTime - this.axisModifier.swipeTransitionSpread;
                        }

                        break;
                    }
                }
            } else {
                this.endTime = Double.MaxValue;
                this.endTransitionThreshold = Double.MaxValue;
            }

            return this;
        }
    }
}