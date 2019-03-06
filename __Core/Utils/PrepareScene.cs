using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt {
    
	public class PrepareScene : MonoBehaviour {
		
		public bool defaultX;
		public bool defaultY;
		public bool defaultZ;

        public bool invertYAxis;
        public bool invertXAxis;
		
        public Axis xSwipeAxis;
        public Axis ySwipeAxis;
        public Axis zSwipeAxis;

        public Axis xMomentumAxis;
        public Axis yMomentumAxis;
        public Axis zMomentumAxis;
		
        public BoolReference _invertYAxis;
        public BoolReference _invertXAxis;

        [ValueDropdown("orientationValues")]
        [SerializeField]
        DimensionType orientation;

        [SerializeField]
        bool resetSequences;

        [ShowIf("resetSequences")]
        [SerializeField]
        SequenceList sequenceList;

        [SerializeField]
        bool bufferScene;

        [ShowIf("bufferScene")]
        [ShowInInspector, ReadOnly]
        int sequencesBuffered = 0;

        [SerializeField]
        [ShowIf("bufferScene")]
        int totalSequenceCount = 1;

        [SerializeField]
        [ShowIf("bufferScene")]
        [Required]
        SimpleEvent bufferSceneTriggered;

        // Not currently in use, but will need this for bookmarking
        [SerializeField]
        [ShowIf("bufferScene")]
        [Required]
        SimpleEvent bufferSceneCompleted;

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

        // Use this for initialization
        void Start () {
            xSwipeAxis.Active = defaultX;
            ySwipeAxis.Active = defaultY;
            zSwipeAxis.Active = defaultZ;

            xMomentumAxis.Active = defaultX;
            yMomentumAxis.Active = defaultY;
            zMomentumAxis.Active = defaultZ;

            _invertYAxis.Variable.Value = invertYAxis;
            _invertXAxis.Variable.Value = invertXAxis;

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

            if (bufferScene == true) {
                bufferSceneTriggered.Raise();
            } else {
                prepareSceneCompleted.Raise();
            }
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
		
        public void BufferSequenceCallback()
        {
            sequencesBuffered++;
            if(sequencesBuffered > totalSequenceCount) {
                Debug.LogWarning("More sequences buffered than total count specified. Check " + this.name + ".", this);
            }
            if(sequencesBuffered == totalSequenceCount) {
                bufferSceneCompleted.Raise(); // Not currently in use, but will use this for bookmarking
                prepareSceneCompleted.Raise();
            }
        }

        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}
