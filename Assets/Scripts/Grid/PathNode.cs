
public class PathNode : IHeapItem<PathNode>
{
    public Cell Cell;

    public int gCost; 
    public int hCost; 
    public int fCost => gCost + hCost;

    public PathNode Parent;
    public int HeapIndex { get; set; }

    public PathNode(Cell cell)
    {
        Cell = cell;
    }

    public void Reset()
    {
        gCost = 0;
        hCost = 0;
        HeapIndex = 0;
        Parent = null;
    }

    public int CompareTo(PathNode other)
    {
        int compare = fCost.CompareTo(other.fCost);
        if(compare == 0)
            compare = hCost.CompareTo(other.hCost);
        
        return compare;
    }
}
