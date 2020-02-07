using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace AltSalt.Maestro
{
    // This script is taken from the Unity Standard Assets FPS Counter // 
    [ExecuteInEditMode]
    public class FrameRateController : MonoBehaviour
    {
        [Required]
        [SerializeField]
        [ReadOnly]
        private AppSettingsReference _appSettings = new AppSettingsReference();

        private AppSettings appSettings => _appSettings.GetVariable() as AppSettings;
        
        private int framesPerSecond
        {
            get => appSettings.GetFramesPerSecond(this);
            set => appSettings.SetFramesPerSecond(this.gameObject, value);
        }

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        private List<FloatReference> _frameStepValues = new List<FloatReference>();

        private List<FloatReference> frameStepValues => _frameStepValues;
        
        const float measurePeriod = 0.5f;
        private int frames = 0;
        private float periodCounter = 0;
        
        private TextMeshPro _text;

        private TextMeshPro text
        {
            get => _text;
            set => _text = value;
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

        private void Awake()
        {
#if UNITY_EDITOR
            _appSettings.PopulateVariable(this, nameof(_appSettings));
            string frameStepListPath = nameof(_frameStepValues);
            for (int i = 0; i < frameStepValues.Count; i++) {
                string frameStepReferencePath = $"{frameStepListPath}.{i}"; 
                frameStepValues[i].PopulateVariable(this, frameStepReferencePath);
            }
#endif
            periodCounter = Time.realtimeSinceStartup + measurePeriod;
            text = GetComponent<TextMeshPro>();
        }

        private void Update()
        {
            if (Application.isPlaying == false) return;
            
            // measure average frames per second
            frames++;
            if (Time.realtimeSinceStartup > periodCounter)
            {
                framesPerSecond = (int) (frames/measurePeriod);
                frames = 0;
                periodCounter += measurePeriod;
                
                
                // Here follow a few custom equations to create framerate-independent step intervals
                // for animation in timeline (and whatever else may need it). Each equation is slightly
                // different, and some more correct than others - that being said, the I selected has
                // the best feel.
                //
                // These equations are based on a base frame rate of 60 seconds at Unity's default timestep of .02 seconds
                // between frames.
                //
                // Exponential functions are in the form y = ab^x

                float modifiedStepValue = 0.08f * Mathf.Pow(0.977159968434246f, framesPerSecond);
                if (modifiedStepValue < .02f) {
                    modifiedStepValue = .02f;
                }
                
//                frameStepInterval = 0.097521092336358f * Mathf.Pow( 0.973939972730944f, framesPerSecond);
//                if (frameStepInterval < .02f) {
//                    frameStepInterval = .02f;
//                }
                
//                frameStepInterval = 0.126992084157456f * Mathf.Pow(0.969663144646718f, framesPerSecond);
//                if (frameStepInterval < .02f) {
//                    frameStepInterval = .02f;


                // Piecewise equations - Attempt to do calculations linearly found by finding the equation of a line
                // between points (60, .02) and (30, .04) in the case of frame rates above 30,
                // and another line in between points (30, .04) and (15, .08) for values below 30.
                //
                // (When our framerate drops below 60, we increase the timestep accordingly so
                // our timeline animations in particular are consistent across devices.)
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


                frameStepHistory[frameStepHistoryCounter] = modifiedStepValue;
                frameStepHistoryCounter++;
                if (frameStepHistoryCounter >= frameStepHistory.Length) {
                    frameStepHistoryCounter = 0;
                }

                float newStepValue = frameStepHistory.Average();
                for (int i = 0; i < frameStepValues.Count; i++) {
                    frameStepValues[i].SetValue(this.gameObject, newStepValue);
                }
                
                text.SetText($"FPS: {framesPerSecond} \n Step Interval {frameStepValues[0]:F6}");
            }
        }

        private static bool IsPopulated(List<FloatReference> attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}