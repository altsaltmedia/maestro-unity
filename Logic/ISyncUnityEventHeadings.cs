using UnityEditor;
using UnityEngine;

namespace AltSalt.Maestro.Logic
{
    public interface ISyncUnityEventHeadings
    {
#if UNITY_EDITOR        
        void SyncUnityEventHeadings(SerializedProperty unityEventParentProperty);
#endif        
    }
}