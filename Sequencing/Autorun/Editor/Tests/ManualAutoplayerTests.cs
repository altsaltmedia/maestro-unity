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
    public class ManualAutoplayerTests
    {
        private AppSettings appSettings;
        private RootConfig rootConfig;
        private ManualAutoplayer manualAutoplayer;
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
            Autorun_Controller autorunController = rootConfig.GetComponentInChildren<Autorun_Controller>();
            manualAutoplayer = autorunController.GetComponentInChildren<ManualAutoplayer>();
            sequenceControllers = rootConfig.masterSequences[0].sequenceControllers;

            sequenceControllers[0].sequence.active = true;
            sequenceControllers[1].sequence.active = false;
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
            manualAutoplayer.AutoplayAllSequences();
            
            startTime = DateTime.UtcNow;
            while ((DateTime.UtcNow - startTime).TotalSeconds < 1) { yield return null; }
            
            Assert.AreEqual(false, initialSequence.sequenceController.gameObject.active);
            Assert.AreEqual(true, adjacentSequence.sequenceController.gameObject.active);
            Assert.AreEqual(.5f, (float)adjacentSequence.currentTime);
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
            
            manualAutoplayer.AutoplayAllSequences();
            
            startTime = DateTime.UtcNow;
            while ((DateTime.UtcNow - startTime).TotalSeconds < 1) { yield return null; }
            
            Assert.AreEqual(false, initialSequence.sequenceController.gameObject.active);
            Assert.AreEqual(true, adjacentSequence.sequenceController.gameObject.active);
            Assert.AreEqual(1.5f, (float)adjacentSequence.currentTime);
        }
    }
}