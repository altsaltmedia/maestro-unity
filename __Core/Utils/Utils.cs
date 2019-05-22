/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AltSalt
{
    public enum DimensionType { Vertical, Horizontal }

    public enum AspectRatioType { x16x9, x9x16, x4x3 }

    public enum RatioType { Numerator, Denominator }

    public enum MaterialAttributeType { Color, Float }

    public enum BranchName { yNeg, yPos, xNeg, xPos }

    public enum AxisDestination { fromAxis, toAxis }

    public enum EventPayloadType { stringPayload, floatPayload, boolPayload, scriptableObjectPayload }

    public static class Utils
    {

        public static Color transparent = new Color(1, 1, 1, 0);
        public static float pageHeight = 6.3f;

        public static int GetAxisId(string axisName)
        {
            switch (axisName) {
                case "x":
                    return 0;

                case "y":
                    return 1;

                case "z":
                    return 2;

                default:
                    return 0;
            }
        }

        public static EventPayload CreateEventPayload()
        {
            EventPayload payloadInstance = ScriptableObject.CreateInstance(typeof(EventPayload)) as EventPayload;
            return payloadInstance;
        }

#if UNITY_EDITOR
        public static AppSettings GetAppSettings()
        {
            string[] guids;
            string path;

            guids = AssetDatabase.FindAssets("t:AppSettings");

            if(guids.Length > 1) {
                Debug.LogWarning("More than one matching file found. Please check to see if this is intentional.");
            }

            path = AssetDatabase.GUIDToAssetPath(guids[0]);

            return (AppSettings)AssetDatabase.LoadAssetAtPath(path, typeof(AppSettings));
        }

        public static ModifySettings GetModifySettings()
        {
            string[] guids;
            string path;

            guids = AssetDatabase.FindAssets("t:ModifySettings");

            if(guids.Length == 0) {
                throw new Exception("Modify Settings not found! You must create an instance of Modify Settings.");
            }

            if (guids.Length > 1) {
                Debug.LogWarning("More than one matching file found. Please check to see if this is intentional.");
            }

            path = AssetDatabase.GUIDToAssetPath(guids[0]);

            return (ModifySettings)AssetDatabase.LoadAssetAtPath(path, typeof(ModifySettings));
        }

        public static SimpleEvent GetSimpleEvent(string target)
        {
            string[] guids;
            string path;

            guids = AssetDatabase.FindAssets(target);

            if (guids.Length > 1) {
                Debug.LogWarning("More than one matching file found. Please check to see if this is intentional.");
            }

            path = AssetDatabase.GUIDToAssetPath(guids[0]);

            return (SimpleEvent)AssetDatabase.LoadAssetAtPath(path, typeof(SimpleEvent));
        }

        public static ComplexEvent GetComplexEvent(string target)
        {
            string[] guids;
            string path;

            guids = AssetDatabase.FindAssets(target);

            if (guids.Length > 1) {
                Debug.LogWarning("More than one matching file found. Please check to see if this is intentional.");
            }

            path = AssetDatabase.GUIDToAssetPath(guids[0]);

            return (ComplexEvent)AssetDatabase.LoadAssetAtPath(path, typeof(ComplexEvent));
        }

        public static FloatVariable GetFloatVariable(string target)
        {
            string[] guids;
            string path;

            guids = AssetDatabase.FindAssets(target);

            if (guids.Length > 1) {
                Debug.LogWarning("More than one matching file found. Please check to see if this is intentional.");
            }

            path = AssetDatabase.GUIDToAssetPath(guids[0]);

            return (FloatVariable)AssetDatabase.LoadAssetAtPath(path, typeof(FloatVariable));
        }

        public static BoolVariable GetBoolVariable(string target)
        {
            string[] guids;
            string path;

            guids = AssetDatabase.FindAssets(target);

            if (guids.Length > 1) {
                Debug.LogWarning("More than one matching file found. Please check to see if this is intentional.");
            }

            path = AssetDatabase.GUIDToAssetPath(guids[0]);

            return (BoolVariable)AssetDatabase.LoadAssetAtPath(path, typeof(BoolVariable));
        }

        public static ScriptableObject GetScriptableObject(string target)
        {
            string[] guids;
            string path;

            guids = AssetDatabase.FindAssets(target);

            if (guids.Length > 1) {
                Debug.LogWarning("More than one matching file found. Please check to see if this is intentional.");
            }

            path = AssetDatabase.GUIDToAssetPath(guids[0]);

            return (ScriptableObject)AssetDatabase.LoadAssetAtPath(path, typeof(ScriptableObject));
        }

        public static void RepaintInspector(System.Type t)
        {
            Editor[] ed = (Editor[])Resources.FindObjectsOfTypeAll<Editor>();
            for (int i = 0; i < ed.Length; i++) {
                if (ed[i].GetType() == t) {
                    ed[i].Repaint();
                    return;
                }
            }
        }

        public static void RepaintInspector()
        {
            Editor[] ed = (Editor[])Resources.FindObjectsOfTypeAll<Editor>();
            for (int i = 0; i < ed.Length; i++) {
                ed[i].Repaint();
                return;
            }
        }
#endif

        // Given a value and a list of other values to compare against, will check if
        // a value is in between the numbers in the list, returning the first index
        // where it is greater than or equal to the first value, but less than the following
        // value. If less than the smallest value of the list, returns index 0; if greater
        // than every value of the list, returns the list count.
        public static int GetValueIndexInList(float value, List<float> listOfValues)
        {
            int valueIndex = -1;

            // If there's only one value in our comparison list,
            // make a simple comparison to see if our value is
            // less than or greater than it.
            if (listOfValues.Count == 1) {
                if (value < listOfValues[0]) {
                    valueIndex = 0;
                } else {
                    valueIndex = 1;
                }
            }
            else {
                if (value < listOfValues[0]) {
                    valueIndex = 0;
                } else {
                    for (int i = 0; i < listOfValues.Count - 1; i++) {
                        if (value >= listOfValues[i] && value < listOfValues[i + 1]) {
                            valueIndex = i + 1;
                            break;
                        }
                    }
                    if(valueIndex == -1) {
                        valueIndex = listOfValues.Count;
                    }
                }
            }

            if(valueIndex == -1) {
                Debug.LogError("Given list, could not find index for specified value. Is your list populated correctly?");
            }

            return valueIndex;
        }

        public static void ExpandList(List<Rect> list, int index)
        {
            // Must add 1 to zero-based index in order to compare correctly
            // with the length of the list
            while (list.Count < index + 1) {
                list.Add(new Rect());
            }
        }

        public static void ExpandList(List<Vector2> list, int index)
        {
            // Must add 1 to zero-based index in order to compare correctly
            // with the length of the list
            while(list.Count < index + 1) {
                list.Add(new Vector2());
            }
        }

        public static void ExpandList(List<Vector3> list, int index)
        {
            // Must add 1 to zero-based index in order to compare correctly
            // with the length of the list
            while (list.Count < index + 1) {
                list.Add(new Vector3());
            }
        }

        public static void ExpandList(List<float> list, int index)
        {
            // Must add 1 to zero-based index in order to compare correctly
            // with the length of the list
            while (list.Count < index + 1) {
                list.Add(new float());
            }
        }

        public static void ExpandList(List<DimensionType> list, int index)
        {
            // Must add 1 to zero-based index in order to compare correctly
            // with the length of the list
            while (list.Count < index + 1) {
                list.Add(new DimensionType());
            }
        }

        public static void ExpandList(List<AspectRatioType> list, int index)
        {
            // Must add 1 to zero-based index in order to compare correctly
            // with the length of the list
            while (list.Count < index + 1) {
                list.Add(new AspectRatioType());
            }
        }

        public static float GetValueFromDesiredPercent(float baseNumber, float desiredPercent)
        {
            return baseNumber * desiredPercent;
        }

        /*
        * @ClampVectorValue
        * 
        * Restricts the sensitivity of swipe information
        * 
        * @param Vector2 swipe Swipe information from EasyTouch plugin
        * @param float sensitivity Sensitivity for either OnSwipe or OnSwipeEnd
        */
        public static Vector2 ClampVectorValue(Vector2 rawVector, float maxMin = 1f)
        {
            Vector2 clampedVector = new Vector2(0, 0);

            clampedVector.x = Mathf.Clamp(rawVector.x, maxMin * -1f, maxMin);
            clampedVector.y = Mathf.Clamp(rawVector.y, maxMin * -1f, maxMin);

            return clampedVector;
        }

        /*
        * @ConvertV2toV3
        * 
        * Sets the velocity depending on which axis is currently active
        * 
        * @param Vector2 swipe The clamped swipe information
        */
        public static Vector3 ConvertV2toV3(Vector2 rawVector)
        {
            Vector3 v3 = new Vector3(0, 0, 0);

            Vector2 v2 = rawVector;

            v3.x = v2.x;
            v3.y = v2.y;
            v3.z = v2.y;

            return v3;
        }

        public static Vector3 InvertV3Values(Vector3 vector3, string[] axesToInvert) {
            for (int i = 0; i < axesToInvert.Length; i++) {
                int axisId = Utils.GetAxisId(axesToInvert[i]);
                vector3[axisId] *= -1;
            }
            return vector3;
        }

        public static int GetV3Direction(Vector3 vector3)
        {
            float vectorValue = vector3.x + vector3.y;
            if(vectorValue >= 0) {
                return 1;
            } else {
                return -1;
            }
        }

        // Raises a Vector3 to a power while retaining
        // its original positive or negative values
        public static Vector3 ExponentiateV3(Vector3 rawVector, float power)
        {
            Vector3 exponentV3 = new Vector3();
            for (int i = 0; i < 3; i++) {
                exponentV3[i] = ExponentiatePosNegValue(rawVector[i], power);
            }
            return exponentV3;
        }

        // Raises a positive or negative value to a power while
        // retaining its original positive or negative value
        public static float ExponentiatePosNegValue(float rawValue, float power)
        {
            int sign = rawValue >= 0 ? 1 : -1;
            float distance = Mathf.Abs(rawValue);
            float exponentVal = distance > 0 ? Mathf.Pow(distance, power) : 0;
            return exponentVal *= sign;
        }

        public static List<int> AllIndexesOf(this string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length) {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }

        public static bool IsPopulated(ColorReference attribute)
        {
            if (attribute.UseConstant == true) {
                return true;
            }
            else {
                return attribute.Variable == null ? false : true;
            }
        }

        public static bool IsPopulated(FloatReference attribute)
        {
            if (attribute.UseConstant == true) {
                return true;
            }
            else {
                return attribute.Variable == null ? false : true;
            }
        }

        public static bool IsPopulated(IntReference attribute)
        {
            if (attribute.UseConstant == true) {
                return true;
            }
            else {
                return attribute.Variable == null ? false : true;
            }
        }


        public static bool IsPopulated(BoolReference attribute)
        {
            if (attribute.UseConstant == true) {
                return true;
            }
            else {
                return attribute.Variable == null ? false : true;
            }
        }

        public static bool IsPopulated(V2Reference attribute)
        {
            if (attribute.UseConstant == true) {
                return true;
            }
            else {
                return attribute.Variable == null ? false : true;
            }
        }

        public static bool IsPopulated(V3Reference attribute)
        {
            if (attribute.UseConstant == true) {
                return true;
            }
            else {
                return attribute.Variable == null ? false : true;
            }
        }

        public static bool IsPopulated(List<float> attribute)
        {
            return attribute.Count < 1 ? false : true;
        }

        public static bool IsPopulated(List<Sequence> attribute)
        {
            return attribute.Count < 1 ? false : true;
        }

        public static bool IsPopulated(List<TargetMaterialAttribute> attribute)
        {
            return attribute.Count < 1 ? false : true;
        }

        public static bool IsPopulated(List<TextCollectionBank> attribute) {
            return attribute.Count < 1 ? false : true;
        }

        public static bool IsPopulated(UnityEvent attribute)
        {
            return attribute.GetPersistentMethodName(0).Length < 1 ? false : true;
        }

        public static bool IsPopulated(ComplexUnityEventHandler attribute)
        {
            return attribute.GetPersistentMethodName(0).Length < 1 ? false : true;
        }

        public static int GetNthIndexFromEnd(string s, char t, int n)
        {
            int count = 0;
            for (int i = s.Length - 1; i >= 0; i--) {
                if (s[i] == t) {
                    count++;
                    if (count == n) {
                        return i;
                    }
                }
            }
            return -1;
        }

        public static bool IsPopulated(StringReference attribute)
        {
            if (attribute.UseConstant == true) {
                return true;
            }
            else {
                return attribute.Variable == null ? false : true;
            }
        }

        public static float Quadratic(float k)
        {
            return (k * k) / 2;
        }

        private static float Asymptotic(float k)
        {
            return k / ((k * k) - 1);
        }

        private static float Cubic(float k)
        {
            return (k * k * (3 - k));
        }

        private static float Bounce(float k)
        {
            if ((k /= 1f) < (1f / 2.75f)) {
                return 7.5625f * k * k;
            }
            else if (k < (2f / 2.75f)) {
                return 7.5625f * (k -= (1.5f / 2.75f)) * k + 0.75f;
            }
            else if (k < (2.5 / 2.75)) {
                return 7.5625f * (k -= (2.25f / 2.75f)) * k + 0.9375f;
            }
            else {
                return 7.5625f * (k -= (2.625f / 2.75f)) * k + 0.984375f;
            }
        }

        private static float Circular(float k)
        {
            return Mathf.Sqrt(1f - (--k * k));
        }

        private static float Back(float k)
        {
            float b = 4f;
            return (k = k - 1f) * k * ((b + 1f) * k + b) + 1f;
        }

        private static float Elastic(float k)
        {
            float f = 0.22f;
            float e = 0.4f;

            if (k == 0) { return 0; }
            if (k == 1f) { return 1f; }

            return (e * Mathf.Pow(2f, -10f * k) * Mathf.Sin((k - f / 4f) * (2f * Mathf.PI) / f) + 1f);

        }
    }



}