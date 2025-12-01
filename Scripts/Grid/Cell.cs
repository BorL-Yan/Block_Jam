using System;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.Serialization;

public class Cell : MonoBehaviour
{
    [SerializeField] public int Row;
    [SerializeField] public int Column;
    
    [SerializeField] public CellType CellType;
    [SerializeField] public Block BlockData;
    // Данные для Трубы
    [SerializeField] public TubeDirection CurrentTubeDirection;
    
    public BlockController BlockController;
    public WallController WallController;
    public TubeController TubeController;
    
    [SerializeField] public TubeController ConnectedTube;
    
    [HideInInspector] public GridController Grid;

    private void Start()
    {
        if (BlockData != null)
        {
            BlockData.RemoveFirstBlock();
        }
    }


    private void UpdateNeighborBlocks()
    {
        List<Cell> neighbors = Grid.GetNeighborCells(Row, Column);

        foreach (var item in neighbors)
        {
            if (item.BlockData.BlockType == BlockType.Blocked)
            {
                item.BlockController.ActivateBlockedBlock();
            }
        }
    }

    public void BlockSelected()
    {
        switch (BlockData.BlockType)
        {
            case BlockType.Normal:
            {
                UpdateNeighborBlocks();
                CellType = CellType.Empty;
                break;
            }
            case BlockType.List:
            {
                
                var items = BlockData.Items;
                if (items == null)
                {
                    Debug.LogWarning("Items is null");
                    return;
                }
                if (items.Count > 0)
                {
                    BlockColor newColor = BlockData.GetBlockInList();
                    if (LevelController.Instance.BlockPrefab != null)
                    {
                        var instance = Instantiate(LevelController.Instance.BlockPrefab);
                        instance.transform.position = transform.position;
                        instance.name = $"Block : {this.name}";
                        
                        BlockController spawnedBlock = instance.GetComponent<BlockController>();
                    
                        if (spawnedBlock != null)
                        {
                            // Передаем 'this', если пуле нужно знать, кто её родил, или null/target
                            spawnedBlock.Init(newColor, this); 
                        }

                        BlockController = spawnedBlock;
                    }
                    else
                    {
                        Debug.LogWarning("Block Prefab is null");
                    }
                    
                }
                if(ConnectedTube != null)
                    ConnectedTube.UpdateTubeText();
                else
                {
                    Debug.LogWarning("Tube Controller is null");
                }
                break;
            }
        }
    }
   
    public void UpdatePosition()
    {
#if UNITY_EDITOR
        if (WallController != null) WallController.transform.position = transform.position;
        if (BlockController != null) BlockController.transform.position = transform.position;
        if (TubeController != null) TubeController.transform.position = transform.position;
#endif
    }
    
    public void ClearContent()
    {
#if UNITY_EDITOR
        DestroyCell();
        BlockData = null;
        CellType = CellType.Empty;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
    
    public void DestroyCell()
    {
#if UNITY_EDITOR
        if (WallController != null) DestroyImmediate(WallController.gameObject);
        if (BlockController != null) DestroyImmediate(BlockController.gameObject);
        if (TubeController != null) DestroyImmediate(TubeController.gameObject);
#endif
    }

}

public enum CellType
{
    Empty,
    Wall,
    Block,
    Tube,
}
