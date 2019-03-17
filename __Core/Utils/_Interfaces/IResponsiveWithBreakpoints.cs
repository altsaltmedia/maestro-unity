/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

namespace AltSalt {

    public interface IResponsiveWithBreakpoints
    {
#if UNITY_EDITOR
        void Reset();
        void LogBreakpointMessage();
#endif
    }
}