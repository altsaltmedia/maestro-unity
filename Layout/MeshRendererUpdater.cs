using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Layout
{
    
    [ExecuteInEditMode]
    public class MeshRendererUpdater : MonoBehaviour
    {
        [SerializeField]
        [FormerlySerializedAs("meshRenderers")]
        protected List<MeshRenderer> _meshRenderers = new List<MeshRenderer>();

        private List<MeshRenderer> meshRenderers => _meshRenderers;

        [SerializeField]
        [ReadOnly]
        [FormerlySerializedAs("materialInstances")]
        protected List<Material> _materialInstances =  new List<Material>();

        private List<Material> materialInstances => _materialInstances;

        [SerializeField]
        [FormerlySerializedAs("targetMaterialAttributes")]
        protected List<TargetMaterialAttribute> _targetMaterialAttributes = new List<TargetMaterialAttribute>();

        private List<TargetMaterialAttribute> targetMaterialAttributes => _targetMaterialAttributes;

        protected void Start()
        {
            StoreRenderer();
            CreateMaterials();
        }

#if UNITY_EDITOR
        private void OnGUI()
        {
            RefreshRenderer();
        }
#endif

        private void Update()
        {
            RefreshRenderer();
        }

        private void RefreshRenderer()
        {
            if(meshRenderers.Count < 1) {
                return;
            }
            for (int i = 0; i < targetMaterialAttributes.Count; i++) {
                if (targetMaterialAttributes[i].materialType == MaterialAttributeType.Color) {
                    for (int q = 0; q < materialInstances.Count; q++){
                        materialInstances[q].SetColor(targetMaterialAttributes[i].targetAttributeName, targetMaterialAttributes[i].colorValue.GetValue());
                    }
                }
                else if (targetMaterialAttributes[i].materialType == MaterialAttributeType.Float) {
                    for (int q = 0; q < materialInstances.Count; q++) {
                        materialInstances[q].SetFloat(targetMaterialAttributes[i].targetAttributeName, targetMaterialAttributes[i].floatValue.GetValue());
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
    }
}