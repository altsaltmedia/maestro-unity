using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Layout
{
    [ExecuteInEditMode]
    public class ScrollSnapUtils : MonoBehaviour
    {
        [FormerlySerializedAs("prevBtn"),SerializeField]
        private Touchable _prevBtn;
        public Touchable prevBtn => _prevBtn;

        [FormerlySerializedAs("nextBtn"),SerializeField]
        private Touchable _nextBtn;
        public Touchable nextBtn => _nextBtn;

        [FormerlySerializedAs("closeBtn"),SerializeField]
        private Touchable _closeBtn;
        public Touchable closeBtn => _closeBtn;

        [FormerlySerializedAs("iconContainer"),SerializeField]
        private RectTransform _iconContainer;
        public RectTransform iconContainer => _iconContainer;
    }
}

