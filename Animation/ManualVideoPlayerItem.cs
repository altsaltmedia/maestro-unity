using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Video;

namespace AltSalt.Maestro.Animation
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(VideoPlayer))]
    public class ManualVideoPlayerItem : MonoBehaviour
    {
        [SerializeField]
        [FormerlySerializedAs("initialTime")]
        private float _initialTime;

        private float initialTime => _initialTime;

        [SerializeField]
        private Material _videoMaterial;

        private Material videoMaterial => _videoMaterial;

        [ReadOnly]
        [SerializeField]
        [InfoBox("By default, we should only change the _Color attribute on videos.")]
        private string _targetAttributeName = "_Color";

        private string targetAttributeName => _targetAttributeName;
        
        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        private ColorReference _color;

        private Color color => _color.GetValue();

        private ColorVariable _colorVariableCache;

        private ColorVariable colorVariableCache
        {
            get => _colorVariableCache;
            set => _colorVariableCache = value;
        }

        private VideoPlayer _videoPlayer;

        private VideoPlayer videoPlayer
        {
            get
            {
                if (_videoPlayer == null) {
                    _videoPlayer = GetComponent<VideoPlayer>();
                }

                return _videoPlayer;
            }
            set => _videoPlayer = value;
        }

        public double videoLength => videoPlayer.length;

        public double localTime => videoPlayer.time;
        
        private MeshRenderer _meshRenderer;

        private MeshRenderer meshRenderer
        {
            get
            {
                if (_meshRenderer == null) {
                    _meshRenderer = GetComponent<MeshRenderer>();
                }

                return _meshRenderer;
            }
            set => _meshRenderer = value;
        }

        [ShowInInspector]
        public int sortingOrder => meshRenderer.sortingOrder;

        [ShowInInspector]
        [InfoBox("This value should be set dynamically at run time")]
        private bool _hide;

        public bool hide
        {
            get => _hide;
            set => _hide = value;
        }

        private SimpleSignalListener _colorListener;

        private SimpleSignalListener colorListener
        {
            get => _colorListener;
            set => _colorListener = value;
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (_color.GetVariable() != colorVariableCache) {
                RefreshColorListener();
            }
        }
#endif
        
        public void Init()
        {
            videoPlayer = GetComponent<VideoPlayer>();
            meshRenderer = GetComponent<MeshRenderer>();
            videoPlayer.sendFrameReadyEvents = true;
            videoPlayer.prepareCompleted += OnPrepareCompleted;
            videoPlayer.frameReady += OnFrameReady;
            videoPlayer.Prepare();
            
            meshRenderer.sharedMaterial = new Material(videoMaterial);
            CreateColorListener();
            RefreshVideoColor();
        }

        private SimpleSignalListener CreateColorListener()
        {
            if (_color.GetVariable() != null && colorListener == null) {
                ColorVariable colorVariable = _color.GetVariable() as ColorVariable;
                colorVariableCache = colorVariable;
                colorListener = new SimpleSignalListener(colorVariable, this.gameObject);
                colorListener.targetEventExecuted += RefreshVideoColor;
                return colorListener;
            }

            return null;
        }

        private void RefreshColorListener()
        {
            if (colorListener != null) {
                colorListener.targetEventExecuted -= RefreshVideoColor;
                colorListener.DestroyListener();
            }
            CreateColorListener();
        }

        private void OnDisable()
        {
            if (colorListener != null) {
                colorListener.targetEventExecuted -= RefreshVideoColor;
                colorListener.DestroyListener();
            }
        }

        public void PrepareVideo()
        {
            if (videoPlayer.isPrepared == false) {
                videoPlayer.Prepare();
            }
        }
        
        public void ForcePrepareVideo()
        {
            videoPlayer.Prepare();
        }

        private void OnPrepareCompleted(VideoPlayer param)
        {
            videoPlayer.StepForward();
            videoPlayer.time = initialTime;
        }

        public void SetTime(double targetTime)
        {
            videoPlayer.time = targetTime;

            if (Application.isPlaying == false) {
                CallRefreshVideoTexture();
            }
        }

        [Button(ButtonSizes.Large)]
        public void RefreshVideoColor()
        {
            if (_color.GetVariable() == null) {
                Debug.Log("Please provide a color variable.", this);
                return;
            }

            meshRenderer.sharedMaterial.SetColor(targetAttributeName, hide == false ? color : Utils.transparent);
        }

        [Button(ButtonSizes.Large)]
        public void CallRefreshVideoTexture()
        {
            OnFrameReady(videoPlayer, videoPlayer.frame);
        }
        
        [Button(ButtonSizes.Large)]
        public void RefreshMaterial()
        {
            meshRenderer.sharedMaterial = new Material(videoMaterial);
        }

        private void OnFrameReady(VideoPlayer param, long frameId)
        {
            meshRenderer.sharedMaterial.mainTexture = videoPlayer.texture;
        }

        private static bool IsPopulated(ColorReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }
}