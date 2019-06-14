using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FluffyUnderware.Curvy.Controllers;
using DG.Tweening;

namespace AltSalt
{
    [ExecuteInEditMode]
    public class LerpCurvyController : MonoBehaviour
    {
        public GameObject splineObject;
        SplineController splineController;
        TrailRenderer trailRenderer;

        void Start()
        {
            splineController = splineObject.GetComponent<SplineController>();
            trailRenderer = splineObject.GetComponent<TrailRenderer>();
        }

        public void CallPlayAnimation()
        {
            StartCoroutine(PlayAnimation());
        }

        IEnumerator PlayAnimation()
        {
            splineController.Position = 0;
            yield return new WaitForSeconds(.02f);
            trailRenderer.emitting = true;
            DOTween.To(() => splineController.Position, x => splineController.Position = x, 1, 1).SetEase(Ease.Linear).OnComplete(CallResetAnimation);
        }

        // Update is called once per frame
        public void CallPlayAnimationBackwards()
        {
            StartCoroutine(PlayAnimationBackwards());
        }

        IEnumerator PlayAnimationBackwards()
        {
            splineController.Position = 1;
            yield return new WaitForSeconds(.02f);
            trailRenderer.emitting = true;
            DOTween.To(() => splineController.Position, x => splineController.Position = x, 0, 1).SetEase(Ease.Linear).OnComplete(CallResetAnimation);
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