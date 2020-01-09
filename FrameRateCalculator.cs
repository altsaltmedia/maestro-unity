using System;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace AltSalt.Maestro
{
    // This script is taken from the Unity Standard Assets FPS Counter // 
    public class FrameRateCalculator : MonoBehaviour
    {
        const float measurePeriod = 0.5f;
        private int frames = 0;
        private float periodCounter = 0;
        
        private TextMeshPro _text;

        private TextMeshPro text
        {
            get => _text;
            set => _text = value;
        }

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private IntReference _framesPerSecond;

        private int framesPerSecond
        {
            get => _framesPerSecond.GetValue(this.gameObject);
            set => _framesPerSecond.GetVariable(this.gameObject).SetValue(value);
        }

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private FloatReference _frameStepInterval;

        private float frameStepInterval
        {
            get => _frameStepInterval.GetValue(this.gameObject);
            set => _frameStepInterval.GetVariable(this.gameObject).SetValue(value);
        }
        
        private float[] _frameStepHistory = new float[10];

        private float[] frameStepHistory
        {
            get => _frameStepHistory;
            set => _frameStepHistory = value;
        }

        private int _frameStepHistoryCounter = 0;
        
        private int frameStepHistoryCounter
        {
            get => _frameStepHistoryCounter;
            set => _frameStepHistoryCounter = value;
        }


        private void Start()
        {
            periodCounter = Time.realtimeSinceStartup + measurePeriod;
            text = GetComponent<TextMeshPro>();
        }

        private void Update()
        {
            // measure average frames per second
            frames++;
            if (Time.realtimeSinceStartup > periodCounter)
            {
                framesPerSecond = (int) (frames/measurePeriod);
                frames = 0;
                periodCounter += measurePeriod;
                
                
                // Here follow a few custom exponential equations to create framerate-independent step internvals
                // for animation in timeline (and whatever else may need it).
                //
                // These equations are based on a base frame rate of 60 seconds at Unity's default timestep of .02 seconds
                // between frames.
                //
                // Exponential functions are in the form y = ab^x

                float newStepInterval = 0.08f * Mathf.Pow(0.977159968434246f, framesPerSecond);
                if (newStepInterval < .02f) {
                    newStepInterval = .02f;
                }
                
//                frameStepInterval = 0.097521092336358f * Mathf.Pow( 0.973939972730944f, framesPerSecond);
//                if (frameStepInterval < .02f) {
//                    frameStepInterval = .02f;
//                }
                
//                frameStepInterval = 0.126992084157456f * Mathf.Pow(0.969663144646718f, framesPerSecond);
//                if (frameStepInterval < .02f) {
//                    frameStepInterval = .02f;
//                }

                frameStepHistory[frameStepHistoryCounter] = newStepInterval;
                frameStepHistoryCounter++;
                if (frameStepHistoryCounter >= frameStepHistory.Length) {
                    frameStepHistoryCounter = 0;
                }

                frameStepInterval = frameStepHistory.Average();

                // Piecewise equation an attempt to do calculations linearly was found by finding the equation of a line
                // between points (60, .02) and (30, .04) in the case of frame rates above 30,
                // and another line in between points (30, .04) and (15, .08) for values below 30.
                //
                // Note: When our framerate drops below 60, we increase the timestep accordingly so
                // our timeline animations in particular are consistent across devices.
                //
                // Equations are the following:
                //
                // y = -0.000666666666667x + .06
                // y = -0.002666666666667x + .12
                //
                
//                if (framesPerSecond >= 30) {
//                    frameStepInterval = framesPerSecond * -0.000666666666667f + .06f;
//                    if (frameStepInterval < .02f) {
//                        frameStepInterval = .02f;
//                    }
//                }
//                else {
//                    frameStepInterval = framesPerSecond * -0.002666666666667f + .12f;
//                    if (frameStepInterval > .08f) {
//                        frameStepInterval = .08f;
//                    }
//                }

                text.SetText($"FPS: {framesPerSecond} \n Step Interval {frameStepInterval:F6}");
            }
        }

        private static bool IsPopulated(IntReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
        
        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}