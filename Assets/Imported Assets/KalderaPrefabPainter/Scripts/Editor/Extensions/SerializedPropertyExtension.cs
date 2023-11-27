using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CollisionBear.WorldEditor.Lite.Extensions
{
    public static class SerializedPropertyExtension
    {
        public static T AddToObjectArray<T>(this SerializedProperty arrayProperty, T item) where T : UnityEngine.Object
        {
            if (!arrayProperty.isArray) {
                throw new UnityException(string.Format("Serialized Property {0} is not an array", arrayProperty.name));
            }

            arrayProperty.serializedObject.Update();
            arrayProperty.InsertArrayElementAtIndex(arrayProperty.arraySize);
            arrayProperty.GetArrayElementAtIndex(arrayProperty.arraySize - 1).objectReferenceValue = item;
            arrayProperty.serializedObject.ApplyModifiedProperties();
            return item;
        }

        public static void RemoveFromObjectArrayAt(this SerializedProperty arrayProperty, int index)
        {
            if (index < 0) {
                throw new UnityException("Removing negative index not supported");
            }

            if (!arrayProperty.isArray) {
                throw new UnityException(string.Format("Serialized Property {0} is not an array", arrayProperty.name));
            }

            if (index > arrayProperty.arraySize - 1) {
                throw new UnityException(string.Format("Index is out of bounds", arrayProperty.name));
            }

            arrayProperty.serializedObject.Update();

            arrayProperty.GetArrayElementAtIndex(index).SetPropertyValue(null);
            arrayProperty.DeleteArrayElementAtIndex(index);
            arrayProperty.serializedObject.ApplyModifiedProperties();
        }

        public static bool ArrayContains<T>(this SerializedProperty arrayProperty, T item) where T : UnityEngine.Object
        {
            if (!arrayProperty.isArray) {
                throw new UnityException(string.Format("Serialized Property {0} is not an array", arrayProperty.name));
            }

            arrayProperty.serializedObject.Update();

            for (int i = 0; i < arrayProperty.arraySize; i++) {
                if (arrayProperty.GetArrayElementAtIndex(i).objectReferenceValue == item) {
                    return true;
                }
            }

            return false;
        }

        public static void SetPropertyValue(this SerializedProperty property, object value)
        {
            switch (property.propertyType) {

                case SerializedPropertyType.AnimationCurve:
                    property.animationCurveValue = value as AnimationCurve;
                    break;

                case SerializedPropertyType.ArraySize:
                    property.intValue = Convert.ToInt32(value);
                    break;

                case SerializedPropertyType.Boolean:
                    property.boolValue = Convert.ToBoolean(value);
                    break;

                case SerializedPropertyType.Bounds:
                    property.boundsValue = (value == null)
                            ? new Bounds()
                            : (Bounds)value;
                    break;

                case SerializedPropertyType.Character:
                    property.intValue = Convert.ToInt32(value);
                    break;

                case SerializedPropertyType.Color:
                    property.colorValue = (value == null)
                            ? new Color()
                            : (Color)value;
                    break;

                case SerializedPropertyType.Float:
                    property.floatValue = Convert.ToSingle(value);
                    break;

                case SerializedPropertyType.Integer:
                    property.intValue = Convert.ToInt32(value);
                    break;

                case SerializedPropertyType.LayerMask:
                    property.intValue = (value is LayerMask) ? ((LayerMask)value).value : Convert.ToInt32(value);
                    break;

                case SerializedPropertyType.ObjectReference:
                    property.objectReferenceValue = value as UnityEngine.Object;
                    break;

                case SerializedPropertyType.Quaternion:
                    property.quaternionValue = (value == null)
                            ? Quaternion.identity
                            : (Quaternion)value;
                    break;

                case SerializedPropertyType.Rect:
                    property.rectValue = (value == null)
                            ? new Rect()
                            : (Rect)value;
                    break;

                case SerializedPropertyType.String:
                    property.stringValue = value as string;
                    break;

                case SerializedPropertyType.Vector2:
                    property.vector2Value = (value == null)
                            ? Vector2.zero
                            : (Vector2)value;
                    break;

                case SerializedPropertyType.Vector3:
                    property.vector3Value = (value == null)
                            ? Vector3.zero
                            : (Vector3)value;
                    break;

                case SerializedPropertyType.Vector4:
                    property.vector4Value = (value == null)
                            ? Vector4.zero
                            : (Vector4)value;
                    break;

            }
        }

        public static void RemoveFromObjectArray<T>(this SerializedProperty arrayProperty, T elementToRemove) where T : UnityEngine.Object
        {
            if (!arrayProperty.isArray) {
                throw new UnityException(string.Format("Serialized Property {0} is not an array", arrayProperty.name));
            }

            if (!elementToRemove) {
                throw new UnityException("Removing null element is not supported");
            }

            arrayProperty.serializedObject.Update();
            for (var i = 0; i < arrayProperty.arraySize; i++) {
                var elementProperty = arrayProperty.GetArrayElementAtIndex(i);
                if (elementProperty.objectReferenceValue == elementToRemove) {
                    arrayProperty.RemoveFromObjectArrayAt(i);
                    return;
                }
            }

            throw new UnityException(string.Format("Element {0} not found in property {1}", elementToRemove.name, arrayProperty.name));
        }


        public static System.Type GetPropertyType(this SerializedProperty property)
        {
            System.Type parentType = property.serializedObject.targetObject.GetType();

            var propertyPathParts = property.propertyPath.Split('.').ToList();
            System.Reflection.FieldInfo fieldInfo = parentType.GetField(propertyPathParts.First());
            propertyPathParts.RemoveAt(0);

            foreach (var part in propertyPathParts) {
                parentType = fieldInfo.FieldType;
                fieldInfo = parentType.GetField(part);
            }

            return parentType;
        }
    }
}
