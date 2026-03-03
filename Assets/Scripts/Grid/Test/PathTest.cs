using System.Collections.Generic;
using UnityEngine;

public class PathTest : MonoBehaviour
{
    public GridController Grid;
    public Cell start;
    public Cell target;

    private void Start()
    {
        PathFinder finder = new PathFinder(Grid);
        
        
        List<Cell> path = finder.FindPathToBottomLine(start);
        Debug.Log(path.Count);

        if (path != null)
        {
            foreach (var cell in path)
                Debug.Log($"Path → {cell.Row},{cell.Column}");
        }
        else
        {
            Debug.Log("Путь не найден");
        }
    }
}