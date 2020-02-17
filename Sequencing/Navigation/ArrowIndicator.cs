using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Sequencing.Navigation
{
    public class ArrowIndicator : NavigationModule
    {
        [ReadOnly]
        [ShowInInspector]
        private NavigationController _navigationController;

        protected override NavigationController navigationController
        {
            get => _navigationController;
            set => _navigationController = value;
        }
        
        [SerializeField]
        private GameObject _fourWayArrow;

        private GameObject fourWayArrow => _fourWayArrow;

        [SerializeField]
        private GameObject _verticalArrow;

        private GameObject verticalArrow => _verticalArrow;

        [SerializeField]
        private GameObject _horizontalArrow;

        private GameObject horizontalArrow => _horizontalArrow;

        private Axis ySwipeAxis =>
            navigationController.appSettings.GetYSwipeAxisReference(this, inputGroupKey).GetVariable() as Axis;
        
        private Axis xSwipeAxis =>
            navigationController.appSettings.GetXSwipeAxisReference(this, inputGroupKey).GetVariable() as Axis;
        
        public void RefreshArrowIndicator(ComplexPayload complexPayload)
        {
            navigationController = complexPayload.GetObjectValue(DataType.systemObjectType) as NavigationController;
            UpdateArrow();
        }

        public void UpdateArrow()
        {
            if (navigationController == null) {
                return;
            }
            
            if (ySwipeAxis.active == true && xSwipeAxis.active == true) {
                
                fourWayArrow.SetActive(true);
                
                verticalArrow.SetActive(false);
                horizontalArrow.SetActive(false);
            }

            else if (ySwipeAxis.active == true) {
                
                verticalArrow.SetActive(true);
                
                fourWayArrow.SetActive(false);
                horizontalArrow.SetActive(false);
            }

            else {
                
                horizontalArrow.SetActive(true);
                
                fourWayArrow.SetActive(false);
                verticalArrow.SetActive(false);
            }
        }
        
    }
}