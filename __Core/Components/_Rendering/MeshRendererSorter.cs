using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AltSalt
{
    #if UNITY_EDITOR
    [ExecuteInEditMode]
    #endif
    public class MeshRendererSorter : MonoBehaviour
    {
        
        [InfoBox("The sorting order for meshes, unlike sprites, must be set via script")]
        public int sortingOrder;

        public string sortingLayer;
        
        MeshRenderer meshRenderer;
        
        void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
        
        void Start()
        {
            SetSortingOrder();
        }

#if UNITY_EDITOR
        void OnGUI()
        {
            SetSortingOrder();
        }
#endif

        public void SetSortingOrder()
        {
            meshRenderer.sortingOrder = sortingOrder;
            if(sortingLayer.Length > 0) {
                meshRenderer.sortingLayerName = sortingLayer;
            }
        }

        
    }
}