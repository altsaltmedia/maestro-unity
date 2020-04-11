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
    public class AutoplayerTests
    {
        private AppSettings appSettings;
        private RootConfig rootConfig;
        private Autoplayer autoplayer;
        private List<SequenceController> sequenceControllers;
        private PlayableDirector playableDirector;
        private Playable rootPlayable;

        private DateTime startTime;
        
        private bool isReversing
        {
            get => appSettings.GetIsReversing(rootConfig.gameObject, rootConfig.inputGroupKey);
            set => appSettings.SetIsReversing(rootConfig.gameObject, rootConfig.inputGroupKey, value);
        }
        
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
            autoplayer = rootConfig.GetComponentInChildren<Autoplayer>();
            sequenceControllers = rootConfig.masterSequences[0].sequenceControllers;

            sequenceControllers[0].sequence.active = true;
            sequenceControllers[1].sequence.active = false;
        }
        
        [UnityTest]
        public IEnumerator _Activates_Forward_Autoplay()
        {
            Sequence initialSequence = sequenceControllers[0].sequence;
            
            playableDirector = initialSequence.sequenceController.playableDirector;
            playableDirector.RebuildGraph();
            rootPlayable = playableDirector.playableGraph.GetRootPlayable(0);
            
            initialSequence.sequenceController.SetSequenceTime(initialSequence.sequenceController.gameObject, 0);
            appSettings.SetIsReversing(rootConfig.gameObject, rootConfig.inputGroupKey, false);
            
            autoplayer.ActivateEligibleForAutoplayAndRefresh();
            Assert.AreEqual(SequenceUpdateState.ForwardAutoplay, initialSequence.sequenceController.sequenceUpdateState);
            Assert.Greater(rootPlayable.GetSpeed(), 0);
            
            startTime = DateTime.UtcNow;
            while ((DateTime.UtcNow - startTime).TotalSeconds < .2) { yield return null; }
            
            Assert.Greater(initialSequence.currentTime, 0);
        }

        [UnityTest]
        public IEnumerator _Pauses_On_Forward_Autoplay()
        {
            Sequence initialSequence = sequenceControllers[0].sequence;
            
            playableDirector = initialSequence.sequenceController.playableDirector;
            playableDirector.RebuildGraph();
            rootPlayable = playableDirector.playableGraph.GetRootPlayable(0);
            
            initialSequence.sequenceController.SetSequenceTime(initialSequence.sequenceController.gameObject, 0);
            appSettings.SetIsReversing(rootConfig.gameObject, rootConfig.inputGroupKey, false);
            
            // Play once
            autoplayer.ActivateEligibleForAutoplayAndRefresh();
            Assert.AreEqual(SequenceUpdateState.ForwardAutoplay, initialSequence.sequenceController.sequenceUpdateState);;
            Assert.Greater(rootPlayable.GetSpeed(), 0);
            
            startTime = DateTime.UtcNow;
            while ((DateTime.UtcNow - startTime).TotalSeconds < .6) { yield return null; }

            // Check that we paused at first interval
            Assert.AreEqual(SequenceUpdateState.ManualUpdate, initialSequence.sequenceController.sequenceUpdateState);
            Assert.AreEqual(0, rootPlayable.GetSpeed());
            Assert.AreEqual(0.5f, (float)initialSequence.currentTime);

            // Play again
            autoplayer.ActivateEligibleForAutoplayAndRefresh();
            Assert.AreEqual(SequenceUpdateState.ForwardAutoplay, initialSequence.sequenceController.sequenceUpdateState);
            Assert.Greater(rootPlayable.GetSpeed(), 0);
            
            startTime = DateTime.UtcNow;
            while ((DateTime.UtcNow - startTime).TotalSeconds < .6) { yield return null; };
            
            // Check that we paused at 2nd interval
            Assert.AreEqual(SequenceUpdateState.ManualUpdate, initialSequence.sequenceController.sequenceUpdateState);
            Assert.AreEqual(0, rootPlayable.GetSpeed());
            Assert.AreEqual(1f, initialSequence.currentTime);
        }

        [UnityTest]
        public IEnumerator _Activates_Reverse_Autoplay()
        {
            Sequence initialSequence = sequenceControllers[0].sequence;
            
            playableDirector = initialSequence.sequenceController.playableDirector;
            playableDirector.RebuildGraph();
            rootPlayable = playableDirector.playableGraph.GetRootPlayable(0);
            
            initialSequence.sequenceController.SetSequenceTime(initialSequence.sequenceController.gameObject, 1.5f);
            appSettings.SetIsReversing(rootConfig.gameObject, rootConfig.inputGroupKey, true);
            
            startTime = DateTime.UtcNow;
            while ((DateTime.UtcNow - startTime).TotalSeconds < .6) { yield return null; }

            autoplayer.ActivateEligibleForAutoplayAndRefresh();
            
            Assert.AreEqual(SequenceUpdateState.ManualUpdate, initialSequence.sequenceController.sequenceUpdateState);
            Assert.AreEqual(0, rootPlayable.GetSpeed());
            
            startTime = DateTime.UtcNow;
            while ((DateTime.UtcNow - startTime).TotalSeconds < .6) { yield return null; }
            
            Assert.Less(initialSequence.currentTime, 1.5);
        }
        
        [UnityTest]
        public IEnumerator _Pauses_On_Reverse_Autoplay()
        {
            Sequence initialSequence = sequenceControllers[0].sequence;
            
            playableDirector = initialSequence.sequenceController.playableDirector;
            playableDirector.RebuildGraph();
            rootPlayable = playableDirector.playableGraph.GetRootPlayable(0);
            
            initialSequence.sequenceController.SetSequenceTime(initialSequence.sequenceController.gameObject, 1.5f);
            appSettings.SetIsReversing(rootConfig.gameObject, rootConfig.inputGroupKey, true);
            
            // Give the scene time to set up before starting
            startTime = DateTime.UtcNow;
            while ((DateTime.UtcNow - startTime).TotalSeconds < .6) { yield return null; };

            // Play once
            autoplayer.ActivateEligibleForAutoplayAndRefresh();
            Assert.AreEqual(SequenceUpdateState.ManualUpdate, initialSequence.sequenceController.sequenceUpdateState);

            startTime = DateTime.UtcNow;
            while ((DateTime.UtcNow - startTime).TotalSeconds < .6) { yield return null; }

            // Check that we paused at first interval
            Assert.AreEqual(SequenceUpdateState.ManualUpdate, initialSequence.sequenceController.sequenceUpdateState);
            Assert.AreEqual(0, rootPlayable.GetSpeed());
            Assert.AreEqual(1f, (float)initialSequence.currentTime);
            
            // Play again
            autoplayer.ActivateEligibleForAutoplayAndRefresh();
            Assert.AreEqual(SequenceUpdateState.ManualUpdate, initialSequence.sequenceController.sequenceUpdateState);
            
            startTime = DateTime.UtcNow;
            while ((DateTime.UtcNow - startTime).TotalSeconds < .6) { yield return null; };
            
            // Check that we paused at 2nd interval
            Assert.AreEqual(SequenceUpdateState.ManualUpdate, initialSequence.sequenceController.sequenceUpdateState);
            Assert.AreEqual(0, rootPlayable.GetSpeed());
            Assert.AreEqual(.5f, (float)initialSequence.currentTime);
        }
        
        [UnityTest]
        public IEnumerator _Autoplays_Following_Sequence_On_Forward()
        {
            Sequence initialSequence = sequenceControllers[0].sequence;
            Sequence adjacentSequence = sequenceControllers[1].sequence;
            
            initialSequence.active = true;
            initialSequence.sequenceController.playableDirector.RebuildGraph();
            
            adjacentSequence.active = false;
            
            initialSequence.sequenceController.SetSequenceTime(initialSequence.sequenceController.gameObject, 1.6f);
            appSettings.SetIsReversing(rootConfig.gameObject, rootConfig.inputGroupKey, false);
            autoplayer.ActivateEligibleForAutoplayAndRefresh();
            
            startTime = DateTime.UtcNow;
            while ((DateTime.UtcNow - startTime).TotalSeconds < 1) { yield return null; }
            
            Assert.AreEqual(false, initialSequence.sequenceController.gameObject.active);
            Assert.AreEqual(true, adjacentSequence.sequenceController.gameObject.active);
            Assert.Greater(adjacentSequence.currentTime, 0);
        }
        
        [UnityTest]
        public IEnumerator _Autoplays_Preceding_Sequence_On_Reverse()
        {
            Sequence initialSequence = sequenceControllers[1].sequence;
            Sequence adjacentSequence = sequenceControllers[0].sequence;
            
            initialSequence.active = true;
            initialSequence.sequenceController.playableDirector.RebuildGraph();
            
            adjacentSequence.active = false;

            initialSequence.sequenceController.SetSequenceTime(initialSequence.sequenceController.gameObject, .5f);
            appSettings.SetIsReversing(rootConfig.gameObject, rootConfig.inputGroupKey, true);
            
            startTime = DateTime.UtcNow;
            while ((DateTime.UtcNow - startTime).TotalSeconds < 1) { yield return null; }
            
            autoplayer.ActivateEligibleForAutoplayAndRefresh();
            
            startTime = DateTime.UtcNow;
            while ((DateTime.UtcNow - startTime).TotalSeconds < 1) { yield return null; }
            
            Assert.AreEqual(false, initialSequence.sequenceController.gameObject.active);
            Assert.AreEqual(true, adjacentSequence.sequenceController.gameObject.active);
            Assert.Less(adjacentSequence.currentTime, adjacentSequence.duration);
        }
    }
}