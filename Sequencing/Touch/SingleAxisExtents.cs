using UnityEngine;

namespace AltSalt.Sequencing.Touch
{
    public class SingleAxisExtents : AxisExtents
    {
        [SerializeField]
        private Axis _swipeAxis;

        public Axis swipeAxis
        {
            get => _swipeAxis;
            private set => _swipeAxis = value;
        }

        [SerializeField]
        private Axis _momentumAxis;

        public Axis momentumAxis
        {
            get => _momentumAxis;
            private set => _momentumAxis = value;
        }
        
        [SerializeField]
        private bool _inverted;

        public bool inverted
        {
            get => _inverted;
            private set => _inverted = value;
        }
        
        public SingleAxisExtents(AxisModifier axisModifier, TouchController.TouchData touchData, SingleAxis singleAxis)
        {
            this.axisModifier = axisModifier;
            this.axisModifierMarker = singleAxis;
            
            if (singleAxis.axisType == AxisType.Y)
            {
                this.swipeAxis = axisModifier.touchController.ySwipeAxis;
                this.momentumAxis = axisModifier.touchController.yMomentumAxis;
            }
            else
            {
                this.swipeAxis = axisModifier.touchController.xSwipeAxis;
                this.momentumAxis = axisModifier.touchController.xMomentumAxis;
            }
            
            this.startTime = singleAxis.time;
        }

    }
}