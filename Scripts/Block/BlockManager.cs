using System.Collections.Generic;
using Lib;


public class BlockManager : SingletonSceneAutoCreated<BlockManager>
{
    private List<BlockController> _blockControllers = new List<BlockController>();
    
    public void AddBlockController(BlockController blockController)
    {
        _blockControllers.Add(blockController);
    }
    
    
    
}
