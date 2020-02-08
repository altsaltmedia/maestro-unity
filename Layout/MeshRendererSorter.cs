using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AltSalt.Maestro.Layout
{
    #if UNITY_EDITOR
    [ExecuteInEditMode]
    #endif
    public class MeshRendererSorter : MonoBehaviour
    {
        
        [InfoBox("The sorting order for meshes, unlike sprites, must be set via script")]
        [OnValueChanged(nameof(SetSortingOrder))]
        public int sortingOrder;

        [OnValueChanged(nameof(SetSortingOrder))]
        public string sortingLayer;
        
        MeshRenderer meshRenderer;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Start()
        {
            SetSortingOrder();
        }

        private void SetSortingOrder()
        {
            meshRenderer.sortingOrder = sortingOrder;
            if(sortingLayer.Length > 0) {
                meshRenderer.sortingLayerName = sortingLayer;
            }
        }

        
    }
}