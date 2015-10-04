using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Assets.Code.Tools
{
    public static class Helper
    {
        /// <summary>
        /// Add a new child game object.
        /// </summary>
        public static GameObject AddChild(GameObject parent, bool undo)
        {
            GameObject go = new GameObject();
            if (parent != null)
            {
                Transform t = go.transform;
                t.SetParent(parent.transform, false);
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;
                go.layer = parent.layer;
            }
            return go;
        }

        /// <summary>
        /// Instantiate an object and add it to the specified parent.
        /// </summary>

        public static GameObject AddChild(GameObject parent, GameObject prefab)
        {
            GameObject go = GameObject.Instantiate(prefab) as GameObject;
#if UNITY_EDITOR
            UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create Object");
#endif
            if (go != null && parent != null)
            {
                Transform t = go.transform;
                t.SetParent(parent.transform, false);
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;
                go.layer = parent.layer;
            }
            return go;
        }

        /// <summary>
        /// Ensure that the angle is within -180 to 180 range.
        /// </summary>

        [System.Diagnostics.DebuggerHidden]
        [System.Diagnostics.DebuggerStepThrough]
        static public float WrapAngle(float angle)
        {
            while (angle > 180f) angle -= 360f;
            while (angle < -180f) angle += 360f;
            return angle;
        }

        public static IEnumerator UnscaledWait(float seconds)
        {
            var finishTime = Time.unscaledTime + seconds;
            while (Time.unscaledTime < finishTime)
                yield return null;
        }

        public static bool CustomApproximately(float val1, float val2, float epsilon = 0.001f)
        {
            return Mathf.Abs(Mathf.Abs(val1) - Mathf.Abs(val2)) < epsilon;
        }

        public static T[, ,] SingleToMulti<T>(int width, int height, int deep, T[] array)
        {
            int index = 0;
            T[, ,] multi = new T[width, height, deep];

            for (int z = 0; z < deep; z++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        multi[x, y, z] = array[index];
                        index++;
                    }
                }
            }
            return multi;
        }

        public static T[] MultiToSingle<T>(T[, ,] array)
        {
            int index = 0;
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            int deep = array.GetLength(2);

            T[] single = new T[(width * height * deep)];

            for (int z = 0; z < deep; z++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        single[index] = array[x, y, z];
                        index++;
                    }
                }
            }
            return single;
        }

        public static string ColorizedText(string text, string color)
        {
            return string.Format("<color={0}>{1}</color>", color, text);
        }
    }
}
