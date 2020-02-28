using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Animation
{
    [Serializable]
    [ExecuteInEditMode]
    [RequireComponent(typeof(ForwardReverseVideoPlayer))]
    public class VideoRendererUpdater : MonoBehaviour
    {
        
//         [SerializeField]
//         protected List<MeshRenderer> _meshRenderers = new List<MeshRenderer>();
//
//         protected List<MeshRenderer> meshRenderers => _meshRenderers;
//
//         [SerializeField]
//         [ReadOnly]
//         protected List<Material> _materialInstances = new List<Material>();
//
//         private List<Material> materialInstances => _materialInstances;
//
//         [FormerlySerializedAs("_playableVideoPlayerController"),SerializeField]
//         [ReadOnly]
//         private ManualVideoPlayer _manualVideoPlayer;
//
//         private ManualVideoPlayer manualVideoPlayer
//         {
//             get => _manualVideoPlayer;
//             set => _manualVideoPlayer = value;
//         }
//
//         private void Awake()
//         {
//             manualVideoPlayer = GetComponent<ManualVideoPlayer>();
//             CreateMaterials();
//         }
//
// #if UNITY_EDITOR
//         private void OnGUI()
//         {
//             RefreshRenderer();
//         }
// #endif
//
//         private void Update()
//         {
//             RefreshRenderer();
//         }
//
//         private void RefreshRenderer()
//         {
//             if (meshRenderers.Count < 1) {
//                 return;
//             }
//             if(_color.GetVariable() == null) {
//                 Debug.Log("Please provide a color variable.", this);
//                 return;
//             }
//
//             if(manualVideoPlayer.forwardVideoInstance.isHidden == false) {
//                 UpdateVideoColor(0, _color.GetValue());
//             } else {
//                 UpdateVideoColor(0, Utils.transparent);
//             }
//
// #if UNITY_ANDROID
//             if (manualVideoPlayer.reverseVideoInstance.isHidden == false) {
//                 UpdateVideoColor(1, color);
//             } else {
//                 UpdateVideoColor(1, Utils.transparent);
//             }
// #endif
//
//         }
//
//         private void UpdateVideoColor(int targetId, Color targetColor)
//         {
//             materialInstances[targetId].SetColor(targetAttributeName, targetColor);
//         }
//
//         [Button(ButtonSizes.Large), GUIColor(0.8f, 0.6f, 1)]
//         [InfoBox("Creates and assigns duplicate materials based on assigned mesh renderers.")]
//         [SerializeField]
//         private void CreateMaterials()
//         {
//             for (int q = 0; q < meshRenderers.Count; q++) {
//                 while (materialInstances.Count < meshRenderers.Count) {
//                     materialInstances.Add(null);
//                 }
//                 materialInstances[q] = new Material(meshRenderers[q].sharedMaterial);
//                 meshRenderers[q].sharedMaterial = materialInstances[q];
//             }
//         }
//
//         [Button(ButtonSizes.Large), GUIColor(0.8f, 0.6f, 1)]
//         [InfoBox("Reset and remove materials.")]
//         [SerializeField]
//         private void RemoveMaterials()
//         {
//             materialInstances.Clear();
//         }
//
//         private static bool IsPopulated(ColorReference attribute)
//         {
//             return Utils.IsPopulated(attribute);
//         }

    }
}