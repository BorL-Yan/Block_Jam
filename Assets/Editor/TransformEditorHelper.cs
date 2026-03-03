using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

[CustomEditor(typeof(Transform))]
[CanEditMultipleObjects]
public class TransformEditorHelper : Editor
{
    private SerializedProperty m_Pos;
    private SerializedProperty m_Rot;
    private SerializedProperty m_Scale;

    private void OnEnable()
    {
        m_Pos = serializedObject.FindProperty("m_LocalPosition");
        m_Rot = serializedObject.FindProperty("m_LocalRotation");
        m_Scale = serializedObject.FindProperty("m_LocalScale");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawPropertyWithReset("Position", m_Pos, Vector3.zero);
        DrawPropertyWithReset("Rotation", m_Rot, Quaternion.identity);
        DrawPropertyWithReset("Scale", m_Scale, Vector3.one);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawPropertyWithReset(string label, SerializedProperty property, object resetValue)
    {
        EditorGUILayout.BeginHorizontal();

        // Отрисовка самого поля (позиция, поворот или масштаб)
        if (property.propertyType == SerializedPropertyType.Quaternion)
        {
            // Для поворота используем Euler angles, как в стандартном инспекторе
            Vector3 euler = property.quaternionValue.eulerAngles;
            EditorGUI.BeginChangeCheck();
            euler = EditorGUILayout.Vector3Field(label, euler);
            if (EditorGUI.EndChangeCheck())
            {
                property.quaternionValue = Quaternion.Euler(euler);
            }
        }
        else
        {
            EditorGUILayout.PropertyField(property, new GUIContent(label));
        }

        // Кнопка сброса "R" (Reset)
        GUI.backgroundColor = new Color(1f, 0.6f, 0.6f); // Слегка красноватая кнопка
        if (GUILayout.Button("R", GUILayout.Width(25), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
        {
            if (resetValue is Vector3 v3) property.vector3Value = v3;
            else if (resetValue is Quaternion q) property.quaternionValue = q;
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.EndHorizontal();
    }
}