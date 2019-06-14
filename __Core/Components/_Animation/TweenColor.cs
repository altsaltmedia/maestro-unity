using UnityEngine;
using DG.Tweening;
using TMPro;

namespace AltSalt
{
    public class TweenColor : TweenController
    {
        void Start()
        {
            DOTween.defaultTimeScaleIndependent = true;
        }

        public override void TweenValue (TweenTarget tweenTarget)
        {
            if(tweenTarget.objectType == ObjectType.Sprite) {
                TweenSpriteColor(tweenTarget);
            } else {
                TweenTMProColor(tweenTarget);
            }
        }

        void TweenSpriteColor(TweenTarget spriteTarget)
        {
            DOTween.To(() => spriteTarget.targetObject.GetComponent<SpriteRenderer>().color, x => spriteTarget.targetObject.GetComponent<SpriteRenderer>().color = x, spriteTarget.targetColor, duration);
            DOTween.To(() => spriteTarget.targetObject.GetComponent<SpriteRenderer>().color, x => spriteTarget.targetObject.GetComponent<SpriteRenderer>().color = x, spriteTarget.originalColor, duration).SetDelay(1);
        }

        void TweenTMProColor(TweenTarget textTarget)
        {
            DOTween.To(() => textTarget.targetObject.GetComponent<TextMeshPro>().color, x => textTarget.targetObject.GetComponent<TextMeshPro>().color = x, textTarget.targetColor, duration);
            DOTween.To(() => textTarget.targetObject.GetComponent<TextMeshPro>().color, x => textTarget.targetObject.GetComponent<TextMeshPro>().color = x, textTarget.originalColor, duration).SetDelay(1);
        }

    }

}