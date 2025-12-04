#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Reflection; // Больше не нужно, но оставим на всякий случай

[CustomEditor(typeof(GridController))]
public class GridControllerEditor : Editor
{
    private SerializedProperty rowsProp;
    private SerializedProperty columnsProp;
    
    private GridController Controller => (GridController)target;
    
    private void OnEnable()
    {
        // Ищем свойства по имени приватных полей
        rowsProp = serializedObject.FindProperty("_rows");
        columnsProp = serializedObject.FindProperty("_columns");
    }

    public override void OnInspectorGUI()
    {
        // Всегда начинаем с обновления сериализованного объекта
        serializedObject.Update(); 

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Grid Settings", EditorStyles.boldLabel);

        // 🔹 Rows
        EditorGUILayout.BeginHorizontal();
        
        // Используем PropertyField для стандартного отображения (с поддержкой Min(1))
        EditorGUILayout.PropertyField(rowsProp, new GUIContent("Rows"));

        if (GUILayout.Button("-", GUILayout.Width(30)))
            SetRows(rowsProp.intValue - 1);

        if (GUILayout.Button("+", GUILayout.Width(30)))
            SetRows(rowsProp.intValue + 1);

        EditorGUILayout.EndHorizontal();

        // 🔹 Columns
        EditorGUILayout.BeginHorizontal();
        
        // Используем PropertyField для стандартного отображения (с поддержкой Min(1))
        EditorGUILayout.PropertyField(columnsProp, new GUIContent("Columns"));

        if (GUILayout.Button("-", GUILayout.Width(30)))
            SetColumns(columnsProp.intValue - 1);

        if (GUILayout.Button("+", GUILayout.Width(30)))
            SetColumns(columnsProp.intValue + 1);

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);
        if (GUILayout.Button("Set Borders to Walls", GUILayout.Height(30)))
        {
            // Вызываем метод контроллера
            Controller.SetBordersToWalls();
        }
        EditorGUILayout.Space(10);
        if (GUILayout.Button("Set Bevel to Walls", GUILayout.Height(30)))
        {
            // Вызываем метод контроллера
            Controller.BevelCalculate();
        }
        
        
        EditorGUILayout.Space(5);

        if (GUILayout.Button("Reset Level (Keep Grid)", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("Сброс уровня", 
                    "Это удалит все БЛОКИ и СТЕНЫ, но оставит пустую сетку. Продолжить?", "Да", "Отмена"))
            {
                ((GridController)target).ClearGridContent();
            }
        }
        
        
        // Показываем остальные поля, исключая те, что уже обработаны
        EditorGUILayout.Space(10);
        DrawPropertiesExcluding(serializedObject, "m_Script", "_rows", "_columns");

        // Применяем изменения, если они были
        bool changed = serializedObject.ApplyModifiedProperties();

        // Ручная перестройка, если размер изменился
        if (changed)
        {
            // Проверяем, изменились ли размеры (после ApplyModifiedProperties)
            if (rowsProp.intValue != Controller.Rows || columnsProp.intValue != Controller.Columns)
            {
                 // Обновление _prevRows и _prevColumns будет сделано в GridController.Update()
                 // или принудительно через RebuildGrid()
                 
                 // Принудительный RebuildGrid
                 if (!Application.isPlaying)
                 {
                    Controller.RebuildGrid();
                 }
            }
        }
        
        // Принудительная перерисовка, чтобы видеть изменения сетки в EditMode
        if (!Application.isPlaying)
            EditorUtility.SetDirty(target);
    }
    
    private void SetRows(int value)
    {
        rowsProp.intValue = Mathf.Max(1, value);
        serializedObject.ApplyModifiedProperties();
        
        // Принудительный RebuildGrid сразу после изменения
        if (!Application.isPlaying)
        {
             Controller.RebuildGrid();
        }
    }

    private void SetColumns(int value)
    {
        columnsProp.intValue = Mathf.Max(1, value);
        serializedObject.ApplyModifiedProperties();
        
        // Принудительный RebuildGrid сразу после изменения
        if (!Application.isPlaying)
        {
             Controller.RebuildGrid();
        }
    }
}

#endif