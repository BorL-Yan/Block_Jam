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

    public event Action BLockActivate; 
    
    private void Start()
    {
        if (BlockData != null)
        {
            BlockData.RemoveFirstBlock();
        }
        if(ConnectedTube != null) ConnectedTube.UpdateTubeText();
    }


    private void UpdateNeighborBlocks()
    {
        List<Cell> neighbors = Grid.GetNeighborCells(Row, Column);
        BLockActivate?.Invoke();
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
                UpdateNeighborBlocks();
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
                        var block = Instantiate(LevelController.Instance.BlockPrefab).GetComponent<BlockController>();
                        block.transform.position = ConnectedTube.transform.position;
                        block.MoveToCell((transform.position - ConnectedTube.transform.position).normalized);
                        block.name = $"Block : {this.name}";
                        
                        ConnectedTube.Activate();
                    
                        if (block != null)
                        {
                            // Передаем 'this', если пуле нужно знать, кто её родил, или null/target
                            block.Init(newColor, this); 
                        }

                        BlockController = block;
                    }
                    else
                    {
                        Debug.LogWarning("Block Prefab is null");
                    }
                    
                }
                else
                {
                    CellType = CellType.Empty;
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
