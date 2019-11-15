using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace AltSalt.Maestro.Animation
{
    public class PostProcessing : MonoBehaviour
    {

        [ValidateInput("IsPopulated")]
        public BoolReference isFlicked;

        [ValidateInput("IsPopulated")]
        public BoolReference isReversing;

        [ValidateInput("IsPopulated")]
        public FloatReference currentTimescale;

        [Required]
        public SimpleEventTrigger timescaleChanged;

        [RangeAttribute(0, 5)]
        public float duration;

        [RangeAttribute(0, 1)]
        public float[] strength = { .15f, .2f, .3f, .4f} ;

        public float[] timescales = { 1f, 1.5f, 2f, 2.5f};
        int timescaleIndex = 5;

        bool previousSwipeNegative = false;

        public void UpdateTimescale()
        {
            if(isFlicked.Value == false) {
                return;
            }

            if(isReversing.Value == previousSwipeNegative) {
                timescaleIndex++;
            } else {
                timescaleIndex = 1;
            }

            if(timescaleIndex >= timescales.Length) {
                timescaleIndex = timescales.Length - 1;
            }

            currentTimescale.Variable.SetValue(isReversing.Value == false ? timescales[timescaleIndex] : timescales[timescaleIndex] * -1);

            Time.timeScale = Mathf.Abs(currentTimescale.Value);
            timescaleChanged.RaiseEvent(this.gameObject);
            ExecuteEffect();

            previousSwipeNegative = isReversing.Value;
        }

        public void ResetTimescale()
        {
            timescaleIndex = 0;
            currentTimescale.Variable.SetValue(timescales[timescaleIndex]);
            Time.timeScale = currentTimescale.Value;
        }

        public void ExecuteEffect ()
        {
            //CameraPlay.Radial(0.5f, 0.5f, duration, strength[timescaleIndex]);
        }

        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}