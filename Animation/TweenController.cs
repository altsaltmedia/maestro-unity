using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Animation {

    public enum ObjectType { Sprite, TextMeshPro }

    [System.Serializable]
    public class TweenTarget
    {
        public GameObject targetObject;
        public ObjectType objectType;
        public Color originalColor;
        public Color targetColor;

        private ValueDropdownList<ObjectType> objectTypeValues = new ValueDropdownList<ObjectType>(){
            {"Sprite", ObjectType.Sprite },
            {"Text Mesh Pro", ObjectType.TextMeshPro }
        };
    }

    public class TweenController : MonoBehaviour
    {
        [SerializeField]
        protected List<TweenTarget> tweenTargets = new List<TweenTarget>();

        [SerializeField]
        protected float duration;

        public void TriggerTweens()
        {
            for (int i = 0; i < tweenTargets.Count; i++) {
                TweenValue(tweenTargets[i]);
            }
        }

        public virtual void TweenValue(TweenTarget tweenTarget) {}

    }
    
}