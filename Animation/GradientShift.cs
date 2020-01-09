using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Animation {

    #if UNITY_EDITOR
    [ExecuteInEditMode]
    #endif
    public class GradientShift : MonoBehaviour {

        [ValidateInput("IsPopulated")]
        public ColorReference _Color;

        [ValidateInput("IsPopulated")]
        public ColorReference _FadeColor;

        [ValidateInput("IsPopulated")]
        public FloatReference _Exponent;

        [ValidateInput("IsPopulated")]
        public FloatReference _Subtract;

        [SerializeField]
        string sortingLayer = "Default";

        [InfoBox("The sorting order for meshes, unlike sprites, must be set via script")]
        public int sortingOrder;

        MeshRenderer meshRenderer;

        void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        void Start()
        {
            SetSortingOrder();
            RefreshRenderer();
        }

        void OnGUI()
        {
            if(Application.isPlaying == false) {
                SetSortingOrder();
                RefreshRenderer();
            }
        }

        void Update ()
        {
            RefreshRenderer();
        }

        void RefreshRenderer()
        {
            if(meshRenderer == null) {
                meshRenderer = GetComponent<MeshRenderer>();
            }
            meshRenderer.sharedMaterial.SetColor("_Color", _Color.value);
            meshRenderer.sharedMaterial.SetColor("_FadeColor", _FadeColor.value);
            meshRenderer.sharedMaterial.SetFloat("_Subtract", _Subtract);
            meshRenderer.sharedMaterial.SetFloat("_Exponent", _Exponent);
        }

        void SetSortingOrder()
        {
            meshRenderer.sortingLayerName = sortingLayer;
            meshRenderer.sortingOrder = sortingOrder;
        }

        private static bool IsPopulated(ColorReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
            
        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

	}
}