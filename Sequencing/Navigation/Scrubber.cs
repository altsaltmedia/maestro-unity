using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Sequencing.Navigation
{

    [RequireComponent(typeof(Slider))]
    public class Scrubber : NavigationModule
    {
        [SerializeField]
        SimpleEventTrigger sequenceScrubbed;
        
        private Slider _slider;

        private Slider slider
        {
            get => _slider;
            set => _slider = value;
        }
        
        [ShowInInspector]
        [ReadOnly]
        private MasterSequence _activeMasterSequence;

        private MasterSequence activeMasterSequence
        {
            get => _activeMasterSequence;
            set => _activeMasterSequence = value;
        }

        // Use this for initialization
        private void OnEnable()
        {
            slider = GetComponent<Slider>();
        }

        public void RefreshScrubber(ComplexPayload complexPayload)
        {
            navigationController = complexPayload.GetObjectValue(DataType.systemObjectType) as NavigationController;
            activeMasterSequence = navigationController.activeMasterSequence;
            
            slider.maxValue = (float)activeMasterSequence.duration;
            slider.value = (float)activeMasterSequence.elapsedTime;
        }

        private void OnMouseUpAsButton()
        { 
            TriggerInputActionComplete();
        }

        public void ScrubSequence(float newValue)
        {
            activeMasterSequence.SetElapsedTime(newValue);
        }
    }

}