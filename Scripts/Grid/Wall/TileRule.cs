using System;
using UnityEngine;

public enum NeighborState
{
    DontCare,   // (Пусто) Игнорировать
    Present,    // (Зеленая стрелка) Стена должна быть
    Absent      // (Красный крест) Стены быть НЕ должно
}

[Serializable]
public class TileRule
{
    public string Name;
    public GameObject Prefab;
    public int RotationY;
    
    [Tooltip("Если true, то после этого правила другие проверяться не будут.")]
    public bool StopOnMatch;
    
    // Массив из 8 соседей.
    // Порядок: 
    // 0:TopLeft, 1:Top, 2:TopRight, 
    // 3:Left,           4:Right, 
    // 5:BotLeft, 6:Bot, 7:BotRight
    [HideInInspector]
    public NeighborState[] Neighbors = new NeighborState[8];
    // Конструктор, чтобы массив не был null
    public TileRule()
    {
        Neighbors = new NeighborState[8];
    }
    
    // Проверка правила
    public bool CheckRule(bool[] currentNeighbors)
    {
        for (int i = 0; i < 8; i++)
        {
            
            NeighborState required = Neighbors[i];
            if (required == NeighborState.DontCare) 
                continue;
            bool isPresent = currentNeighbors[i];

            if (required == NeighborState.Present && !isPresent) return false;
            if (required == NeighborState.Absent && isPresent) return false;
        }
        return true;
    }
    
}
