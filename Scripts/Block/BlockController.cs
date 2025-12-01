using System;
using System.Collections.Generic;
using System.ComponentModel;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class BlockController : MonoBehaviour, IMovable, ISelect, IDisposable
{
    [field: SerializeField] public Transform Root;
    [SerializeField] public MeshRenderer _meshRenderer;
    [SerializeField] private List<BlockMaterial> _blockMaterials;
    [SerializeField] private GameObject _blockedObject;
    [SerializeField] private Collider _selectCollider;
    public Vector3 Position => Root.position;
    public Vector3 MergeControllerPoint { get; private set; }
    
    public Queue<Vector3> Points { get; set; }
    
    [SerializeField] private float _speed;
    
    private MovmentControl _movment;
    public BlockAction Action;
    
    private BlockStateMachine _stateMachine;
    [SerializeField, HideInInspector] public Cell Cell;
    
    
    [SerializeField] private BlockColor _blockColor;
    public BlockColor BlockColor => _blockColor;
    

    private void Awake()
    {
        _movment = new MovmentControl(this);
        _stateMachine = new BlockStateMachine(this);
        _stateMachine.Init(this);

        
        if( Action == null ) Action = new BlockAction();
    }

    public void Init(BlockColor blockColor, Cell cell)
    {
        _blockColor = blockColor;
        Cell = cell;
        foreach (var item in _blockMaterials)
        {
            if (item.blockColor == blockColor)
            {
                _meshRenderer.material = item.material;
                break;
            }
        }

        if (Cell != null)
        {
            Cell.BlockData.BlockColor = _blockColor;
        }
        
        _stateMachine.ChangeState<BlockDiactivateState>();
    }

    
    /// <summary>
    /// Инициализация в редакторе — безопасно!
    /// </summary>
    public void InitEditor(BlockColor blockColor, Cell cell, bool blocked)
    {
#if UNITY_EDITOR
        _blockedObject.SetActive(blocked);
        _selectCollider.enabled = !blocked;
        
        _blockColor = blockColor;
        Cell = cell;

        foreach (var item in _blockMaterials)
        {
            if (item.blockColor == blockColor)
            {
                _meshRenderer.sharedMaterial = item.material;
                break;
            }
        }
#endif
    }

    public void UpdateVisual(BlockColor blockColor)
    {
        _blockColor = blockColor;
        foreach (var item in _blockMaterials)
        {
            if (item.blockColor == blockColor)
            {
                _meshRenderer.material = item.material;
                return;
            }
        }
    }
    
    private void StartScene()
    {
        LevelController.Instance.AddBlock();
        _stateMachine.ChangeState<BlockDiactivateState>();

    }

    private void Update()
    {
       _stateMachine.Update(); 
    }

    private void FixedUpdate()
    {
        _stateMachine.FixedUpdate();
    }

    #region Move

    public void Move(Vector3 direction)
    {
        _movment.Move(direction, _speed);
    }

    public void Rotate(Vector3 direction)
    {
        _movment.Rotate(direction);
    }

    public void MoveOneShot(Vector3 direction)
    {
        _movment.MoveOneShut(direction);
    }

    #endregion

    public void Select()
    {
        //if(!_stateMachine.InState<BlockActivateState>()) return;
        
        Action.OnSelect?.Invoke();
    }

    public void BlockSelected()
    {
        Cell.BlockSelected();
    }

    public void ActivateBlockedBlock()
    {
        _blockedObject.SetActive(false);
        Cell.BlockData.BlockType = BlockType.Normal;
        _selectCollider.enabled = true;
        _stateMachine.ChangeState<BlockActivateState>();
    }
    
    public void DestroyBlock()
    {
        Destroy(Root.gameObject);
    }
    
    private void OnEnable()
    {
        LevelController.Instance.levelActions.OnStartLevel += StartScene;
    }

    private void OnDisable()
    {
        if(LevelController.Instance!= null)
            LevelController.Instance.levelActions.OnStartLevel -= StartScene;
    }
}

[Serializable]
public class BlockMaterial
{
    public Material material;
    public BlockColor blockColor;
}
