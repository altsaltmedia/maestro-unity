using UnityEngine;

namespace AltSalt.Sequencing.Touch
{
    public partial class AxisModifier
    {
        public partial class SimpleSwitch
        {
            [SerializeField]
            private SimpleSwitchClip _sourceClip;

            private SimpleSwitchClip sourceClip
            {
                get => _sourceClip;
                set => _sourceClip = value;
            }

            public static SimpleSwitch CreateInstance(TouchController touchController, TouchController.TouchData touchData,SimpleSwitchClip sourceClip)
            {
                var inputData = ScriptableObject.CreateInstance(typeof(SimpleSwitch)) as SimpleSwitch;

                inputData.touchController = touchController;
                inputData.touchData = touchData;
                inputData.sourceClip = sourceClip;

                double midpoint = (sourceClip.endTime - sourceClip.startTime) / 2d;
                inputData.inflectionPoint =
                    (float) touchData.masterSequence.LocalToMasterTime(touchData.masterSequence.sequenceData,
                        touchData.sequence, midpoint);

                if (sourceClip.switchType == SimpleSwitchType.YtoX)
                {
                    inputData.swipeOrigin = touchController.ySwipeAxis;
                    inputData.swipeDestination = touchController.xSwipeAxis;
                    inputData.momentumOrigin = touchController.yMomentumAxis;
                    inputData.momentumDestination = touchController.xMomentumAxis;
                }
                else
                {
                    inputData.swipeOrigin = touchController.xSwipeAxis;
                    inputData.swipeDestination = touchController.ySwipeAxis;
                    inputData.momentumOrigin = touchController.xMomentumAxis;
                    inputData.momentumDestination = touchController.yMomentumAxis;
                }

                return inputData;
            }
        }
    }
}