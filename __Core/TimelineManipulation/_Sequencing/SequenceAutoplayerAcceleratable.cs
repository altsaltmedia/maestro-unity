using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    public class SequenceAutoplayerAcceleratable : SequenceAutoplayer
    {
        [ValidateInput("IsPopulated")]
        public BoolReference isFlicked;

        [ValidateInput("IsPopulated")]
        public FloatReference currentTimescale;

        bool previousSwipeNegative = false;

        protected override void Update()
        {

            if (appSettings.autoplayActive.Value == false) {
                return;
            }

            for (int i = 0; i < sequenceList.sequences.Count; i++) {

                if (sequenceList.sequences[i].Active == true && sequenceList.sequences[i].hasAutoplay == true && sequenceList.sequences[i].autoplayActive == true) {

                    for (int q = 0; q < sequenceList.sequences[i].autoplayThresholds.Count; q++) {

                        if (sequenceList.sequences[i].currentTime >= sequenceList.sequences[i].autoplayThresholds[q].startTime &&
                            sequenceList.sequences[i].currentTime < sequenceList.sequences[i].autoplayThresholds[q].endTime) {

                            //if(isFlicked.Value == false) {
                            AutoplaySequence(sequenceList.sequences[i], sequenceList.sequences[i].autoplayThresholds[q]);
                            //} else {
                            //    if(currentTimescale.Value >= 0) {
                            //        AutoplayAcceleratedSequence(sequenceList.sequences[i], sequenceList.sequences[i].autoplayThresholds[q], false);
                            //    } else {
                            //        AutoplayAcceleratedSequence(sequenceList.sequences[i], sequenceList.sequences[i].autoplayThresholds[q], true);
                            //    }
                            //}

                            sequenceModified.RaiseEvent(this.gameObject);
                        }
                    }
                }
            }

            previousSwipeNegative = isReversing.Value;
        }

        protected void AutoplayAcceleratedSequence(Sequence targetSequence, StartEndThreshold targetAutoplayThreshold, bool isNegative)
        {
            if (isNegative == false) {
                // Update sequence moving forward
                targetSequence.ModifySequenceTime(Time.fixedDeltaTime);
                if (targetSequence.currentTime >= targetAutoplayThreshold.endTime) {
                    targetSequence.autoplayActive = false;
                }
            }
            else {
                // Update sequence moving backward
                targetSequence.ModifySequenceTime(Time.fixedDeltaTime * -1f);
                if (targetSequence.currentTime < targetAutoplayThreshold.startTime) {
                    targetSequence.autoplayActive = false;
                }
            }
        }

        protected static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }

}