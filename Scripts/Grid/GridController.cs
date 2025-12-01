using System;
using System.Collections.Generic;
using System.Linq;
using Game;
using Lib;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class GridController : SingletonScene<GridController>
{
    // Используем public поле для назначения родителя в инспекторе
    public GameObject BlockParentObject;
    public GameObject CellParentObject;
    
    // Приватные поля, которые теперь корректно обрабатываются SerializedProperty
    [SerializeField, Min(1)] private int _columns = 1;
    [SerializeField, Min(1)] private int _rows = 1;
    
    // Public свойства для доступа
    public int Rows => _rows;
    public int Columns => _columns;

    private PathFinder pathFinder;
    public PathFinder PathFinder => pathFinder;
    
    [SerializeField] private GameObject CellPrefab;

    // Список для сериализации/сохранения
    [SerializeField] private List<Cell> _savedCells = new List<Cell>();
    
    private Cell[,] _grid = new Cell[0, 0];
    
    // Предыдущие значения для отслеживания изменений в редакторе
    private int _prevRows; 
    private int _prevColumns;
    
    [Header("Grass Transforing Objects")]
    [SerializeField] private GameObject _grassLeft;
    [SerializeField] private GameObject _grassRight;
    [SerializeField] private GameObject _grassUp;


    public bool Created { get; private set; }

    private void OnEnable()
    {
        _prevRows = _rows;
        _prevColumns = _columns;
        
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            // Убеждаемся, что BlockParentObject существует
            if (CellParentObject == null)
            {
                var go = new GameObject("Cell Contents");
                go.transform.SetParent(this.transform);
                CellParentObject = go;
            }
            
            // загружаем сетку из сохранённых клеток
            RebuildFromSavedCells();
        }
#endif
        if (Application.isPlaying)
        {
            _grid = new Cell[_rows, _columns];

            // Защита: проверяем, совпадает ли размер сохраненных данных с настройками
            if (_savedCells.Count < _rows * _columns)
            {
                Debug.LogError($"Ошибка! В _savedCells {_savedCells.Count} элементов, а нужно {_rows * _columns}.");
                return;
            }
            for (int r = 0; r < _rows; r++)
            {
                for (int c = 0; c < _columns; c++)
                {
                    // Формула перевода координат 2D -> 1D
                    int index = r * _columns + c;

                    // Копируем (или создаем новый, если Cell - ссылочный тип, лучше клонировать)
                    _grid[r, c] = _savedCells[index]; 
                }
            }
            pathFinder = new PathFinder(this);
        }
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Application.isPlaying) return;

        // Проверяем изменения в редакторе
        if (_rows != _prevRows || _columns != _prevColumns)
        {
            _prevRows = _rows;
            _prevColumns = _columns;

            RebuildGrid();
        }
#endif
    }
    
    public Cell GetCell(int r, int c)
    {
        if (_grid == null && !InBounds(r,c)) return null;
        
        try
        {
            return _grid[r, c];
        }
        catch (IndexOutOfRangeException e)
        {
            Debug.Log($"Grid Size. Rows : {_rows}, Colums :  {_columns}");
            Debug.Log($"GetCell({r}, {c}), Error: {e.Message}");
        }
        return null;
    }

    public bool InBounds(int r, int c)
    {
        return r >= 0 && r < _rows && c >= 0 && c < _columns;
    }
    

    // ------------------------------------------------------------
    //  ВОССТАНОВЛЕНИЕ СЕТКИ ИЗ _savedCells
    //  (Вызывается только при загрузке компонента в редакторе)
    // ------------------------------------------------------------
    private void RebuildFromSavedCells()
    {
        // Временно устанавливаем размеры, исходя из максимальных Row/Column в сохраненных ячейках
        int maxR = _rows;
        int maxC = _columns;

        // Если сохраненные ячейки существуют, убедимся, что размеры достаточно велики
        if (_savedCells.Count > 0)
        {
             foreach (var cell in _savedCells)
             {
                 if (cell != null)
                 {
                     maxR = Mathf.Max(maxR, cell.Row + 1);
                     maxC = Mathf.Max(maxC, cell.Column + 1);
                 }
             }
        }
        
        // Обновляем приватные поля, если они меньше. 
        // Это предотвращает потерю данных при загрузке
        if (_rows < maxR || _columns < maxC)
        {
             _rows = maxR;
             _columns = maxC;
             _prevRows = _rows;
             _prevColumns = _columns;
             
             // Нужен RebuildGrid, если размеры изменились
             RebuildGrid();
             return;
        }

        // Если размеры не изменились и ячейки загружены
        RebuildGrid();
    }
    
    public void RebuildGrid() // Сделал public, чтобы вызывать из редактора
    {
#if UNITY_EDITOR

        if (CellPrefab == null)
        {
            Debug.LogError("CellPrefab не назначен!");
            return;
        }

        // Если BlockParentObject не установлен, используем transform
        Transform parentTransform = CellParentObject != null ? CellParentObject.transform : transform;
        
        // старые значения
        int oldRows = _grid.GetLength(0);
        int oldCols = _grid.GetLength(1);

        Cell[,] newGrid = new Cell[_rows, _columns];
        
        // Создадим словарь для быстрого доступа к существующим ячейкам
        Dictionary<Tuple<int, int>, Cell> existingCells = new Dictionary<Tuple<int, int>, Cell>();
        
        // Заполняем словарь из _grid и заодно из _savedCells, 
        // чтобы избежать потери данных, если _grid пуст после перезагрузки
        IEnumerable<Cell> cellsToProcess;

        // Если _grid не пуст, используем его (плоское представление). Иначе используем _savedCells.
        if (_grid.Length > 0)
        {
            // Преобразование двумерного массива в плоскую коллекцию с помощью LINQ
            cellsToProcess = _grid.Cast<Cell>(); 
        }
        else
        {
            cellsToProcess = _savedCells;
        }
        foreach (var cell in cellsToProcess)
        {
            if (cell != null)
            {
                existingCells[new Tuple<int, int>(cell.Row, cell.Column)] = cell;
            }
        }


        // 🔷 1. Создание или перенос/удаление существующих клеток
        for (int r = 0; r < _rows; r++)
        {
            for (int c = 0; c < _columns; c++)
            {
                Cell cell;
                Tuple<int, int> key = new Tuple<int, int>(r, c);

                if (existingCells.TryGetValue(key, out cell))
                {
                    // Клетка существует и находится в новых границах
                    cell.Row = r;
                    cell.Column = c;
                    cell.Grid = this; // Обновляем ссылку на себя
                    cell.transform.SetParent(parentTransform);
                    cell.transform.localPosition = new Vector3(r, 0, c);
                    newGrid[r, c] = cell;
                    existingCells.Remove(key); // Удаляем из словаря, чтобы знать, что осталось
                }
                else if (r < oldRows && c < oldCols && _grid.Length > 0)
                {
                    // Ячейка была в старых границах, но не в словаре (случай, когда Grid не пуст)
                    // Это может быть дублирующим кодом, но обеспечивает консистентность
                    // (Обычно здесь ничего не должно происходить, если мы используем existingCells)
                }
                else
                {
                    // Создаём новую
                    GameObject obj = PrefabUtility.InstantiatePrefab(CellPrefab) as GameObject;
                    obj.name = $"Cell {r},{c}";
                    obj.transform.SetParent(parentTransform);
                    obj.transform.localPosition = new Vector3(r, 0, c);
                    obj.SetActive(true);

                    Cell newCell = obj.GetComponent<Cell>();
                    if (newCell != null)
                    {
                        newCell.Row = r;
                        newCell.Column = c;
                        newCell.Grid = this;
                        newCell.CellType = CellType.Empty;
                        newGrid[r, c] = newCell;
                    }
                    else
                    {
                        DestroyImmediate(obj);
                    }
                }
                
                if(cell != null) cell.UpdatePosition();
            }
        }
        
        // 🔷 2. Удаляем клетки, которые вышли за границы (остались в existingCells)
        foreach (var remainingCell in existingCells.Values)
        {
            if (remainingCell != null && remainingCell.gameObject != null)
            {
                remainingCell.DestroyCell();
                DestroyImmediate(remainingCell.gameObject);
            }
        }

        _grid = newGrid;
        
        // 🔷 3. Обновляем сериализуемый список
        _savedCells.Clear();
        foreach (var cell in _grid)
        {
            if(cell != null)
                _savedCells.Add(cell);
        }
        
        // Обновление позиции
        
        this.transform.position = new Vector3((float)-_rows / 2 + 0.5f, this.transform.position.y, this.transform.position.z);
        CalculateGrassTransform();
        Created = true;
        // Принудительное обновление инспектора
        EditorUtility.SetDirty(this);
#endif
    }

    private void CalculateGrassTransform()
    {
        if(_grassLeft == null || _grassRight == null || _grassUp == null) return;
        _grassLeft.transform.position = new Vector3((float)-_rows / 2 + 0.5f, 0, 0);
        _grassRight.transform.position = new Vector3((float)_rows / 2 - 0.5f, 0, 0);
        _grassUp.transform.position = new Vector3(0,0, (float)_columns - 0.5f);
    }
  
    private int CalculateShift(int oldSize, int newSize)
    {
        // Базовый сдвиг (целочисленное деление)
        int diff = newSize - oldSize;
        int shift = diff / 2;

        // Коррекция для нечетных изменений
        if (diff % 2 != 0) 
        {
            // Если мы РАСТЕМ (diff > 0) и новый размер НЕЧЕТНЫЙ (3, 5...), добавляем сдвиг (+1), 
            // чтобы новая линия появилась слева.
            if (newSize > oldSize && newSize % 2 != 0)
            {
                shift += 1;
            }
            // Если мы УМЕНЬШАЕМСЯ (diff < 0) и новый размер ЧЕТНЫЙ (2, 4...), уменьшаем сдвиг (-1),
            // чтобы обрезка произошла слева (восстанавливая центр).
            else if (newSize < oldSize && newSize % 2 == 0)
            {
                shift -= 1;
            }
        }
        
        return shift;
    }

    
    public void ClearGridContent()
    {
#if UNITY_EDITOR
        if (_grid == null) return;

        foreach (var cell in _grid)
        {
            if (cell != null)
            {
                // Вызываем очистку содержимого, но НЕ удаляем саму клетку
                cell.ClearContent();
            }
        }
    
        // Обновляем список сохраненных данных, чтобы там тоже обновились типы на Empty
        _savedCells.Clear();
        foreach (var cell in _grid)
        {
            if(cell != null) _savedCells.Add(cell);
        }

        Debug.Log("Уровень очищен: Сетка осталась, блоки и стены удалены.");
        EditorUtility.SetDirty(this);
#endif
    }
    
    public void SetBordersToWalls()
    {
#if UNITY_EDITOR
        // Загружаем префаб стены, так как GridController должен уметь его создавать
        GameObject wallPrefab = Resources.Load<GameObject>("Prefab/Wall");
        if (wallPrefab == null)
        {
            Debug.LogError("Не найден префаб стены в Resources/Prefab/Wall");
            return;
        }

        for (int r = 0; r < _rows; r++)
        {
            for (int c = 0; c < _columns; c++)
            {
                // Проверка: является ли клетка крайней
                if (r == 0 || r == _rows - 1 || c == 0 || c == _columns - 1)
                {
                    Cell cell = _grid[r, c];
                    if (cell == null) continue;

                    // Если это уже стена, пропускаем
                    if (cell.CellType == CellType.Wall) continue;

                    // Меняем тип
                    cell.CellType = CellType.Wall;

                    // 1. Удаляем блок, если он был
                    if (cell.BlockController != null)
                    {
                        DestroyImmediate(cell.BlockController.gameObject);
                        cell.BlockController = null;
                        cell.BlockData = null;
                    }

                    // 2. Создаем стену, если её нет
                    if (cell.WallController == null)
                    {
                        var wallObj = PrefabUtility.InstantiatePrefab(wallPrefab) as GameObject;
                    
                        if(wallObj == null) Debug.LogError("Wall Object is null");
                        
                        // Используем BlockParentObject если есть, иначе родителя клетки
                        Transform parent = BlockParentObject != null ? BlockParentObject.transform : cell.transform;
                    
                        wallObj.transform.position = cell.transform.position;
                        wallObj.gameObject.transform.SetParent(parent);
                        wallObj.name = $"Wall_{r}_{c}";
                        cell.WallController = wallObj.GetComponent<WallController>();
                        
                        cell.WallController.Row = r;
                        cell.WallController.Column = c;
                    }
                
                    EditorUtility.SetDirty(cell);
                }
            }
        }

#endif
    }

    public void BevelCalculate()
    {
        foreach (var cell in _grid)
        {
            if (cell != null && cell.WallController != null)
            {
                cell.WallController.UpdateWall();
            }
        }
    }
    
    public List<Cell> GetNeighborCells(int row, int column)
    {
        List<Cell> cells = new List<Cell>();
        int[] dRow = { -1, 1, 0, 0 };
        int[] dCol = { 0, 0, -1, 1 };

        for (int k = 0; k < 4; k++)
        {
            int checkRow = row + dRow[k];
            int checkCol = column + dCol[k];

            // Проверка границ
            if (InBounds(checkRow, checkCol))
            {
                // Убедимся, что клетка существует (если grid разрежен)
                if (_grid[checkRow, checkCol] != null)
                {
                    cells.Add(_grid[checkRow, checkCol]);
                }
            }
        }
        return cells;
    }
    
}