/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AltSalt.Maestro {

    public interface IResponsive
    {
        string elementName {
            get;
        }

        List<float> aspectRatioBreakpoints {
            get;
        }

        bool hasBreakpoints {
            get;
            set;
        }

        int priority {
            get;
        }

        Scene parentScene {
            get;
        }

        bool logElementOnLayoutUpdate {
            get;
        }

        void CallExecuteLayoutUpdate(UnityEngine.Object callerObject);

#if UNITY_EDITOR
        List<float> AddBreakpoint(float targetBreakpoint);
        string LogAddBreakpointMessage(float targetBreakpoint);
#endif

    }

}