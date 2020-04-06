using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Playables;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro.Sequencing.Autorun.Tests
{
    public class LerperTests
    {
        private AppSettings appSettings;
        private RootConfig rootConfig;
        private Lerper lerper;
        private List<SequenceController> sequenceControllers;
        private PlayableDirector playableDirector;
        private Playable rootPlayable;

        private DateTime startTime;
        
        [UnitySetUp]
        public IEnumerator _Setup()
        {
            if(EditorSettings.enterPlayModeOptionsEnabled == false || (EditorSettings.enterPlayModeOptions & EnterPlayModeOptions.DisableDomainReload) == 0) {
                throw new System.Exception("You must have domain reload disabled to perform these tests");
            }
            
            EditorSceneManager.OpenScene(
                ProjectNamespaceData.namespaceData[ModuleNamespace.Autorun].editorPath + "Tests/Scenes/SequencingTests.unity");
            
            yield return new EnterPlayMode();

            appSettings = Utils.GetAppSettings();
                
            rootConfig = Object.FindObjectOfType<RootConfig>();
            rootConfig.Configure();
            lerper = rootConfig.GetComponentInChildren<Lerper>();
            sequenceControllers = rootConfig.masterSequences[0].sequenceControllers;

            sequenceControllers[0].sequence.active = true;
            sequenceControllers[1].sequence.active = false;
        }
        
        [UnityTest]
        public IEnumerator _Activates_Forward_Lerp()
        {
            Sequence initialSequence = sequenceControllers[0].sequence;
            
            playableDirector = initialSequence.sequenceController.playableDirector;
            playableDirector.RebuildGraph();
            rootPlayable = playableDirector.playableGraph.GetRootPlayable(0);
            
            initialSequence.sequenceController.SetSequenceTime(initialSequence.sequenceController.gameObject, 0);
            appSettings.SetIsReversing(rootConfig.gameObject, rootConfig.inputGroupKey, false);
            
            lerper.TriggerLerpSequences();
            Assert.AreEqual(SequenceUpdateState.ForwardAutoplay, sequenceControllers[0].sequenceUpdateState);
            Assert.Greater(rootPlayable.GetSpeed(), 0);
            
            startTime = DateTime.UtcNow;
            while ((DateTime.UtcNow - startTime).TotalSeconds < .2) { yield return null; }
            
            Assert.Greater(sequenceControllers[0].sequence.currentTime, 0);
        }
        
        [UnityTest]
        public IEnumerator _Pauses_On_Forward_Lerp()
        {
            Sequence initialSequence = sequenceControllers[0].sequence;
            
            playableDirector = initialSequence.sequenceController.playableDirector;
            playableDirector.RebuildGraph();
            rootPlayable = playableDirector.playableGraph.GetRootPlayable(0);
            
            initialSequence.sequenceController.SetSequenceTime(initialSequence.sequenceController.gameObject, 0);
            appSettings.SetIsReversing(rootConfig.gameObject, rootConfig.inputGroupKey, false);;
            
            yield return null;

            // Play once
            lerper.TriggerLerpSequences();
            Assert.AreEqual(SequenceUpdateState.ForwardAutoplay, sequenceControllers[0].sequenceUpdateState);
            Assert.Greater(rootPlayable.GetSpeed(), 0);
            
            startTime = DateTime.UtcNow;
            while ((DateTime.UtcNow - startTime).TotalSeconds < .6) { yield return null; }

            // Check that we paused at first interval
            Assert.AreEqual(SequenceUpdateState.ManualUpdate, sequenceControllers[0].sequenceUpdateState);
            Assert.AreEqual(0, rootPlayable.GetSpeed());
            Assert.AreEqual(0.5f, (float)sequenceControllers[0].sequence.currentTime, .015f);

            // Play again
            lerper.TriggerLerpSequences();
            Assert.AreEqual(SequenceUpdateState.ForwardAutoplay, sequenceControllers[0].sequenceUpdateState);
            Assert.Greater(rootPlayable.GetSpeed(), 0);
            
            startTime = DateTime.UtcNow;
            while ((DateTime.UtcNow - startTime).TotalSeconds < .6) { yield return null; }
            
            // Check that we paused at 2nd interval
            Assert.AreEqual(SequenceUpdateState.ManualUpdate, sequenceControllers[0].sequenceUpdateState);
            Assert.AreEqual(0, rootPlayable.GetSpeed());
            Assert.AreEqual(1f, (float)sequenceControllers[0].sequence.currentTime, .015f);
        }
        
        [UnityTest]
        public IEnumerator _Activates_Reverse_Lerp()
        {
            Sequence initialSequence = sequenceControllers[0].sequence;
            
            playableDirector = initialSequence.sequenceController.playableDirector;
            playableDirector.RebuildGraph();
            rootPlayable = playableDirector.playableGraph.GetRootPlayable(0);
            
            initialSequence.sequenceController.SetSequenceTime(initialSequence.sequenceController.gameObject, 1.5f);
            appSettings.SetIsReversing(rootConfig.gameObject, rootConfig.inputGroupKey, true);

            lerper.TriggerLerpSequences();
            Assert.AreEqual(SequenceUpdateState.ManualUpdate, sequenceControllers[0].sequenceUpdateState);
            Assert.AreEqual(0, rootPlayable.GetSpeed());
            
            startTime = DateTime.UtcNow;
            while ((DateTime.UtcNow - startTime).TotalSeconds < .6) { yield return null; }
            
            Assert.Less(sequenceControllers[0].sequence.currentTime, 1.5);
        }
        
        [UnityTest]
        public IEnumerator _Pauses_On_Reverse_Lerp()
        {
            Sequence initialSequence = sequenceControllers[0].sequence;
            
            playableDirector = initialSequence.sequenceController.playableDirector;
            playableDirector.RebuildGraph();
            rootPlayable = playableDirector.playableGraph.GetRootPlayable(0);
            
            initialSequence.sequenceController.SetSequenceTime(initialSequence.sequenceController.gameObject, 1.5f);
            appSettings.SetIsReversing(rootConfig.gameObject, rootConfig.inputGroupKey, true);
            
            // Give the scene time to set up before starting
            startTime = DateTime.UtcNow;
            while ((DateTime.UtcNow - startTime).TotalSeconds < .5) { yield return null; };

            // Play once
            lerper.TriggerLerpSequences();
            Assert.AreEqual(SequenceUpdateState.ManualUpdate, sequenceControllers[0].sequenceUpdateState);

            startTime = DateTime.UtcNow;
            while ((DateTime.UtcNow - startTime).TotalSeconds < .6) { yield return null; }

            // Check that we paused at first interval
            Assert.AreEqual(SequenceUpdateState.ManualUpdate, sequenceControllers[0].sequenceUpdateState);
            Assert.AreEqual(0, rootPlayable.GetSpeed());
            Assert.AreEqual(1, (float)sequenceControllers[0].sequence.currentTime, .03);
            
            // Play again
            lerper.TriggerLerpSequences();
            Assert.AreEqual(SequenceUpdateState.ManualUpdate, sequenceControllers[0].sequenceUpdateState);
            
            startTime = DateTime.UtcNow;
            while ((DateTime.UtcNow - startTime).TotalSeconds < .6) { yield return null; };
            
            // Check that we paused at 2nd interval
            Assert.AreEqual(SequenceUpdateState.ManualUpdate, sequenceControllers[0].sequenceUpdateState);
            Assert.AreEqual(0, rootPlayable.GetSpeed());
            Assert.AreEqual(.5, (float)sequenceControllers[0].sequence.currentTime, .03);
        }

        [UnityTest]
        public IEnumerator _Activates_Following_Sequence_On_Forward()
        {
            Sequence initialSequence = sequenceControllers[0].sequence;
            Sequence adjacentSequence = sequenceControllers[1].sequence;
            
            initialSequence.active = true;
            initialSequence.sequenceController.playableDirector.RebuildGraph();
            
            adjacentSequence.active = false;
            
            initialSequence.sequenceController.SetSequenceTime(initialSequence.sequenceController.gameObject, 1.6f);
            appSettings.SetIsReversing(rootConfig.gameObject, rootConfig.inputGroupKey, false);
            lerper.TriggerLerpSequences();
            
            startTime = DateTime.UtcNow;
            while ((DateTime.UtcNow - startTime).TotalSeconds < 1) { yield return null; }
            
            Assert.AreEqual(false, initialSequence.sequenceController.gameObject.active);
            Assert.AreEqual(true, adjacentSequence.sequenceController.gameObject.active);

            Autorun_Data initialAutorunData =
                lerper.autorunController.autorunData.Find(x => x.sequence == initialSequence);
            Assert.False(initialAutorunData.isLerping); ;
        }
        
        [UnityTest]
        public IEnumerator _Activates_Preceding_Sequence_On_Reverse()
        {
            Sequence initialSequence = sequenceControllers[1].sequence;
            Sequence adjacentSequence = sequenceControllers[0].sequence;
            
            initialSequence.active = true;
            initialSequence.sequenceController.playableDirector.RebuildGraph();
            
            adjacentSequence.active = false;

            initialSequence.sequenceController.SetSequenceTime(initialSequence.sequenceController.gameObject, .4f);
            appSettings.SetIsReversing(rootConfig.gameObject, rootConfig.inputGroupKey, true);
            lerper.TriggerLerpSequences();
            
            startTime = DateTime.UtcNow;
            while ((DateTime.UtcNow - startTime).TotalSeconds < 1) { yield return null; }
            
            Assert.AreEqual(false, initialSequence.sequenceController.gameObject.active);
            Assert.AreEqual(true, adjacentSequence.sequenceController.gameObject.active);
            
            Autorun_Data initialAutorunData =
                lerper.autorunController.autorunData.Find(x => x.sequence == initialSequence);
            Assert.False(initialAutorunData.isLerping);
        }
        
        [UnityTest]
        public IEnumerator _Can_Alternate_Between_Sequences_On_Forward()
        {
            Sequence initialSequence = sequenceControllers[0].sequence;
            Autorun_Data initialAutorunData =
                lerper.autorunController.autorunData.Find(x => x.sequence == initialSequence);
            
            Sequence adjacentSequence = sequenceControllers[1].sequence;
            Autorun_Data adjacentAutorunData =
                lerper.autorunController.autorunData.Find(x => x.sequence == adjacentSequence);
            
            initialSequence.active = true;
            initialSequence.sequenceController.playableDirector.RebuildGraph();
            
            adjacentSequence.active = false;
            
            /// Activate adjacent sequence
            initialSequence.sequenceController.SetSequenceTime(initialSequence.sequenceController.gameObject, 1.6f);
            appSettings.SetIsReversing(rootConfig.gameObject, rootConfig.inputGroupKey, false);
            lerper.TriggerLerpSequences();
            
            startTime = DateTime.UtcNow;
            while ((DateTime.UtcNow - startTime).TotalSeconds < 2.5) { yield return null; }
            
            Assert.AreEqual(false, initialSequence.sequenceController.gameObject.active);
            Assert.AreEqual(true, adjacentSequence.sequenceController.gameObject.active);
            Assert.False(initialAutorunData.isLerping);

            /// Activate initial sequence
            appSettings.SetIsReversing(rootConfig.gameObject, rootConfig.inputGroupKey, true);
            lerper.TriggerLerpSequences();
            
            startTime = DateTime.UtcNow;
            while ((DateTime.UtcNow - startTime).TotalSeconds < 2.5) { yield return null; }
            
            Assert.AreEqual(true, initialSequence.sequenceController.gameObject.active);
            Assert.AreEqual(false, adjacentSequence.sequenceController.gameObject.active);
            Assert.False(adjacentAutorunData.isLerping);
            
            /// Reactivate adjacent sequence
            appSettings.SetIsReversing(rootConfig.gameObject, rootConfig.inputGroupKey, false);
            lerper.TriggerLerpSequences();
            
            startTime = DateTime.UtcNow;
            while ((DateTime.UtcNow - startTime).TotalSeconds < 2.5) { yield return null; }
            
            Assert.AreEqual(false, initialSequence.sequenceController.gameObject.active);
            Assert.AreEqual(true, adjacentSequence.sequenceController.gameObject.active);
            Assert.False(initialAutorunData.isLerping);
        }
    }
}