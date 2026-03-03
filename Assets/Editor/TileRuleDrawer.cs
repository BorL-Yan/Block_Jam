using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TileRule))]
public class TileRuleDrawer : PropertyDrawer
{
    private const float GridSize = 66f; // Размер всей сетки
    private const float CellSize = 22f; // Размер одной кнопки
 
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 130f;
    }
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect fieldsRect = new Rect(position.x, position.y + 5, position.width - GridSize - 20, position.height);
        Rect gridRect = new Rect(position.x + position.width - GridSize - 5, position.y + 10, GridSize, GridSize);

        float lh = EditorGUIUtility.singleLineHeight;
        
        // 1. Рисуем Имя
        EditorGUI.PropertyField(new Rect(fieldsRect.x, fieldsRect.y, fieldsRect.width, lh), property.FindPropertyRelative("Name"));
        
        // 2. Рисуем Префаб (смещаемся вниз)
        EditorGUI.PropertyField(new Rect(fieldsRect.x, fieldsRect.y + lh + 2, fieldsRect.width, lh), property.FindPropertyRelative("Prefab"));
        
        // 3. НОВОЕ: Рисуем Rotation (смещаемся еще ниже)
        EditorGUI.PropertyField(new Rect(fieldsRect.x, fieldsRect.y + (lh + 2) * 2, fieldsRect.width, lh), property.FindPropertyRelative("RotationY"));
        // 4. НОВОЕ: Stop On Match
        EditorGUI.PropertyField(new Rect(fieldsRect.x, fieldsRect.y + (lh + 2) * 3, fieldsRect.width, lh), property.FindPropertyRelative("StopOnMatch"));

        // ... Код отрисовки сетки остается без изменений ...
        SerializedProperty neighborsProp = property.FindPropertyRelative("Neighbors");
        if (neighborsProp.arraySize != 8) neighborsProp.arraySize = 8; // Страховка

        // Рисуем сетку 3x3
        // Индексы в массиве:
        // 0 1 2
        // 3 X 4
        // 5 6 7
        
        int index = 0;
        for (int y = 0; y < 3; y++) // Строки (Верх, Центр, Низ)
        {
            for (int x = 0; x < 3; x++) // Колонки
            {
                // Пропускаем центр (1,1)
                if (x == 1 && y == 1)
                {
                    Rect centerRect = new Rect(gridRect.x + x * CellSize, gridRect.y + y * CellSize, CellSize, CellSize);
                    DrawCenterIcon(centerRect);
                    continue;
                }

                Rect btnRect = new Rect(gridRect.x + x * CellSize, gridRect.y + y * CellSize, CellSize, CellSize);
                DrawStateButton(btnRect, neighborsProp.GetArrayElementAtIndex(index));
                index++;
            }
        }

        // Рамка вокруг сетки
        Handles.DrawSolidRectangleWithOutline(gridRect, Color.clear, Color.grey);

        EditorGUI.EndProperty();
    }

    private void DrawCenterIcon(Rect rect)
    {
        EditorGUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f)); // Темный фон
        // Можно нарисовать иконку или просто оставить пустым
    }
    
    private void DrawStateButton(Rect rect, SerializedProperty property)
    {
        NeighborState state = (NeighborState)property.enumValueIndex;
        Color oldColor = GUI.backgroundColor;
        string icon = "";

        switch (state)
        {
            case NeighborState.DontCare:
                GUI.backgroundColor = new Color(1f, 1f, 1f, 0.1f);
                break;
            case NeighborState.Present:
                GUI.backgroundColor = Color.green;
                icon = "✔";
                break;
            case NeighborState.Absent:
                GUI.backgroundColor = Color.red;
                icon = "✖";
                break;
        }

        if (GUI.Button(rect, icon))
        {
            property.enumValueIndex = (property.enumValueIndex + 1) % 3;
        }

        GUI.backgroundColor = oldColor;
        Handles.DrawSolidRectangleWithOutline(rect, Color.clear, Color.black);
    }
}
