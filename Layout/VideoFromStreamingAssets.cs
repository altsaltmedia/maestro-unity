using UnityEngine;
using UnityEngine.Video;
using System.IO;

public class VideoFromStreamingAssets : MonoBehaviour
{
    private VideoPlayer _videoPlayer;

    private VideoPlayer videoPlayer
    {
        get => _videoPlayer;
        set => _videoPlayer = value;
    }

    [SerializeField]
    private string _videoFileName = "";

    public string videoFileName
    {
        get => _videoFileName;
        set => _videoFileName = value;
    }

    public void SetVideoUrl()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        // Set the path for WebGL build
        if (Application.platform == RuntimePlatform.WebGLPlayer && videoFileName != "")
        {
            videoPlayer.source = VideoSource.Url;
            string path = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
            videoPlayer.url = path;
        }
        
    }
}