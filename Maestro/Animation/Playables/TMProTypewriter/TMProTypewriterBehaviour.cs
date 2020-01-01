/***********************************************

Copyright © 2018 AltSalt Media, LLC.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Animation
{
    [Serializable]
    public class TMProTypewriterBehaviour : LerpToTargetBehaviour
    {
        [FormerlySerializedAs("setValuesImmediately")]
        [SerializeField]
        private bool _setValuesImmediately = false;

        public bool setValuesImmediately
        {
            get => _setValuesImmediately;
            set => _setValuesImmediately = value;
        }

        [FormerlySerializedAs("initialMaxVisibleCharacters")]
        [SerializeField]
        private int _initialMaxVisibleCharacters = 0;

        public int initialMaxVisibleCharacters
        {
            get => _initialMaxVisibleCharacters;
            set => _initialMaxVisibleCharacters = value;
        }

        [FormerlySerializedAs("targetMaxVisibleCharacters")]
        [SerializeField]
        private int _targetMaxVisibleCharacters = 0;

        public int targetMaxVisibleCharacters
        {
            get => _targetMaxVisibleCharacters;
            set => _targetMaxVisibleCharacters = value;
        }

        [FormerlySerializedAs("initialMaxVisibleWords")]
        [SerializeField]
        private int _initialMaxVisibleWords = 0;

        public int initialMaxVisibleWords
        {
            get => _initialMaxVisibleWords;
            set => _initialMaxVisibleWords = value;
        }

        [FormerlySerializedAs("targetMaxVisibleWords")]
        [SerializeField]
        private int _targetMaxVisibleWords = 0;

        public int targetMaxVisibleWords
        {
            get => _targetMaxVisibleWords;
            set => _targetMaxVisibleWords = value;
        }
    }   
}