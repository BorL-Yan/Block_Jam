using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Cell))]
[CanEditMultipleObjects]
public class CellEditor : Editor
{
    private GameObject _blockPrefab;
    private GameObject _wallPrefab;
    private GameObject _tubePrefab;

    private SerializedProperty _cellTypeProp;
    private SerializedProperty _blockDataProp;
    private SerializedProperty _tubeDirProp;
    
    private void OnEnable()
    {
        // Загружаем префаб 1 раз
        _blockPrefab = Resources.Load<GameObject>("Prefab/BlockBase");
        _wallPrefab = Resources.Load<GameObject>("Prefab/Wall");
        _tubePrefab = Resources.Load<GameObject>("Prefab/Tube");

        if (_blockPrefab == null) Debug.LogError("❌ Не найден Resources/Prefab/BlockBase"); 
        if (_wallPrefab == null) Debug.LogError("❌ Не найден Resources/Prefab/Wall");
        if (_tubePrefab == null) Debug.LogError("❌ Не найден Resources/Prefab/Tube");
        
        _cellTypeProp = serializedObject.FindProperty("CellType");
        _blockDataProp = serializedObject.FindProperty("BlockData");
        _tubeDirProp = serializedObject.FindProperty("CurrentTubeDirection");
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();
        
        EditorGUILayout.PropertyField(_cellTypeProp);

        bool dataChanged = serializedObject.ApplyModifiedProperties();
        CellType currentType = (CellType)_cellTypeProp.intValue;
        
        // --- БЛОК -------------------------
        if (!_cellTypeProp.hasMultipleDifferentValues && currentType == CellType.Block)
        {
            DrawBlockData();
            if (serializedObject.ApplyModifiedProperties()) dataChanged = true;
        }
        
        // --- ТРУБА (TUBE) -----------------
        if (!_cellTypeProp.hasMultipleDifferentValues && currentType == CellType.Tube)
        {
            if (serializedObject.ApplyModifiedProperties()) dataChanged = true;
        }
        
        // --- ОБНОВЛЕНИЕ ВИЗУАЛА ----------
        if (dataChanged || GUI.changed)
        {
            foreach (var obj in targets)
            {
                Cell cell = (Cell)obj;
                UpdateCellVisuals(cell);
            }
        }
        
    }
    private void UpdateCellVisuals(Cell cell)
    {
        // Сначала логика удаления лишнего
        if (cell.CellType != CellType.Block)
        {
            cell.DestroyCell();
            DeleteTube(cell);
        }
        if (cell.CellType != CellType.Wall) DeleteWall(cell);
        if(cell.CellType != CellType.Tube && cell.BlockData.BlockType != BlockType.List) DeleteTube(cell);

        // Потом логика создания нужного
        switch (cell.CellType)
        {
            case CellType.Block:
                if (cell.BlockData == null) cell.BlockData = new Block();
                EnsureBlockInstance(cell);
                break;

            case CellType.Wall:
                EnsureWallInstance(cell);
                break;
                
            case CellType.Tube:
                EnsureTubeInstance(cell);
                break;
        }
        
        // Помечаем объект как "грязный", чтобы сцена сохранилась
        EditorUtility.SetDirty(cell);
    }
    
    
    // --------------------------------------------------------------------------------
    //  UI ДЛЯ BLOCK DATA
    // --------------------------------------------------------------------------------

    private void DrawBlockData()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Block Settings", EditorStyles.boldLabel);

        SerializedProperty blockType = _blockDataProp.FindPropertyRelative("BlockType");
        SerializedProperty blockColor = _blockDataProp.FindPropertyRelative("BlockColor");
        SerializedProperty items = _blockDataProp.FindPropertyRelative("Items");
        
        // Получаем доступ к классу Block напрямую для удобной работы со списком
        Cell cell = (Cell)target;

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(blockType);
        bool typeChanged = EditorGUI.EndChangeCheck();

        BlockType currentType = (BlockType)blockType.intValue;
        
        // --- ЛОГИКА ДЛЯ СПИСКА (LIST) ---
        if (currentType == BlockType.List)
        {
            if (!Application.isPlaying && targets.Length == 1)
            {
                EnsureBlockInstance(cell);
                
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Pipe Settings", EditorStyles.boldLabel);
                
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(_tubeDirProp);
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties(); // Сохраняем направление перед перемещением
                    MoveTube(cell, (TubeDirection)_tubeDirProp.intValue);
                }
            }
            EditorGUILayout.PropertyField(items, true);
        }
        // --- ОБЫЧНЫЙ БЛОК ---
        else if (currentType == BlockType.Normal || currentType == BlockType.Blocked)
        {
            EditorGUILayout.PropertyField(blockColor);
            if (targets.Length == 1) EnsureBlockInstance(cell);
            
            if(cell.ConnectedTube != null) DeleteTube(cell);
        }
        
        serializedObject.ApplyModifiedProperties();
    }
    
    // ----------------------------------------------------------------------
    // ЛОГИКА ТРУБЫ
    // ----------------------------------------------------------------------

    private void MoveTube(Cell cell, TubeDirection newDir)
    {
        foreach (TubeDirection dir in System.Enum.GetValues(typeof(TubeDirection)))
        {
            if (dir == newDir) continue; // Новое направление не трогаем пока

            Cell neighbor = GetNeighbor(cell, dir);
            // Если сосед - труба, удаляем её
            if (neighbor != null && neighbor.CellType == CellType.Tube)
            {
                Undo.RecordObject(neighbor, "Clear Old Tube");
                neighbor.CellType = CellType.Empty; 
            
                // Принудительно обновляем соседа, чтобы он удалил свой TubeController
                UpdateCellVisuals(neighbor); 
            }
        }

        // 2. СОЗДАНИЕ НОВОГО
        Cell newNeighbor = GetNeighbor(cell, newDir);
    
        if (newNeighbor != null)
        {
            if (newNeighbor.CellType != CellType.Empty)
            {
                Debug.LogWarning($"⚠️ Ячейка {newDir} не пуста, перезаписываем...");
            }

            // Вызываем метод, который настроит соседа и свяжет нас с ним
            CreateTubeAt(cell, newNeighbor, newDir);
        }
        else
        {
            Debug.LogError($"❌ Граница сетки, трубу ставить некуда.");
        }
    }
    
    private void CreateTubeAt(Cell originCell, Cell targetCell, TubeDirection direction)
    {
        Undo.RecordObject(targetCell, "Set Tube Type");
        targetCell.CellType = CellType.Tube;
    
        // Это создаст TubeController НА targetCell и запишет его в targetCell.TubeController
        UpdateCellVisuals(targetCell); 
    
        // Поворачиваем трубу (она уже создана вызовом выше)
        if (targetCell.TubeController != null)
        {
            Undo.RecordObject(targetCell.TubeController.transform, "Rotate Tube");
            targetCell.TubeController.SetRotation(direction);
            targetCell.TubeController.Connect(originCell);
            EditorUtility.SetDirty(targetCell.TubeController);
            
            // 2. Связываем БЛОК (Origin) с новой трубой
            Undo.RecordObject(originCell, "Link Tube");
            originCell.CurrentTubeDirection = direction; 
            originCell.ConnectedTube = targetCell.TubeController; 

            EditorUtility.SetDirty(targetCell);
            EditorUtility.SetDirty(originCell);
        }
    }
    
    private Cell GetNeighbor(Cell origin, TubeDirection dir)
    {
        if (origin.Grid == null) return null;

        int r = origin.Row;
        int c = origin.Column;

        switch (dir)
        {
            case TubeDirection.Left:  r -= 1; break;
            case TubeDirection.Right: r += 1; break;
            case TubeDirection.Up:    c += 1; break; 
            case TubeDirection.Down:  c -= 1; break;
        }
        
        return origin.Grid.GetCell(r, c); 
    }
    

    #region Instance
    
    private void EnsureBlockInstance(Cell cell)
    {
        if (_blockPrefab == null) return;

        // 1. Сначала гарантируем наличие GameObject
        if (cell.BlockController == null)
        {
            // Пытаемся найти существующий
            cell.BlockController = cell.GetComponentInChildren<BlockController>(); 
            
            // Если совсем нет - создаем
            if (cell.BlockController == null)
            {
                var instance = (GameObject)PrefabUtility.InstantiatePrefab(_blockPrefab);
                
                Transform parentTransform = cell.Grid != null && cell.Grid.BlockParentObject != null 
                    ? cell.Grid.BlockParentObject.transform 
                    : cell.transform.parent;

                cell.BlockController = instance.GetComponent<BlockController>();
                
                instance.transform.SetParent(parentTransform);
                instance.transform.position = cell.transform.position;
                instance.name = $"Block : {cell.name}";

            }
        }

        // 2. Теперь настраиваем его в зависимости от данных
        if (cell.BlockController != null)
        {
            BlockType type = cell.BlockData.BlockType;

            if (type == BlockType.List)
            {
                // Если список не пуст — показываем первый элемент
                if (cell.BlockData.Items != null && cell.BlockData.Items.Count > 0)
                {
                    cell.BlockController.InitEditor(cell.BlockData.Items[0], cell, false);
                    // Включаем renderer, если он был выключен
                    cell.BlockController.gameObject.SetActive(true);
                }
                else
                {
                    // Если список пуст, мы НЕ удаляем CellType, мы просто скрываем визуальный объект
                    // или уничтожаем только GO, но не меняем CellType
                    DestroyImmediate(cell.BlockController.gameObject);
                    cell.BlockController = null;
                    // ВАЖНО: Не вызываем DeleteBlock(cell), чтобы не сбросить CellType в Empty
                    return;
                }
            }
            else
            {
                // Normal или Blocked
                bool blocked = (type == BlockType.Blocked);
                cell.BlockController.InitEditor(cell.BlockData.BlockColor, cell, blocked);
                cell.BlockController.gameObject.SetActive(true);
            }
            
            if (cell.BlockController != null)
            {
                EditorUtility.SetDirty(cell.BlockController);
            }
        }
    }
    
    private void EnsureWallInstance(Cell cell)
    {
        if (_wallPrefab == null) return;

        if (cell.WallController == null)
        { 
            var instance = PrefabUtility.InstantiatePrefab(_wallPrefab) as GameObject;
                
            Transform parentTransform = cell.Grid != null && cell.Grid.BlockParentObject != null 
                ? cell.Grid.BlockParentObject.transform 
                : cell.transform.parent;

            instance.transform.SetParent(parentTransform);
            instance.transform.position = cell.transform.position;
            instance.name = $"Wall : {cell.name}";
            WallController wallController = instance.GetComponent<WallController>();
            wallController.Row = cell.Row;
            wallController.Column = cell.Column;
            cell.WallController = wallController;
        }
    }
    
    private void EnsureTubeInstance(Cell cell)
    {
        if (_tubePrefab == null) return;

        // cell - это Ячейка-Труба (бывший targetCell)
        if (cell.TubeController == null)
        {
            cell.TubeController = cell.GetComponent<TubeController>();

            if (cell.TubeController == null)
            {
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(_tubePrefab);
            
                // Родитель - либо общий контейнер, либо сама ячейка
                Transform parentTransform = cell.Grid != null && cell.Grid.BlockParentObject != null 
                    ? cell.Grid.BlockParentObject.transform 
                    : cell.transform.parent;

                instance.transform.SetParent(parentTransform);
                instance.transform.position = cell.transform.position; // Позиция самой ячейки
                instance.name = $"Tube : {cell.name}";
            
                cell.TubeController = instance.GetComponent<TubeController>();
            }
        }
    }


    #endregion


    #region Destroy

    private void DeleteBlock(Cell cell)
    {
        if (cell.BlockController != null)
        {
            DestroyImmediate(cell.BlockController.gameObject);
            cell.BlockController = null;
        }
    }
    
    private void DeleteWall(Cell cell)
    {
        if (cell.WallController != null)
        {
            DestroyImmediate(cell.WallController);
            cell.WallController = null;
        }
    }
    
    private void DeleteTube(Cell cell)
    {
        if (cell.TubeController != null)
        {
            DestroyImmediate(cell.TubeController.gameObject);
            cell.TubeController = null;
        }
        if (cell.ConnectedTube != null)
        {
            DestroyImmediate(cell.ConnectedTube.gameObject);
            cell.ConnectedTube = null;
        }
    }
    #endregion
    
}
