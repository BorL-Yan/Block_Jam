using System;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{
    private readonly GridController _grid;

    private PathNode[,] _nodes;
    private Heap<PathNode> _openSet;
    private HashSet<PathNode> _closedSet;
    
    private static readonly (int r, int c)[] Directions =
    {
        (1, 0), (-1, 0), (0, 1), (0, -1) // 4 направления
    };

    public PathFinder(GridController grid)
    {
        _grid = grid;

        int max = grid.Rows * grid.Columns;
        _openSet = new Heap<PathNode>(max);
        _closedSet = new HashSet<PathNode>();

        CreateNodes();
    }

    private void CreateNodes()
    {
        Debug.Log("Start Creating Nodes");
        _nodes = new PathNode[_grid.Rows, _grid.Columns];

        for (int r = 0; r < _grid.Rows; r++)
        {
            for (int c = 0; c < _grid.Columns; c++)
            {
                try
                {
                    _nodes[r, c] = new PathNode(_grid.GetCell(r, c));
                }
                catch (IndexOutOfRangeException e)
                {
                    Debug.Log(e.Message);
                }
            }
        }
    }
    
    private void ResetNodes()
    {
        _openSet.Clear();
        _closedSet.Clear();

        foreach (var node in _nodes)
            node.Reset();
    }
    
    
    public List<Cell> FindPathToBottomLine(Cell start)
    {
        List<Cell> best = null;
        int bestLen = int.MaxValue;

        int goalColum = 0;

        for (int r = 0; r < _grid.Rows; r++)
        {
            Cell target = _grid.GetCell(r, goalColum);

            if (target.CellType != CellType.Empty)
                continue;

            var p = FindPath(start, target);

            if (p != null && p.Count < bestLen)
            {
                best = p;
                bestLen = p.Count;
            }
        }

        return best;
    }
    
    public List<Cell> FindPath(Cell start, Cell target)
    {
        // Быстрый выход
        if (start == target)
        {
            return new List<Cell> { start };
        }

        // Быстрый выход, если цель недоступна
        if (target.CellType == CellType.Wall)
            return null;
        
        ResetNodes();

        PathNode startNode = _nodes[start.Row, start.Column];
        PathNode targetNode = _nodes[target.Row, target.Column];

        _openSet.Add(startNode);

        while (_openSet.Count > 0)
        {
            PathNode current = _openSet.RemoveFirst();
            _closedSet.Add(current);

            if (current == targetNode)
                return Retrace(startNode, current);

            foreach (var d in Directions)
            {
                int nr = current.Cell.Row + d.r;
                int nc = current.Cell.Column + d.c;

                if (!_grid.InBounds(nr, nc))
                    continue;

                Cell cell = _grid.GetCell(nr, nc);

                if (cell.CellType == CellType.Wall)
                    continue;

                if (cell.CellType == CellType.Block)
                    continue;

                PathNode neighbor = _nodes[nr, nc];

                if (_closedSet.Contains(neighbor))
                    continue;

                int g = current.gCost + 1;

                if (!_openSet.Contains(neighbor) || g < neighbor.gCost)
                {
                    neighbor.gCost = g;
                    neighbor.hCost = Manhattan(neighbor.Cell, target);

                    neighbor.Parent = current;

                    if (!_openSet.Contains(neighbor))
                        _openSet.Add(neighbor);
                    else
                        _openSet.UpdateItem(neighbor);
                }
            }
        }

        return null;
    }
    
    
    private int Manhattan(Cell a, Cell b)
    {
        return Mathf.Abs(a.Row - b.Row) + Mathf.Abs(a.Column - b.Column);
    }
    
    private List<Cell> Retrace(PathNode start, PathNode end)
    {
        List<Cell> path = new();
        PathNode current = end;

        while (current != start)
        {
            path.Add(current.Cell);
            current = current.Parent;
        }
        
        path.Add(start.Cell);
        path.Reverse();
        return path;
    }
    
    #region old
    
    // public List<Cell> FindPath(Cell start, Cell target)
    // {
    //     if (start == null || target == null)
    //         return null;
    //
    //     PathNode[,] nodes = new PathNode[_grid.Rows, _grid.Columns];
    //
    //     for (int r = 0; r < _grid.Rows; r++)
    //     {
    //         for (int c = 0; c < _grid.Columns; c++)
    //         {
    //             nodes[r, c] = new PathNode(_grid.GetCell(r, c));
    //         }
    //     }
    //
    //     List<PathNode> open = new List<PathNode>();
    //     HashSet<PathNode> closed = new HashSet<PathNode>();
    //
    //     PathNode startNode = nodes[start.Row, start.Column];
    //     PathNode targetNode = nodes[target.Row, target.Column];
    //
    //     open.Add(startNode);
    //
    //     while (open.Count > 0)
    //     {
    //         PathNode current = GetLowestFCost(open);
    //
    //         if (current == targetNode)
    //             return RetracePath(startNode, current);
    //
    //         open.Remove(current);
    //         closed.Add(current);
    //
    //         foreach (var d in Directions)
    //         {
    //             int nr = current.Cell.Row + d.r;
    //             int nc = current.Cell.Column + d.c;
    //
    //             if (!_grid.InBounds(nr, nc))
    //                 continue;
    //
    //             Cell neighborCell = _grid.GetCell(nr, nc);
    //
    //             // НЕпроходимые клетки
    //             if (neighborCell.CellType == CellType.Wall)
    //                 continue;
    //
    //             if (neighborCell.CellType == CellType.Block)
    //                 continue;
    //
    //             PathNode neighbor = nodes[nr, nc];
    //
    //             if (closed.Contains(neighbor))
    //                 continue;
    //
    //             int tentativeG = current.gCost + 1;
    //
    //             if (!open.Contains(neighbor) || tentativeG < neighbor.gCost)
    //             {
    //                 neighbor.gCost = tentativeG;
    //                 neighbor.hCost = Heuristic(neighbor.Cell, target);
    //                 neighbor.Parent = current;
    //
    //                 if (!open.Contains(neighbor))
    //                     open.Add(neighbor);
    //             }
    //         }
    //     }
    //
    //     return null; // путь не найден
    // }
    //
    //
    // public List<Cell> FindPathToBottomLine(Cell start)
    // {
    //     List<Cell> bestPath = null;
    //     int bestLength = int.MaxValue;
    //
    //     int lastRow = 0;
    //
    //     for (int c = 0; c < _grid.Columns; c++)
    //     {
    //         Cell target = _grid.GetCell(lastRow, c);
    //
    //         // проходимые типы
    //         if (target.CellType != CellType.Goal)
    //             continue;
    //
    //         List<Cell> path = FindPath(start, target);
    //
    //         if (path != null && path.Count < bestLength)
    //         {
    //             bestLength = path.Count;
    //             bestPath = path;
    //         }
    //     }
    //
    //     return bestPath;
    // }
    // private int Heuristic(Cell a, Cell b)
    // {
    //     // манхэттенская дистанция
    //     return Mathf.Abs(a.Row - b.Row) + Mathf.Abs(a.Column - b.Column);
    // }
    //
    // private PathNode GetLowestFCost(List<PathNode> list)
    // {
    //     PathNode best = list[0];
    //
    //     for (int i = 1; i < list.Count; i++)
    //     {
    //         if (list[i].fCost < best.fCost || 
    //             (list[i].fCost == best.fCost && list[i].hCost < best.hCost))
    //         {
    //             best = list[i];
    //         }
    //     }
    //
    //     return best;
    // }
    //
    // private List<Cell> RetracePath(PathNode start, PathNode end)
    // {
    //     List<Cell> path = new List<Cell>();
    //     PathNode current = end;
    //
    //     while (current != start)
    //     {
    //         path.Add(current.Cell);
    //         current = current.Parent;
    //     }
    //
    //     path.Reverse();
    //     return path;
    // }
    #endregion
}
