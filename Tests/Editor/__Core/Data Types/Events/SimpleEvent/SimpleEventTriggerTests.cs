using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using AltSalt;

namespace Tests
{
    public class SimpleEventTriggerTests
    {
        //SimpleEvent simpleEvent;
        //SimpleEventTrigger simpleEventTrigger;

        //[SetUp]
        //public void BeforeEveryTest()
        //{
        //    simpleEvent = ScriptableObject.CreateInstance(typeof(SimpleEvent)) as SimpleEvent;
        //    simpleEventTrigger = new SimpleEventTrigger();
        //}

        //[Test]
        //public void _Test_SimpleEventTrigger_Constructor()
        //{
        //    Assert.AreEqual(simpleEvent, simpleEventTrigger.GetSimpleEvent());
        //}

        //[Test]
        //public void _Test_SimpleEventTrigger_CallExecution_With_GameObject()
        //{
        //    GameObject listenerObject = new GameObject();
        //    SimpleEventListener simpleEventListener = new SimpleEventListener(simpleEvent, listenerObject);

        //    bool eventTriggered = false;
        //    void EventCallback() { eventTriggered = true; }
        //    simpleEventListener.OnTargetEventExecuted += EventCallback;

        //    GameObject callerObject = new GameObject();
        //    simpleEventTrigger.RaiseEvent(callerObject);

        //    Assert.IsTrue(eventTriggered);

        //    UnityEngine.Object.DestroyImmediate(listenerObject);
        //    UnityEngine.Object.DestroyImmediate(callerObject);
        //}

        //[Test]
        //public void _Test_SimpleEventTrigger_CallExecution_With_String()
        //{
        //    GameObject listenerObject = new GameObject();
        //    SimpleEventListener simpleEventListener = new SimpleEventListener(simpleEvent, listenerObject);

        //    bool eventTriggered = false;
        //    void EventCallback() { eventTriggered = true; }
        //    simpleEventListener.OnTargetEventExecuted += EventCallback;

        //    simpleEventTrigger.RaiseEvent("caller string from test");

        //    Assert.IsTrue(eventTriggered);

        //    UnityEngine.Object.DestroyImmediate(listenerObject);
        //}

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        //[UnityTest]
        //public IEnumerator SimpleEventTriggerTestWithEnumeratorPasses()
        //{
        //    // Use the Assert class to test conditions.
        //    // Use yield to skip a frame.
        //    yield return null;
        //}
    }
}
