using System;
using Assets.Code.Tools;
using UnityEditor;
using UnityEngine;

namespace Assets.Code.Editor
{
    [CustomPropertyDrawer(typeof(RandomRangeAttribute))]
    public class RandomRangeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float min, max;
            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                min = property.vector2Value.x;
                max = property.vector2Value.y;
            }
            else
                throw new NotSupportedException("Only Vector2 supported");

            label = EditorGUI.BeginProperty(position, label, property);
            {
                position = EditorGUI.PrefixLabel(position, label);
                var editWidth = position.width/2 - 40;
                EditorGUI.BeginChangeCheck();
                GUI.Label(new Rect(position.xMin, position.yMin, 30, position.height), "Min");
                min = EditorGUI.FloatField(new Rect(position.xMin + 40, position.yMin, editWidth, position.height), min);

                var maxXPosition = position.width/2 + position.xMin;
                GUI.Label(new Rect(maxXPosition, position.yMin, 30, position.height), "Max");
                max = EditorGUI.FloatField(new Rect(maxXPosition + 40, position.yMin, editWidth, position.height), max);

                if (EditorGUI.EndChangeCheck())
                {
                    if (min > max)
                        min = max;
                    if (max < min)
                        max = min;

                    property.vector2Value = new Vector2(min, max);
                }
            }
            EditorGUI.EndProperty();
        }
    }
}
