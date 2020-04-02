using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Logic
{
    public class CompareTransformPosition : MonoBehaviour
    {
        [SerializeField]
        private bool _active = true;

        private bool active => _active;

        [Required]
        [SerializeField]
        private Transform _sourceTransform;

        private Transform sourceTransform => _sourceTransform;
        
        [Required]
        [SerializeField]
        private Transform _targetTransform;

        private Transform targetTransform => _targetTransform;

        private enum ComparisonType { X, Y }

        [SerializeField]
        private ComparisonType _comparisonType;

        private ComparisonType comparisonType => _comparisonType;

        [SerializeField]
        private FloatReference _outputVariable;

        private FloatReference outputVariable => _outputVariable;

        private Vector3 _previousPosition;

        private Vector3 previousPosition
        {
            get => _previousPosition;
            set => _previousPosition = value;
        }

        private void Update()
        {
            if (active && previousPosition.Equals(sourceTransform.position) == false) {

                if (comparisonType == ComparisonType.Y) {
                    float percentage = sourceTransform.position.y / targetTransform.position.y;
                    outputVariable.SetValue(this.gameObject, percentage);
                }
            }

            previousPosition = sourceTransform.position;
        }
    }
}