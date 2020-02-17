using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;

namespace AltSalt.Maestro.Sequencing.Navigation
{

    [RequireComponent(typeof(Slider))]
    public class Scrubber : NavigationModule, IPointerDownHandler, IPointerUpHandler
    {
        [ReadOnly]
        [ShowInInspector]
        private NavigationController _navigationController;

        protected override NavigationController navigationController
        {
            get => _navigationController;
            set => _navigationController = value;
        }
        
        private bool isScrubbing
        {
            set => navigationController.appSettings.SetIsScrubbing(this.gameObject, inputGroupKey, value);
        }
        
        private bool isReversing
        {
            set => navigationController.appSettings.SetIsReversing(this.gameObject, inputGroupKey, value);
        }

        private SimpleEventTrigger onScrub =>
            navigationController.appSettings.GetOnScrub(this.gameObject, inputGroupKey);

            [SerializeField]
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

        private float _previousValue;

        private float previousValue
        {
            get => _previousValue;
            set => _previousValue = value;
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

        public void OnPointerDown(PointerEventData eventData)
        {
            isScrubbing = true;
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            isScrubbing = false;
        }

        public void ScrubSequence(float newValue)
        {
            if (newValue > previousValue) {
                isReversing = false;
            }
            else {
                isReversing = true;
            }

            previousValue = newValue;
            activeMasterSequence.SetElapsedTime(newValue);
            onScrub.RaiseEvent(this.gameObject);
        }
    }

}