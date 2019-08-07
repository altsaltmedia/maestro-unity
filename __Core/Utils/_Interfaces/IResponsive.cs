/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/
using System.Collections.Generic;

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

        List<float> AddBreakpoint(float targetBreakpoint);

        string LogAddBreakpointMessage(float targetBreakpoint);
    }

}