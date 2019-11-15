/***********************************************

Copyright © 2018 AltSalt Media, LLC.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/
using System;

namespace AltSalt.Maestro.Animation
{
    [Serializable]
    public class TMProTypewriterBehaviour : LerpToTargetBehaviour
    {
        public bool setValuesImmediately = false;

        public int initialMaxVisibleCharacters = 0;
        public int targetMaxVisibleCharacters = 0;

        public int initialMaxVisibleWords = 0;
        public int targetMaxVisibleWords = 0;
    }   
}