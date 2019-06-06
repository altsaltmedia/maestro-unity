/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System;
using UnityEngine;
using System.Collections;
using DoozyUI;
using Sirenix.OdinInspector;
using System.Reflection;

namespace AltSalt
{
    public class AltSaltBootstrap : MonoBehaviour
    {
        [Required]
        [SerializeField]
        AppSettings appSettings;

        [Required]
        [SerializeField]
        ComplexEvent initializeApp;

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

        IEnumerator Start()
        {
            Physics.autoSimulation = false;
            Application.targetFrameRate = 60;

            yield return new WaitForSeconds(1);
            
            EventPayload eventPayload = EventPayload.CreateInstance();

            if(loadDebugMenu == true) {
                eventPayload.Set(debugMenuName);
            } else {
                if(appSettings.hasBeenOpened == false) {
                    eventPayload.Set(initialSceneName);
                } else {
                    eventPayload.Set(subsequentSceneName);
                }
            }
            initializeApp.StoreCaller(this.gameObject);
            initializeApp.Raise(eventPayload);
            Destroy(eventPayload);

            yield break;
        }
    }
}