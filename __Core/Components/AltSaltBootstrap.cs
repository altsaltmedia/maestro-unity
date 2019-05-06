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

        [InfoBox("Will load this scene immediately following the bootstrapper")]
        [SerializeField]
        [DisableIf("loadDebugMenu")]
        string firstSceneName;

        [SerializeField]
        bool loadDebugMenu;

        [SerializeField]
        [ShowIf("loadDebugMenu")]
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
                eventPayload.Set(firstSceneName);
            }
            initializeApp.Raise(eventPayload);
            Destroy(eventPayload);

            yield break;
        }
    }
}