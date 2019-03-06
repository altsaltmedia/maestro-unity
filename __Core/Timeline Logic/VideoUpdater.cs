/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Video;

namespace AltSalt
{
    
    public class VideoUpdater : MonoBehaviour
    {
        
        [Required]
        public Sequence activeClip;
        [Required]
        public AppSettings appSettings;
        [Required]
        public SimpleEvent BoundaryReached;

        VideoPlayer video;

        void Start()
        {
            video = GetComponent<VideoPlayer>();
            video.Prepare();
        }


        // Update is called once per frame
        public void RefreshClip()
        {
            if (appSettings.paused) {
                return;
            }

            video.time = activeClip.currentTime;
        }
        
    }
}