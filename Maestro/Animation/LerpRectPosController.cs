using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace AltSalt.Maestro
{
    [ExecuteInEditMode]
    public class LerpRectPosController : MonoBehaviour
    {
        [SerializeField]
        Vector3 initialPosition;

        [SerializeField]
        Vector3 targetPosition;

        RectTransform rectTransform;
        TrailRenderer trailRenderer;

        void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            trailRenderer = GetComponent<TrailRenderer>();
            trailRenderer.emitting = false;
        }

        public void CallPlayAnimation()
        {
            StartCoroutine(PlayAnimation());
        }

        IEnumerator PlayAnimation()
        {
            rectTransform.localPosition = initialPosition;
            yield return new WaitForSeconds(.02f);
            trailRenderer.emitting = true;
            DOTween.To(() => rectTransform.localPosition, x => rectTransform.localPosition = x, targetPosition, 1).SetEase(Ease.Linear).OnComplete(CallResetAnimation);
        }

        // Update is called once per frame
        public void CallPlayAnimationBackwards()
        {
            StartCoroutine(PlayAnimationBackwards());
        }

        IEnumerator PlayAnimationBackwards()
        {
            rectTransform.localPosition = targetPosition;
            yield return new WaitForSeconds(.02f);
            trailRenderer.emitting = true;
            DOTween.To(() => rectTransform.localPosition, x => rectTransform.localPosition = x, initialPosition, 1).SetEase(Ease.Linear).OnComplete(CallResetAnimation);
        }

        void CallResetAnimation()
        {
            StartCoroutine(ResetAnimation());
        }

        IEnumerator ResetAnimation()
        {
            yield return new WaitForSeconds(.9f);
            trailRenderer.Clear();
            trailRenderer.emitting = false;
        }
    }
}