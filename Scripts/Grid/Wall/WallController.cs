using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WallController : MonoBehaviour
{
    
    [Header("8-Way Rule System")]
    [SerializeField] private List<TileRule> _rules = new List<TileRule>();
    [SerializeField] private GameObject _defaultPrefab;
    
    [HideInInspector] 
    [SerializeField] public List<GameObject> _walls = new();
    
    public int Row { get; set; }
    public int Column { get; set; }

    public void UpdateWall()
    {
        ClearOldWalls();
        
        if (GridController.Instance == null)
        {
            Debug.LogError("GridController instance not found! Make sure GridController is in the scene.");
            return;
        }
        
        GenerateTile();
    }
    
    private void ClearOldWalls()
    {
        foreach (var wall in _walls)
        {
            if (wall != null)
            {
                DestroyImmediate(wall.gameObject);
            }
        }
        _walls.Clear();
    }

    private void GenerateTile()
    {
        bool[] neighbors = GetNeighborsStatus(Row, Column);

        GameObject targetPrefab = _defaultPrefab;
        
        // 2. Ищем правило
        foreach (var rule in _rules)
        {
            if (rule.CheckRule(neighbors))
            {
                if (rule.Prefab != null)
                {
                    CreateObject(rule);
                }
                if (rule.StopOnMatch)
                {
                    break;
                }
            }
        }
        
    }
    private bool[] GetNeighborsStatus(int r, int c)
    {
        bool[] status = new bool[8];
        
        int[,] offsets = new int[,] {
            { 1, -1 }, { 1, 0 }, { 1, 1 },  // Top Row (0, 1, 2)
            { 0, -1 },           { 0, 1 },  // Mid Row (3, 4)
            {-1, -1 }, {-1, 0 }, {-1, 1 }   // Bot Row (5, 6, 7)
        };

        for (int i = 0; i < 8; i++)
        {
            int checkRow = r + offsets[i, 0];
            int checkCol = c + offsets[i, 1];
            status[i] = HasWall(checkRow, checkCol);
        }

        return status;
    }
    
    private bool HasWall(int r, int c)
    {
        Cell cell = GridController.Instance.GetCell(r, c);
        // Считаем стеной, если клетка существует и тип Wall
        // (Можешь инвертировать логику в зависимости от того, что тебе нужно)
        
        if(cell == null) return false;
        
        return cell.CellType == CellType.Wall;
    }
    
    private void CreateObject(TileRule rule) 
    {
#if UNITY_EDITOR
        GameObject obj = PrefabUtility.InstantiatePrefab(rule.Prefab) as GameObject;
#else
        GameObject obj = Instantiate(rule.Prefab);
#endif
        if (obj == null) return;

        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.zero; 
        obj.transform.localRotation = Quaternion.Euler(0, rule.RotationY, 0);
        obj.name = $"{rule.Name}_{Row}_{Column}";
        
        _walls.Add(obj);
    }
}