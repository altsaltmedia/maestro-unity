using System;
using UnityEngine;

namespace AltSalt.Sequencing.Touch
{
    public partial class AxisModifier
    {
        public partial class InvertSwitch
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

            public static InvertSwitch CreateInstance(TouchController touchController, TouchController.TouchData touchData, InvertSwitchClip sourceClip)
            {
                //var inputData = ScriptableObject.CreateInstance(typeof(InvertSwitch)) as InvertSwitch;
                var inputData = new InvertSwitch();

                inputData.touchController = touchController;
                inputData.touchData = touchData;
                inputData.sourceClip = sourceClip;
                inputData.switchType = sourceClip.switchType;

                double midpoint = (sourceClip.endTime - sourceClip.startTime) / 2d;
                inputData.inflectionPoint =
                    (float) MasterSequence.LocalToMasterTime(touchData.masterSequence,
                        touchData.sequence, midpoint);

                if (sourceClip.switchType == InvertSwitchType.ActivateXNeg ||
                    sourceClip.switchType == InvertSwitchType.DeactivateXNeg)
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

            public static BiAxisSwitch CheckActivateSwitch(BaseAxisSwitch axisSwitch, double masterTime)
            {
                InvertSwitch invert = axisSwitch as InvertSwitch;
                AxisModifier axisModifier = invert.touchController.axisModifier;

                switch (invert.switchType)
                {
                    case InvertSwitchType.ActivateYNeg:

                        if (invert.touchController.ySwipeAxis.inverted == false)
                        {
                            if (masterTime >= invert.inflectionPoint - axisModifier.invertTransitionSpread
                                && masterTime < invert.inflectionPoint)
                            {
                                invert.touchController.ySwipeAxis.inverted = true;
                                invert.touchController.yMomentumAxis.inverted = true;
                            }
                        }
                        else
                        {
                            // We double the invertTransitionSpread in the second condition so that we can keep the axis in question
                            // inverted throughout the transition, then flip it after the threshold is passed
                            if (masterTime < invert.inflectionPoint - axisModifier.invertTransitionSpread
                                && masterTime >= invert.inflectionPoint - axisModifier.invertTransitionSpread * 2d)
                            {
                                invert.touchController.ySwipeAxis.inverted = false;
                                invert.touchController.yMomentumAxis.inverted = false;
                            }
                        }

                        break;

                    case InvertSwitchType.DeactivateYNeg:

                        if (invert.touchController.ySwipeAxis.inverted == true)
                        {
                            if (masterTime >= invert.inflectionPoint - axisModifier.invertTransitionSpread
                                && masterTime < invert.inflectionPoint)
                            {
                                invert.touchController.ySwipeAxis.inverted = false;
                                invert.touchController.yMomentumAxis.inverted = false;
                            }
                        }
                        else
                        {
                            // We double the invertTransitionSpread in the second condition so that we can keep the axis in question
                            // inverted throughout the transition, then flip it after the threshold is passed
                            if (masterTime < invert.inflectionPoint - axisModifier.invertTransitionSpread
                                && masterTime >= invert.inflectionPoint - axisModifier.invertTransitionSpread * 2d)
                            {
                                invert.touchController.ySwipeAxis.inverted = true;
                                invert.touchController.yMomentumAxis.inverted = true;
                            }
                        }

                        break;

                    case InvertSwitchType.ActivateXNeg:

                        if (invert.touchController.xSwipeAxis.inverted == false)
                        {
                            // We double the invertTransitionSpread in the second condition so that we can keep the axis in question
                            // inverted throughout the transition, then flip it after the threshold is passed
                            if (masterTime >= invert.inflectionPoint - axisModifier.invertTransitionSpread
                                && masterTime < invert.inflectionPoint)
                            {
                                invert.touchController.xSwipeAxis.inverted = true;
                            }
                        }
                        else
                        {
                            if (masterTime < invert.inflectionPoint - axisModifier.invertTransitionSpread
                                && masterTime >= invert.inflectionPoint - axisModifier.invertTransitionSpread * 2d)
                            {
                                invert.touchController.xSwipeAxis.inverted = false;
                            }
                        }

                        break;

                    case InvertSwitchType.DeactivateXNeg:

                        if (invert.touchController.xSwipeAxis.inverted == true)
                        {
                            // See note above
                            if (masterTime >= invert.inflectionPoint - axisModifier.invertTransitionSpread
                                && masterTime < invert.inflectionPoint)
                            {
                                invert.touchController.xSwipeAxis.inverted = false;
                            }
                        }
                        else
                        {
                            if (masterTime < invert.inflectionPoint - axisModifier.invertTransitionSpread
                                && masterTime >= invert.inflectionPoint - axisModifier.invertTransitionSpread * 2)
                            {
                                invert.touchController.xSwipeAxis.inverted = true;
                            }
                        }

                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return BiAxisSwitch.CheckActivateSwitch(axisSwitch, masterTime);
            }
        }
    }
}