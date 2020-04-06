using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.TestTools;

namespace AltSalt.Maestro.Sequencing.Autorun.Tests
{
    public class AutorunControllerTests
    {
        private AppSettings appSettings;
        
        private bool testInitialized = false;
        private RootConfig rootConfig;
        private Autorun_Controller autorunController;
        private SequenceController sequenceController;
        private PlayableDirector playableDirector;
        
        [UnitySetUp]
        public IEnumerator _Setup()
        {
            if(EditorSettings.enterPlayModeOptionsEnabled == false || (EditorSettings.enterPlayModeOptions & EnterPlayModeOptions.DisableDomainReload) == 0)
            {
                throw new System.Exception("You must have domain reload disabled to perform these tests");
            }
            
            if (testInitialized == false) {
                testInitialized = true;
                EditorSceneManager.OpenScene(
                    ProjectNamespaceData.namespaceData[ModuleNamespace.Autorun].editorPath + "Tests/Scenes/SequencingTests.unity");
                
                yield return new EnterPlayMode();
            }
            
            appSettings = Utils.GetAppSettings();
                
            rootConfig = Object.FindObjectOfType<RootConfig>();
            rootConfig.Configure();
            autorunController = rootConfig.GetComponentInChildren<Autorun_Controller>();
            sequenceController = rootConfig.GetComponentInChildren<SequenceController>();
            playableDirector = rootConfig.GetComponentInChildren<PlayableDirector>();
            playableDirector.RebuildGraph();
        }

        [UnityTest]
        public IEnumerator _Creates_Autorun_Data_List()
        {
            yield return null;
            
            List<Autorun_Data> autorunDataList = autorunController.autorunData;
            Assert.Greater(autorunDataList.Count, 0);
        }
        
        [UnityTest]
        public IEnumerator _Configures_Autorun_Data()
        {
            yield return null;
            
            List<Autorun_Data> autorunDataList = autorunController.autorunData;
            Autorun_Data autorunData = autorunDataList[0];

            List<AutorunExtents> autorunIntervals = autorunData.autorunIntervals;
            
            Assert.AreEqual(autorunIntervals[0].startTime, 0);
            Assert.AreEqual(autorunIntervals[0].endTime, 0.5f);
            
            Assert.AreEqual(autorunIntervals[1].startTime, 0.5f);
            Assert.AreEqual(autorunIntervals[1].endTime, 1f);
            
            Assert.AreEqual(autorunIntervals[2].startTime, 1f);
            Assert.AreEqual(autorunIntervals[2].endTime, 1.5f);
            
            Assert.AreEqual(autorunIntervals[3].startTime, 1.5f);
            Assert.AreEqual(autorunIntervals[3].endTime, double.MaxValue);
        }
        
    }
}