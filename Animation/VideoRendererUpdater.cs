using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Animation
{
    [Serializable]
    [ExecuteInEditMode]
    [RequireComponent(typeof(PlayableVideoPlayerController))]
    public class VideoRendererUpdater : MonoBehaviour
    {
        [ReadOnly]
        [SerializeField]
        [InfoBox("By default, we should only change the _Color attribute on videos.")]
        string targetAttributeName = "_Color";

        [ValidateInput("IsPopulated")]
        public ColorReference _Color;

        [SerializeField]
        protected List<MeshRenderer> meshRenderers = new List<MeshRenderer>();

        [SerializeField]
        [ReadOnly]
        protected List<Material> materialInstances = new List<Material>();

        [SerializeField]
        [ReadOnly]
        PlayableVideoPlayerController playableVideoPlayerController;

        void Start()
        {
            playableVideoPlayerController = GetComponent<PlayableVideoPlayerController>();
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
            if (meshRenderers.Count < 1) {
                return;
            }
            if(_Color.GetVariable() == null) {
                Debug.Log("Please provide a color variable.", this);
                return;
            }

            if(playableVideoPlayerController.mainVideoInstance.isHidden == false) {
                UpdateVideoColor(0, _Color.GetValue());
            } else {
                UpdateVideoColor(0, Utils.transparent);
            }

#if UNITY_ANDROID
            if (playableVideoPlayerController.reverseVideoInstance.isHidden == false) {
                UpdateVideoColor(1, _Color.Value);
            } else {
                UpdateVideoColor(1, Utils.transparent);
            }
#endif

        }

        void UpdateVideoColor(int targetId, Color targetColor)
        {
            materialInstances[targetId].SetColor(targetAttributeName, targetColor);
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

    }
}