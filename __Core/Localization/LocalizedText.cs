/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

namespace AltSalt
{
    public class LocalizedText : MonoBehaviour
    {
        [Required]
        public string key;

        [Required]
        public LocalizationManager m_localizationManager;

        [Required]
        public AppSettings appSettings;

        void Start()
        {
            PopulateWithText();
        }

        public void ManualTextUpdate()
        {
            PopulateWithText();
        }

        void PopulateWithText()
        {
            if (appSettings.localizationActive) {
                m_localizationManager.RefreshText(SceneManager.GetActiveScene().name, gameObject);
            }
        }

#if UNITY_EDITOR
        void OnRenderObject()
        {
            //Uncomment these lines to fix repopulate AppSettings on responsive objects
            //if the values are lost for some reason
            // 
            //if (appSettings == null) {
            //    appSettings = Utils.GetAppSettings();
            //}
        }
#endif
    }
}