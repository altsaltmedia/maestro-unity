using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{

    public enum MaterialAttributeType { Color, Float }

    [Serializable]
    public class TargetMaterialAttribute
    {
        public string targetAttributeName;

        [ValueDropdown("materialTypeValues")]
        public MaterialAttributeType materialType;

        [ValidateInput("IsPopulated")]
        ValueDropdownList<MaterialAttributeType> materialTypeValues = new ValueDropdownList<MaterialAttributeType>(){
            {"Color", MaterialAttributeType.Color },
            {"Float", MaterialAttributeType.Float }
        };

        [EnableIf("materialType", MaterialAttributeType.Color)]
        [ValidateInput("IsPopulated")]
        public ColorReference _Color;

        [EnableIf("materialType", MaterialAttributeType.Float)]
        [ValidateInput("IsPopulated")]
        public FloatReference _Float;

        private bool IsPopulated(ColorReference attribute) {
            if(materialType == MaterialAttributeType.Color) {
                if (attribute.UseConstant == true) {
                    return true;
                }
                else {
                    return attribute.Variable == null ? false : true;
                }
            } else {
                return true;
            }
        }

        private bool IsPopulated(FloatReference attribute)
        {
            if (materialType == MaterialAttributeType.Float) {
                if (attribute.UseConstant == true) {
                    return true;
                }
                else {
                    return attribute.Variable == null ? false : true;
                }
            }
            else {
                return true;
            }
        }

    }

#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class MeshRendererUpdater : MonoBehaviour
    {
        MeshRenderer meshRenderer;

        Material materialInstance;

        [SerializeField]
        List<TargetMaterialAttribute> targetMaterialAttributes = new List<TargetMaterialAttribute>();

        void Awake()
        {
            RefreshRenderer();
        }

        void Start()
        {
            RefreshRenderer();
        }

        void OnGUI()
        {
            RefreshRenderer();
        }

        void Update()
        {
            RefreshRenderer();
        }

        void RefreshRenderer()
        {
            StoreRenderer();
            for (int i = 0; i < targetMaterialAttributes.Count; i++) {
                if (targetMaterialAttributes[i].materialType == MaterialAttributeType.Color) {
                    materialInstance.SetColor(targetMaterialAttributes[i].targetAttributeName, targetMaterialAttributes[i]._Color.Value);
                }
                else if (targetMaterialAttributes[i].materialType == MaterialAttributeType.Float) {
                    materialInstance.SetFloat(targetMaterialAttributes[i].targetAttributeName, targetMaterialAttributes[i]._Float.Value);
                }
            }
        }

        void StoreRenderer()
        {
            if (meshRenderer == null) {
                meshRenderer = GetComponent<MeshRenderer>();
            }

            if (materialInstance == null) {
                materialInstance = new Material(meshRenderer.sharedMaterial);
                meshRenderer.sharedMaterial = materialInstance;
            }
        }

        void Reset()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            materialInstance = new Material(meshRenderer.sharedMaterial);
            meshRenderer.sharedMaterial = materialInstance;
        }

        private static bool IsPopulated(ColorReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

        private static bool IsPopulated(List<TargetMaterialAttribute> attribute) {
            return Utils.IsPopulated(attribute);
        }
}
}