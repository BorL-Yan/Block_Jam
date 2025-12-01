using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Block
{
    [SerializeField] public BlockType BlockType;
    [SerializeField] public BlockColor BlockColor;

    [SerializeField] public List<BlockColor> Items = new();


    public void RemoveFirstBlock()
    {
        if(Items.Count > 0)
            Items.RemoveAt(0);
    }
    public BlockColor GetBlockInList()
    {
        var item = Items[0];
        Items.RemoveAt(0);
        return item;
    }
    
}

[Serializable]
public enum BlockType
{
    Normal,
    Blocked,
    List,
}


[Serializable]
public enum BlockColor
{
    Red,
    Blue,
    Yellow,
    Purple,
}