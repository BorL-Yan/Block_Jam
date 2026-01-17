
using System.Collections.Generic;
using UnityEngine;

public class BlockAniamtionController : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private Dictionary<BlockAnimationType, string> _blockAniamtions;


    private void Awake()
    {
        _blockAniamtions = new Dictionary<BlockAnimationType, string>();
        foreach (BlockAnimationType type in System.Enum.GetValues(typeof(BlockAnimationType)))
        {
            _blockAniamtions[type] = type.ToString();
        }
    }
    
    public void Play(BlockAnimationType animType)
    {
        if (_blockAniamtions.TryGetValue(animType, out string name))
        {
            _animator.Play(name,0,0);
        }
    }
}

public enum BlockAnimationType: byte
{
    Activate,
    Diactivate,
    Hello,
    Idle,
    
    Final_Win_Left,
    Final_Win_Midle,
    Final_Win_Right,
    
    Merge_Left,
    Merge_Midle,
    Merge_Right,
    
    Run_Activate,
    Run,
    
    Jump,
}
