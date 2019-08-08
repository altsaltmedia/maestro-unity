/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AltSalt {

    public interface IResponsive
    {
        string Name {
            get;
        }

        List<float> AspectRatioBreakpoints {
            get;
        }

        bool HasBreakpoints {
            get;
            set;
        }

        int Priority {
            get;
        }

        Scene ParentScene {
            get;
        }

        List<float> AddBreakpoint(float targetBreakpoint);

        bool LogElementOnLayoutUpdate {
            get;
        }

        void CallExecuteLayoutUpdate(UnityEngine.Object callerObject);

        string LogAddBreakpointMessage(float targetBreakpoint);
    }

}