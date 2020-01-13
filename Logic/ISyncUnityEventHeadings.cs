using UnityEditor;
using UnityEngine;

namespace AltSalt.Maestro.Logic
{
    public interface ISyncUnityEventHeadings
    {
        void SyncUnityEventHeadings(SerializedProperty unityEventParentProperty);
    }
}