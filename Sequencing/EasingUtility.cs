using UnityEngine;

namespace AltSalt.Sequencing
{
    [System.Serializable]
    public class EasingUtility
    {
        enum LerpType { EaseIn, EaseOut }

        [SerializeField]
        private LerpType _lerpType = LerpType.EaseOut;

        private LerpType lerpType
        {
            get => _lerpType;
            set => _lerpType = value;
        }

        [SerializeField]
        private EasingFunction.Ease ease = EasingFunction.Ease.EaseOutQuad;

        private EasingFunction.Function _easingFunction;

        private EasingFunction.Function easingFunction
        {
            get => _easingFunction;
            set => _easingFunction = value;
        }
        
        [SerializeField]
        private float _lerpInterval;

        private float lerpInterval
        {
            get => _lerpInterval;
        }
        
        private float _multiplier;

        private float multiplier
        {
            get => _multiplier;
            set => _multiplier = value;
        }
        
        private float _percentValue;

        private float percentValue
        {
            get => _percentValue;
            set => _percentValue = value;
        }
        
        public float GetMultiplier()
        {
            if (lerpType == LerpType.EaseOut)
            {
                multiplier = easingFunction(1f, 0f, percentValue);
            }
            else
            {
                multiplier = easingFunction(0f, 1f, percentValue);
            }
            
            percentValue += lerpInterval;
            return multiplier;
        }

        public EasingUtility Reset()
        {
            percentValue = 0f;
            return this;
        }
    }
}