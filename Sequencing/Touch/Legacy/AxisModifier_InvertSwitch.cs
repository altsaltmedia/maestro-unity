using System;
using UnityEngine;

namespace AltSalt.Sequencing.Touch
{
    public partial class AxisModifier
    {
        public partial class InvertSwitchLegacy2
        {
            [SerializeField]
            private InvertSwitchClip _sourceClip;

            private InvertSwitchClip sourceClip
            {
                get => _sourceClip;
                set => _sourceClip = value;
            }

            [SerializeField]
            private InvertSwitchType _switchType;

            public InvertSwitchType switchType
            {
                get => _switchType;
                set => _switchType = value;
            }

            public static InvertSwitchLegacy2 CreateInstance(Touch_Controller touchController, Touch_Data touchData, InvertSwitchClip sourceClip)
            {
                //var inputData = ScriptableObject.CreateInstance(typeof(InvertSwitch)) as InvertSwitch;
                var inputData = new InvertSwitchLegacy2();

                inputData.touchController = touchController;
                inputData.touchData = touchData;
                inputData.sourceClip = sourceClip;
                inputData.switchType = sourceClip.switchType;

                double midpoint = (sourceClip.endTime - sourceClip.startTime) / 2d;
                inputData.inflectionPoint =
                    (float) MasterSequence.LocalToMasterTime(touchData.masterSequence,
                        touchData.sequence, midpoint);

                // If we are activating the x negative, that means we must be coming from the y axis.
                // Or, if we are deactivating the y negative, that means we just completed an interval
                // using the y negative axis and are resetting the values back to normal.
                if (sourceClip.switchType == InvertSwitchType.ActivateXNeg ||
                    sourceClip.switchType == InvertSwitchType.DeactivateYNeg)
                {
                    inputData.swipeOrigin = touchController.ySwipeAxis;
                    inputData.swipeDestination = touchController.xSwipeAxis;
                    inputData.momentumOrigin = touchController.yMomentumAxis;
                    inputData.momentumDestination = touchController.xMomentumAxis;
                }
                // This is simply the inverse of the above.
                else if(sourceClip.switchType == InvertSwitchType.ActivateYNeg ||
                        sourceClip.switchType == InvertSwitchType.DeactivateXNeg)
                {
                    inputData.swipeOrigin = touchController.xSwipeAxis;
                    inputData.swipeDestination = touchController.ySwipeAxis;
                    inputData.momentumOrigin = touchController.xMomentumAxis;
                    inputData.momentumDestination = touchController.yMomentumAxis;
                }

                return inputData;
            }

            public static BiAxisSwitchLegacy2 CheckActivateSwitch(BaseAxisSwitchLegacy2 axisSwitch, double masterTime)
            {
                InvertSwitchLegacy2 invert = axisSwitch as InvertSwitchLegacy2;
                AxisModifier axisModifier = invert.touchController.axisModifier;

                switch (invert.switchType)
                {
                    case InvertSwitchType.ActivateYNeg:
                        
                        if (masterTime >= invert.inflectionPoint - axisModifier.invertTransitionSpread
                            && masterTime < invert.inflectionPoint)
                        {
                            invert.touchController.ySwipeAxis.inverted = true;
                            invert.touchController.yMomentumAxis.inverted = true;
                        }

                        // We double the invertTransitionSpread in the second condition so that we can keep the axis in question
                        // inverted throughout the transition, then flip it after the threshold is passed
                        else if (masterTime < invert.inflectionPoint - axisModifier.invertTransitionSpread
                            && masterTime >= invert.inflectionPoint - axisModifier.invertTransitionSpread * 2d)
                        {
                            invert.touchController.ySwipeAxis.inverted = false;
                            invert.touchController.yMomentumAxis.inverted = false;
                        }

                        break;

                    case InvertSwitchType.DeactivateYNeg:
                        
                        if (masterTime >= invert.inflectionPoint - axisModifier.invertTransitionSpread
                            && masterTime < invert.inflectionPoint)
                        {
                            invert.touchController.ySwipeAxis.inverted = false;
                            invert.touchController.yMomentumAxis.inverted = false;
                        }
                    
                        // We double the invertTransitionSpread in the second condition so that we can keep the axis in question
                        // inverted throughout the transition, then flip it after the threshold is passed
                        else if (masterTime < invert.inflectionPoint - axisModifier.invertTransitionSpread
                            && masterTime >= invert.inflectionPoint - axisModifier.invertTransitionSpread * 2d)
                        {
                            invert.touchController.ySwipeAxis.inverted = true;
                            invert.touchController.yMomentumAxis.inverted = true;
                        }

                        break;

                    case InvertSwitchType.ActivateXNeg:
                        
                        // We double the invertTransitionSpread in the second condition so that we can keep the axis in question
                        // inverted throughout the transition, then flip it after the threshold is passed
                        if (masterTime >= invert.inflectionPoint - axisModifier.invertTransitionSpread
                            && masterTime < invert.inflectionPoint)
                        {
                            invert.touchController.xSwipeAxis.inverted = true;
                        }
                    
                        else if (masterTime < invert.inflectionPoint - axisModifier.invertTransitionSpread
                            && masterTime >= invert.inflectionPoint - axisModifier.invertTransitionSpread * 2d)
                        {
                            invert.touchController.xSwipeAxis.inverted = false;
                        }

                        break;

                    case InvertSwitchType.DeactivateXNeg:
                        
                        // See note above
                        if (masterTime >= invert.inflectionPoint - axisModifier.invertTransitionSpread
                            && masterTime < invert.inflectionPoint)
                        {
                            invert.touchController.xSwipeAxis.inverted = false;
                        }
                    
                        else if (masterTime < invert.inflectionPoint - axisModifier.invertTransitionSpread
                            && masterTime >= invert.inflectionPoint - axisModifier.invertTransitionSpread * 2)
                        {
                            invert.touchController.xSwipeAxis.inverted = true;
                        }
                    
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return BiAxisSwitchLegacy2.CheckActivateSwitch(axisSwitch, masterTime);
            }
        }
    }
}