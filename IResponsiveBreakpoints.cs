﻿/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AltSalt.Maestro {

    public interface IResponsiveBreakpoints
    {
        string elementName {
            get;
        }
        
        float sceneWidth { get; set; }

        float sceneHeight { get; set; }

        float sceneAspectRatio { get; set; }

        List<float> aspectRatioBreakpoints {
            get;
        }

        bool hasBreakpoints {
            get;
            set;
        }

#if UNITY_EDITOR
        List<float> AddBreakpoint(float targetBreakpoint);
        string LogAddBreakpointMessage(float targetBreakpoint);
#endif

    }

}