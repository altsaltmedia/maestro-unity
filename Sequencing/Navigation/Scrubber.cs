using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;

namespace AltSalt.Maestro.Sequencing.Navigation
{

    [RequireComponent(typeof(Slider))]
    public class Scrubber : NavigationModule, IPointerDownHandler, IPointerUpHandler
    {
        
        [ShowInInspector]
        [SerializeField]
        private NavigationController _navigationController;

        protected override NavigationController navigationController
        {
            get => _navigationController;
            set => _navigationController = value;
        }
        
        private bool isScrubbing
        {
            get => navigationController.appSettings.GetIsScrubbing(this.gameObject, inputGroupKey);
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

        [SerializeField]
        protected GameObjectGenericAction _pointerDownAction;

        private GameObjectGenericAction pointerDownAction => _pointerDownAction;

        [SerializeField]
        protected GameObjectGenericAction _pointerUpAction;

        private GameObjectGenericAction pointerUpAction => _pointerUpAction;

        [ShowInInspector]
        [SerializeField]
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
        
        private void Awake()
        {
            slider = GetComponent<Slider>();
        }

        private void OnEnable()
        {
            slider.maxValue = (float)activeMasterSequence.duration;
            slider.value = (float)activeMasterSequence.elapsedTime;
        }

        public void RefreshScrubber(ComplexPayload complexPayload)
        {
            navigationController = complexPayload.GetObjectValue(DataType.systemObjectType) as NavigationController;
            activeMasterSequence = navigationController.activeMasterSequence;
            
            slider.maxValue = (float)activeMasterSequence.duration;
            slider.value = (float)activeMasterSequence.elapsedTime;
        }

        public void RefreshScrubberUsingActiveMasterSequence()
        {
            if(isScrubbing == true) {
                return;
            }

            slider.value = (float)activeMasterSequence.elapsedTime;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isScrubbing = true;
            activeMasterSequence.SetElapsedTime(this.gameObject, slider.value);
            pointerDownAction.Invoke(this.gameObject);
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            isScrubbing = false;
            pointerUpAction.Invoke(this.gameObject);
        }

        public void ScrubSequence(float newValue)
        {
            if(isScrubbing == false) {
                return;
            }

            if (newValue > previousValue) {
                isReversing = false;
            }
            else {
                isReversing = true;
            }

            previousValue = newValue;
            activeMasterSequence.SetElapsedTime(this.gameObject, newValue);
            onScrub.RaiseEvent(this.gameObject);
        }
    }

}