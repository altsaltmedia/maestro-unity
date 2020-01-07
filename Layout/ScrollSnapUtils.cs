using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt.Maestro.Layout
{
    [ExecuteInEditMode]
    public class ScrollSnapUtils : MonoBehaviour
    {
        [SerializeField]
        Touchable prevBtn;
        public Touchable PrevBtn {
            get {
                return prevBtn;
            }
        }

        [SerializeField]
        Touchable nextBtn;
        public Touchable NextBtn {
            get {
                return nextBtn;
            }
        }

        [SerializeField]
        Touchable closeBtn;
        public Touchable CloseBtn {
            get {
                return closeBtn;
            }
        }

        [SerializeField]
        RectTransform iconContainer;
        public RectTransform IconContainer {
            get {
                return iconContainer;
            }
        }
    }
}

