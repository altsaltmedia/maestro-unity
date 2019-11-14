using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro
{
    [CreateAssetMenu(menuName = "AltSalt/Module References")]
    public class ModuleReferences : ScriptableObject
    {
        [Required]
        [SerializeField]
        public GameObject textPrefab;

        [Required]
        [SerializeField]
        public GameObject spritePrefab;

        [Required]
        [SerializeField]
        public GameObject containerPrefab;

        [Required]
        [SerializeField]
        public GameObject responsiveContainerPrefab;

        [Required]
        [SerializeField]
        public GameObject responsiveContainerLeftAnchorPrefab;

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

        [Required]
        [SerializeField]
        public GameObject scrollSnapController;

        [Required]
        [SerializeField]
        public GameObject scrollSnapIcon;
    }
}