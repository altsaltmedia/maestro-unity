using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AltSalt
{
    [CreateAssetMenu(menuName = "AltSalt/Exposed Reference Tool")]
    public class ExposedReferenceTool : ScriptableObject
    {
        
        public ExposedReference<GameObject> testExposedReference;
        
        
    }
    
}