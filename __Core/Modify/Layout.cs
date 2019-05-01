using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt
{
    [CreateAssetMenu(menuName = "AltSalt/Modify/Layout")]
    public class Layout : ScriptableObject
    {
        #if UNITY_EDITOR
        [SerializeField]
        [Multiline]
        string description;
        #endif
        
        // Start is called before the first frame update
        void Start()
        {
            
        }
        
        // Update is called once per frame
        void Update()
        {
            
        }
    }   
}