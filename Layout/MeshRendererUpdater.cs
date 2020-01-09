using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Layout
{

#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class MeshRendererUpdater : MonoBehaviour
    {
        [SerializeField]
        protected List<MeshRenderer> meshRenderers = new List<MeshRenderer>();

        [SerializeField]
        [ReadOnly]
        protected List<Material> materialInstances =  new List<Material>();

        [SerializeField]
        protected List<TargetMaterialAttribute> targetMaterialAttributes = new List<TargetMaterialAttribute>();

        protected void Start()
        {
            StoreRenderer();
            CreateMaterials();
        }

#if UNITY_EDITOR
        void OnGUI()
        {
            RefreshRenderer();
        }
#endif

        void Update()
        {
            RefreshRenderer();
        }

        void RefreshRenderer()
        {
            if(meshRenderers.Count < 1) {
                return;
            }
            for (int i = 0; i < targetMaterialAttributes.Count; i++) {
                if (targetMaterialAttributes[i].materialType == MaterialAttributeType.Color) {
                    for (int q = 0; q < materialInstances.Count; q++){
                        materialInstances[q].SetColor(targetMaterialAttributes[i].targetAttributeName, targetMaterialAttributes[i]._Color.value);
                    }
                }
                else if (targetMaterialAttributes[i].materialType == MaterialAttributeType.Float) {
                    for (int q = 0; q < materialInstances.Count; q++) {
                        materialInstances[q].SetFloat(targetMaterialAttributes[i].targetAttributeName, targetMaterialAttributes[i]._Float.value);
                    }
                }
            }
        }

        [Button(ButtonSizes.Large), GUIColor(0.8f, 0.6f, 1)]
        [InfoBox("Gets the current game object's mesh renderer, then creates and assigns a duplicate material.")]
        [SerializeField]
        protected void StoreRenderer()
        {
            if (GetComponent<MeshRenderer>() != null) {
                meshRenderers[0] = GetComponent<MeshRenderer>();
            }
        }

        [Button(ButtonSizes.Large), GUIColor(0.8f, 0.6f, 1)]
        [InfoBox("Creates and assigns duplicate materials based on assigned mesh renderers.")]
        [SerializeField]
        void CreateMaterials()
        {
            for (int q = 0; q < meshRenderers.Count; q++) {
                while (materialInstances.Count < meshRenderers.Count) {
                    materialInstances.Add(null);
                }
                materialInstances[q] = new Material(meshRenderers[q].sharedMaterial);
                meshRenderers[q].sharedMaterial = materialInstances[q];
            }
        }

        [Button(ButtonSizes.Large), GUIColor(0.8f, 0.6f, 1)]
        [InfoBox("Reset and remove materials.")]
        [SerializeField]
        void RemoveMaterials()
        {
            materialInstances.Clear();
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