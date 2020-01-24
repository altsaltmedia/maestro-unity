using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Animation {

    #if UNITY_EDITOR
    [ExecuteInEditMode]
    #endif
    public class GradientShift : MonoBehaviour {

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        [FormerlySerializedAs("_Color")]
        private ColorReference _color = new ColorReference();

        private Color color => _color.GetValue();

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        [FormerlySerializedAs("_FadeColor")]
        private ColorReference _fadeColor = new ColorReference();

        private Color fadeColor => _fadeColor.GetValue();

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        [FormerlySerializedAs("_Exponent")]
        private FloatReference _exponent = new FloatReference();

        private float exponent => _exponent.GetValue();

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        [FormerlySerializedAs("_Subtract")]
        private FloatReference _subtract = new FloatReference();

        private float subtract => _subtract.GetValue();

        [SerializeField]
        [FormerlySerializedAs("sortingLayer")]
        private string _sortingLayer = "Default";

        private string sortingLayer => _sortingLayer;

        [InfoBox("The sorting order for meshes, unlike sprites, must be set via script")]
        [SerializeField]
        [FormerlySerializedAs("sortingOrder")]
        private int _sortingOrder;

        private int sortingOrder => _sortingOrder;

        private MeshRenderer _meshRenderer;

        private MeshRenderer meshRenderer
        {
            get => _meshRenderer;
            set => _meshRenderer = value;
        }

        private void Awake()
        {
            GetMeshRenderer();
        }

        private void GetMeshRenderer()
        {
            if(meshRenderer == null) {
                meshRenderer = GetComponent<MeshRenderer>();
            }
        }

        private void Start()
        {
            SetSortingOrder();
            GetMeshRenderer();
            RefreshRenderer();
        }

        private void OnGUI()
        {
            if(Application.isPlaying == false) {
                SetSortingOrder();
                //RefreshRenderer();
            }
        }

//        private void Update ()
//        {
//            RefreshRenderer();
//        }

        public void RefreshRenderer()
        {
            meshRenderer.sharedMaterial.SetColor("_Color", color);
            meshRenderer.sharedMaterial.SetColor("_FadeColor", fadeColor);
            meshRenderer.sharedMaterial.SetFloat("_Subtract", subtract);
            meshRenderer.sharedMaterial.SetFloat("_Exponent", exponent);
        }

        private void SetSortingOrder()
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