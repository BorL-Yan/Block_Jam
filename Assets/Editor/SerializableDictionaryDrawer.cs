using Lib;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(SoundDictionary))] // Убедитесь, что класс SoundDictionary существует
public class SerializableDictionaryDrawer : PropertyDrawer
{
    private const float LineHeight = 20f;
    private const float Padding = 2f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 1. Обязательно обновляем данные объекта перед началом отрисовки
        property.serializedObject.Update();

        EditorGUI.BeginProperty(position, label, property);

        // Отрисовка заголовка
        Rect headerRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(headerRect, label, EditorStyles.boldLabel);

        var keys = property.FindPropertyRelative("keys");
        var values = property.FindPropertyRelative("values");

        // Кнопка добавления
        Rect buttonRect = new Rect(position.x, position.y + 20, position.width, 18);
        if (GUI.Button(buttonRect, "Add New Entry"))
        {
            keys.arraySize++;
            values.arraySize++;
        
            // 2. ВАЖНО: Сразу применяем изменения, иначе они сбросятся!
            property.serializedObject.ApplyModifiedProperties();
            return; // Выходим из метода, чтобы Unity перерисовала интерфейс с новым размером
        }

        float currentY = position.y + 45;

        for (int i = 0; i < keys.arraySize; i++)
        {
            Rect rowRect = new Rect(position.x, currentY, position.width, 18);
            float half = rowRect.width / 2;

            EditorGUI.PropertyField(new Rect(rowRect.x, rowRect.y, half - 5, 18), keys.GetArrayElementAtIndex(i), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rowRect.x + half, rowRect.y, half, 18), values.GetArrayElementAtIndex(i), GUIContent.none);

            currentY += 22;
        }

        EditorGUI.EndProperty();

        // 3. Сохраняем все остальные изменения (например, ввод текста в поля)
        property.serializedObject.ApplyModifiedProperties();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var keys = property.FindPropertyRelative("keys");
        // Считаем высоту: Заголовок + Кнопка Add + (Количество элементов * Высота строки) + Отступы
        return LineHeight + 25f + (keys.arraySize * LineHeight) + 10f;
    }
}
