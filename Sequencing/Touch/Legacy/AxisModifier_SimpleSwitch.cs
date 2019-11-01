using UnityEngine;

namespace AltSalt.Sequencing.Touch
{
    public partial class AxisModifier
    {
        public partial class SimpleSwitchLegacy2
        {
            [SerializeField]
            private SimpleSwitchClip _sourceClip;

            private SimpleSwitchClip sourceClip
            {
                get => _sourceClip;
                set => _sourceClip = value;
            }

            public static SimpleSwitchLegacy2 CreateInstance(Touch_Controller touchController, Touch_Data touchData,SimpleSwitchClip sourceClip)
            {
                //var inputData = ScriptableObject.CreateInstance(typeof(SimpleSwitch)) as SimpleSwitch;
                var inputData = new SimpleSwitchLegacy2();

                inputData.touchController = touchController;
                inputData.touchData = touchData;
                inputData.sourceClip = sourceClip;

                double midpoint = sourceClip.startTime + (sourceClip.endTime - sourceClip.startTime) / 2d;
                inputData.inflectionPoint =
                    (float) MasterSequence.LocalToMasterTime(touchData.masterSequence,
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