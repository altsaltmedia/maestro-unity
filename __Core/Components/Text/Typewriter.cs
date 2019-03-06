using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AltSalt
{
    [ExecuteInEditMode]
    public class Typewriter : MonoBehaviour
    {
        public int maxVisibleCharacters = 0;
        public int maxVisibleWords = 0;
        public int maxVisibleLines = 0;

        TextMeshPro textMeshPro;

        // Use this for initialization
        void Start () {
            GetTextMeshPro();
        }
        
        // Update is called once per frame
        void Update () {
            if(textMeshPro == null) {
                GetTextMeshPro();
            }
            textMeshPro.maxVisibleCharacters = maxVisibleCharacters;
            textMeshPro.maxVisibleWords = maxVisibleWords;
            textMeshPro.maxVisibleLines = maxVisibleLines;
        }

        void GetTextMeshPro()
        {
            textMeshPro = GetComponent<TextMeshPro>();
        }
    }
}   