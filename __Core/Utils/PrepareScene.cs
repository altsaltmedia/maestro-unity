using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt {
    
	public class PrepareScene : MonoBehaviour {

        [SerializeField]
        bool defaultX;
        [SerializeField]
        bool defaultY;
        [SerializeField]
        bool defaultZ;

        [SerializeField]
        bool invertYAxis;
        [SerializeField]
        bool invertXAxis;

        [SerializeField]
        Axis xSwipeAxis;
        [SerializeField]
        Axis ySwipeAxis;
        [SerializeField]
        Axis zSwipeAxis;

        [SerializeField]
        Axis xMomentumAxis;
        [SerializeField]
        Axis yMomentumAxis;
        [SerializeField]
        Axis zMomentumAxis;

        [SerializeField]
        [ValidateInput("IsPopulated")]
        BoolReference _invertYAxis;

        [SerializeField]
        [ValidateInput("IsPopulated")]
        BoolReference _invertXAxis;

        [SerializeField]
        [ValidateInput("IsPopulated")]
        BoolReference isReversing;

        [ValueDropdown("orientationValues")]
        [SerializeField]
        DimensionType orientation;

        [SerializeField]
        bool resetSequences;

        [ShowIf("resetSequences")]
        [SerializeField]
        SequenceList sequenceList;

        [SerializeField]
        bool delayStart;

        [SerializeField]
        [ShowIf("delayStart")]
        float delayAmount;

        [SerializeField]
        [Required]
        SimpleEvent prepareSceneCompleted;

        [SerializeField]
        [ValidateInput("IsPopulated")]
        BoolReference removeOverlayImmediately;

        [SerializeField]
        [Required]
        SimpleEvent fadeInTriggered;

        private ValueDropdownList<DimensionType> orientationValues = new ValueDropdownList<DimensionType>(){
            {"Vertical", DimensionType.Vertical },
            {"Horizontal", DimensionType.Horizontal }
        };

        void Start () {
            ResetScene();
		}

        [HorizontalGroup("Split", 1f)]
        [InfoBox("Reset scene variables")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void ResetScene()
        {
            xSwipeAxis.Active = defaultX;
            ySwipeAxis.Active = defaultY;
            zSwipeAxis.Active = defaultZ;

            xMomentumAxis.Active = defaultX;
            yMomentumAxis.Active = defaultY;
            zMomentumAxis.Active = defaultZ;

            _invertYAxis.Variable.Value = invertYAxis;
            _invertXAxis.Variable.Value = invertXAxis;

            isReversing.Variable.Value = false;

            Time.timeScale = 1.0f;

            if(orientation == DimensionType.Vertical) {
                Screen.orientation = ScreenOrientation.Portrait;
            } else {
                Screen.orientation = ScreenOrientation.LandscapeLeft;
            }

            if(resetSequences == true) {
                TriggerResetSequences();
            }

            if (removeOverlayImmediately.Value == true) {
                fadeInTriggered.Raise();
            }

            if(delayStart == false) {
                prepareSceneCompleted.Raise();
            } else {
                StartCoroutine(PrepareSceneCompleteTimedDelay());
            }
        }

        IEnumerator PrepareSceneCompleteTimedDelay()
        {
            yield return new WaitForSeconds(delayAmount);
            prepareSceneCompleted.Raise();
        }

        void TriggerResetSequences()
        {
            if (sequenceList == null) {
                Debug.LogWarning("No sequence list found on " + this.name + ", please check.", this);
                return;
            }

            for (int i = 0; i < sequenceList.sequences.Count; i++) {
                sequenceList.sequences[i].currentTime = 0;
            }
        }

        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}
