/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

namespace AltSalt
{
    public interface IResponsiveSaveable
    {
#if UNITY_EDITOR
        void SaveValue();
#endif
        void SetValue(int activeIndex);
    }
}