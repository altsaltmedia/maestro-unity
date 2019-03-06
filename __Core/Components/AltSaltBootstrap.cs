/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using UnityEngine;
using System.Collections;
using DoozyUI;
using Sirenix.OdinInspector;

namespace AltSalt
{
    public class AltSaltBootstrap : MonoBehaviour
    {
        [Required]
        [SerializeField]
        AppSettings appSettings;

        [Required]
        [SerializeField]
        ComplexEvent sceneLoadTriggered;

        [InfoBox("Will load this scene immediately following the bootstrapper")]
        [SerializeField]
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

            EventPayload eventPayload = new EventPayload();

            if(loadDebugMenu == true) {
                eventPayload.Set(EventPayloadType.stringPayload.ToString(), debugMenuName);
            } else {
                eventPayload.Set(EventPayloadType.stringPayload.ToString(), firstSceneName);
            }
            eventPayload.Set(EventPayloadType.boolPayload.ToString(), true);
            sceneLoadTriggered.Raise(eventPayload);
            yield break;
        }
    }
}