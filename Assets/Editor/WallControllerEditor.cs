using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WallController))]
public class WallControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Рисуем стандартный инспектор (поля переменных)
        DrawDefaultInspector();

        WallController script = (WallController)target;

        GUILayout.Space(10);
        GUILayout.Label("Actions", EditorStyles.boldLabel);

        // Кнопка для ручного обновления
        if (GUILayout.Button("Update Wall Geometry"))
        {
            // Регистрируем действие для Ctrl+Z (Undo)
            Undo.RegisterCompleteObjectUndo(script, "Update Wall");
            script.UpdateWall();
            
            // Помечаем сцену как "грязную", чтобы изменения сохранились
            EditorUtility.SetDirty(script);
        }
    }
}
