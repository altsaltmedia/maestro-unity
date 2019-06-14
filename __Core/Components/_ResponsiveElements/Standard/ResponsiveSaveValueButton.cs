using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    #if UNITY_EDITOR
    [ExecuteInEditMode]
    #endif
    public class ResponsiveSaveValueButton : MonoBehaviour
    {

#if UNITY_EDITOR
        [Button(ButtonSizes.Large), GUIColor(0.8f, 0.6f, 1)]
        [InfoBox("Saves all attributes on this object's IResponsiveSaveable components at the current breakpoint.")]
        public void SaveAllValues()
        {
            Debug.Log(this);
            Component[] iResponsiveSaveables;

            iResponsiveSaveables = GetComponents(typeof(IResponsiveSaveable));

            foreach (IResponsiveSaveable iResponsiveSaveable in iResponsiveSaveables) {
                iResponsiveSaveable.SaveValue();
            }
        }
#endif

    }   
}