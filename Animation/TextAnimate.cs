using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace AltSalt.Maestro.Animation
{
    public class TextAnimate : MonoBehaviour
    {
        TextMeshPro textMeshPro;

        [SerializeField]
        Color initialColor;

        [SerializeField]
        Color targetColor;

        [SerializeField]
        float duration;

        void Start ()
        {
            textMeshPro = GetComponent<TextMeshPro>();
            textMeshPro.DOColor(initialColor, duration);
        }

        public void PlayAnimation()
        {
            textMeshPro.DOColor(targetColor, duration);
        }
    }
}