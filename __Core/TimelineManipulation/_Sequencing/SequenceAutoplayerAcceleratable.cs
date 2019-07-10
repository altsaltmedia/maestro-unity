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

            for(int q=0; q < sequenceLists.Count; q++) {

                for (int i = 0; i < sequenceLists[q].sequences.Count; i++) {

                    if (sequenceLists[q].sequences[i].Active == true && sequenceLists[q].sequences[i].hasAutoplay == true && sequenceLists[q].sequences[i].autoplayActive == true) {

                        for (int z = 0; z < sequenceLists[q].sequences[i].autoplayThresholds.Count; z++) {

                            if (sequenceLists[q].sequences[i].currentTime >= sequenceLists[q].sequences[i].autoplayThresholds[z].startTime &&
                                sequenceLists[q].sequences[i].currentTime < sequenceLists[q].sequences[i].autoplayThresholds[z].endTime) {

                                //if(isFlicked.Value == false) {
                                AutoplaySequence(sequenceLists[q].sequences[i], sequenceLists[q].sequences[i].autoplayThresholds[z]);
                                //} else {
                                //    if(currentTimescale.Value >= 0) {
                                //        AutoplayAcceleratedSequence(sequenceLists[q].sequences[i], sequenceLists[q].sequences[i].autoplayThresholds[q], false);
                                //    } else {
                                //        AutoplayAcceleratedSequence(sequenceLists[q].sequences[i], sequenceLists[q].sequences[i].autoplayThresholds[q], true);
                                //    }
                                //}

                                sequenceModified.RaiseEvent(this.gameObject);
                            }
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