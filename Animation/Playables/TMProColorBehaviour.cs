using System;
using UnityEngine;

namespace AltSalt.Maestro.Animation
{
    [Serializable]
    public class TMProColorBehaviour : ColorBehaviour
    {
        [SerializeField]
        private TextAnimationStyle _textAnimationStyle;

        public TextAnimationStyle textAnimationStyle
        {
            get => _textAnimationStyle;
            set => _textAnimationStyle = value;
        }
        
        [SerializeField]
        private float _blendValue;

        public float blendValue
        {
            get => _blendValue;
            set => _blendValue = value;
        }
    }
}