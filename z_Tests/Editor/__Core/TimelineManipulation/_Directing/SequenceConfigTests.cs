using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;
using AltSalt;

namespace Tests
{
    public class SequenceConfigTests
    {
		Sequence sequence;

		[SetUp]
		public void BeforeEveryTest()
		{
			sequence = ScriptableObject.CreateInstance(typeof(Sequence)) as Sequence;
		}

        List<StartEndThreshold> GetStartEndThresholds()
        {
            List<StartEndThreshold> startEndThresholds = new List<StartEndThreshold>();
            startEndThresholds.Add(new StartEndThreshold(1d, 2d));
            startEndThresholds.Add(new StartEndThreshold(5d, 10d));
            startEndThresholds.Add(new StartEndThreshold(14d, 22d));

            return startEndThresholds;
        }

        [Test]
        public void _Test_GetAutoplayThresholds()
        {
            List<double> startTimes = new List<double> { 10, 15, 20, 25 };
            List<double> endTimes = new List<double> { 12, 17, 22, 27 };

            List<StartEndThreshold> startEndThresholds = ConfigSequence.CreateStartEndThresholds(startTimes, endTimes);

            for (int i = 0; i < startEndThresholds.Count; i++) {
                Assert.AreEqual(startTimes[i], startEndThresholds[i].startTime);
                Assert.AreEqual(endTimes[i], startEndThresholds[i].endTime);
            }
        }


        //[Test]
        //public void _Test_ConfigureAutoplay()
        //{
        //    List<StartEndThreshold> startEndThresholds = GetStartEndThresholds();

        //    sequenceConfig.ConfigureAutoplay(sequence, startEndThresholds);

        //    for(int i=0; i<startEndThresholds.Count; i++) {
        //        Assert.Contains(startEndThresholds[i], sequence.autoplayThresholds);
        //    }
        //}

        //[Test]
        //public void _Test_ConfigurePauseMomentum()
        //{
        //    List<StartEndThreshold> startEndThresholds = GetStartEndThresholds();

        //    sequenceConfig.ConfigurePauseMomentum(sequence, startEndThresholds);

        //    for (int i = 0; i < startEndThresholds.Count; i++) {
        //        Assert.Contains(startEndThresholds[i], sequence.pauseMomentumThresholds);
        //    }
        //}

        //[Test]
        //public void _Test_ConfigureAxisSwitches()
        //{
        //    sequenceConfig()
        //}

    }
}