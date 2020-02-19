using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Sequencing.Navigation
{
    public class Bookmarker : NavigationModule
    {
        [Required]
        [SerializeField]
        private NavigationController _navigationController;

        protected override NavigationController navigationController
        {
            get => _navigationController;
            set => _navigationController = value;
        }

        [SerializeField]
        private ComplexEventManualTrigger _storeData = new ComplexEventManualTrigger();

        private ComplexEventManualTrigger storeData => _storeData;

        private bool bookmarkingEnabled =>
            navigationController.appSettings.bookmarkingEnabled;

        private BoolReference hasBookmarkReference
            => navigationController.appSettings.GetHasBookmarkReference(this);

        private bool hasBookmark
        {
            get => hasBookmarkReference.GetValue();
            set => hasBookmarkReference.SetValue(this.gameObject, value);
        }

        private bool bookmarkLoadingCompleted
        {
            get => navigationController.appSettings.GetBookmarkLoadingCompleted(this, inputGroupKey);
            set => navigationController.appSettings.SetBookmarkLoadingCompleted(this.gameObject, inputGroupKey, value);
        }
        
        private StringReference lastOpenedSceneReference
            => navigationController.appSettings.GetLastOpenedSceneReference(this);
        
        private string lastOpenedScene
        {
            set => lastOpenedSceneReference.SetValue(this.gameObject, value);
        }
        
        private StringReference lastLoadedSequenceReference 
            => navigationController.appSettings.GetLastLoadedSequenceReference(this);

        private string lastLoadedSequence
        {
            get => lastLoadedSequenceReference.GetValue();
            set => lastLoadedSequenceReference.SetValue(this.gameObject, value);
        }

        private FloatReference lastLoadedSequenceTimeReference 
            => navigationController.appSettings.GetLastLoadedSequenceTimeReference(this);
        
        private float lastLoadedSequenceTime
        {
            get => lastLoadedSequenceTimeReference.GetValue();
            set => lastLoadedSequenceTimeReference.SetValue(this.gameObject, value);
        }
        
        private void OnApplicationPause(bool paused)
        {
            if (Application.isPlaying == false) return;
            
            SaveBookmark();
        }

        private void OnApplicationQuit()
        {
            if (Application.isPlaying == false) return;
            
            SaveBookmark();
        }

        public void ActivateBookmark()
        {
            bookmarkLoadingCompleted = false;
            
            if (moduleActive == true && bookmarkingEnabled == true && hasBookmark == true) {
                MasterSequence targerMasterSequence = navigationController.masterSequences.Find(x => x.sequenceControllers.Find(
                    y => y.sequence.name == lastLoadedSequence));
                if (targerMasterSequence != null) {
                    targerMasterSequence.SetElapsedTime(lastLoadedSequenceTime);

                    for (int i = 0; i < navigationController.masterSequences.Count; i++) {
                        if (navigationController.masterSequences[i] != targerMasterSequence) {
                            navigationController.masterSequences[i].sequenceControllers.ForEach(x =>
                                {
                                    x.sequence.active = false;
                                });
                        }
                    }
                }
            }

            bookmarkLoadingCompleted = true;
        }

        public void SaveBookmark()
        {
            if (moduleActive == false || bookmarkingEnabled == false) return;

            for (int i = 0; i < navigationController.masterSequences.Count; i++) {
                SequenceController targetSequenceConfig = navigationController.masterSequences[i].sequenceControllers
                    .Find(x => x.sequence.active == true);

                if (targetSequenceConfig != null) {
                    hasBookmark = true;
                    lastOpenedScene = this.gameObject.scene.name;
                    lastLoadedSequence = targetSequenceConfig.sequence.name;
                    lastLoadedSequenceTime = (float)targetSequenceConfig.masterSequence.elapsedTime;
                    
                    ComplexPayload complexPayload = ComplexPayload.CreateInstance();
                    complexPayload.scriptableObjectDictionary.Add(0, hasBookmarkReference.GetVariable());
                    complexPayload.scriptableObjectDictionary.Add(1, lastOpenedSceneReference.GetVariable());
                    complexPayload.scriptableObjectDictionary.Add(2, lastLoadedSequenceReference.GetVariable());
                    complexPayload.scriptableObjectDictionary.Add(3, lastLoadedSequenceTimeReference.GetVariable());
                    
                    // Send variables to UserDataController for saving
                    storeData.RaiseEvent(this.gameObject, complexPayload);
                    break;
                }
            }
        }
    }
}