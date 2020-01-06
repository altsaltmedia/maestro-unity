/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System;
using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Logic
{
    public class AltSaltBootstrap : MonoBehaviour
    {
        [Required]
        [SerializeField]
        [ReadOnly]
        private AppSettings _appSettings;

        private AppSettings appSettings
        {
            get
            {
                if (_appSettings == null) {
                    _appSettings = Utils.GetAppSettings();
                }

                return _appSettings;
            }
            set => _appSettings = value;
        }
        
        [Required]
        [SerializeField]
        ComplexEventTrigger initializeAppTrigger;

        [SerializeField]
        [DisableIf("loadDebugMenu")]
        [BoxGroup("Standard Scenes")]
        [InfoBox("Loaded when the app first opens")]
        string initialSceneName;

        [SerializeField]
        [DisableIf("loadDebugMenu")]
        [BoxGroup("Standard Scenes")]
        [InfoBox("Loaded upon subsequent openings")]
        string subsequentSceneName;

        [PropertySpace(1)]

        [SerializeField]
        [BoxGroup("Debug Scene")]
        bool loadDebugMenu;

        [SerializeField]
        [ShowIf("loadDebugMenu")]
        [BoxGroup("Debug Scene")]
        string debugMenuName;

        public void CallInitializeApp()
        {
            StartCoroutine(InitializeApp());
        }

        IEnumerator InitializeApp()
        {
            Physics.autoSimulation = false;
            Application.targetFrameRate = 60;

            yield return new WaitForSeconds(1);

            string targetScene;

            if(loadDebugMenu == true) {
                targetScene = debugMenuName;
            } else {
                if(appSettings.hasBeenOpened == false) {
                    targetScene = initialSceneName;
                } else {
                    targetScene = subsequentSceneName;
                }
            }

            initializeAppTrigger.RaiseEvent(this.gameObject, targetScene);
            yield break;
        }
    }
}