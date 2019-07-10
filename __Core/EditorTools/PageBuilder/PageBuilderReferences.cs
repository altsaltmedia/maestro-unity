using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [CreateAssetMenu(menuName = "AltSalt/Page Builder Reference")]
    public class PageBuilderReferences : ScriptableObject
    {
        [Required]
        [SerializeField]
        public GameObject textPrefab;

        [Required]
        [SerializeField]
        public GameObject spritePrefab;

        [Required]
        [SerializeField]
        public GameObject responsiveContainerPrefab;

        [Required]
        [SerializeField]
        public GameObject sequenceTouchApplier;

        [Required]
        [SerializeField]
        public GameObject sequenceAutoplayer;

        [Required]
        [SerializeField]
        public GameObject standardDirector;

        [Required]
        [SerializeField]
        public GameObject swipeDirector;
    }
}