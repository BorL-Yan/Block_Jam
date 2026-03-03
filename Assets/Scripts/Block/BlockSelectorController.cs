using UnityEngine;


public class BlockSelectorController : MonoBehaviour, ISelect 
{
    [SerializeField] private BlockController _blockController;
    public void Select()
    {
        _blockController.Select();
    }
}
