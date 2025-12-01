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

    [SerializeField] private TrailRenderer _trailRenderer;
    [SerializeField] private ParticleSystem _trailParticleSystem;
    
    [field: SerializeField ] public AnimationCurve _mergeAniamtionCurve;
    
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
        DiactivateTrailRenderer();
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

    public void JumpAndMerge(int index)
    {
        _movment.Jump(index, DestroyBlock);
        
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

    public void DiactivateTrailRenderer()
    {
        _trailRenderer.Clear();
        _trailRenderer.gameObject.SetActive(false);
        _trailParticleSystem.Clear();
        _trailParticleSystem.gameObject.SetActive(false);
    }

    public void ActivateTrailRenderer()
    {
        _trailParticleSystem.gameObject.SetActive(true);
        _trailRenderer.gameObject.SetActive(true);
        
        Gradient trailGradient = new Gradient();
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(0.5f, 0.5f); 
        alphaKeys[1] = new GradientAlphaKey(0.0f, 1.0f);
        
        GradientColorKey[] colorKeys = new GradientColorKey[1];
        Gradient color = new Gradient();

        switch (_blockColor)
        {
            case BlockColor.Red:
            {
                colorKeys[0] = new GradientColorKey(Color.red, 0f);   // Start color (at the object's position)
                break;
            }
            case BlockColor.Yellow:
            {
                colorKeys[0] = new GradientColorKey(Color.yellow, 0f);
                break;
            }
            case BlockColor.Blue:
            {
                colorKeys[0] = new GradientColorKey(Color.blue, 0f);
                break;
            }
            case BlockColor.Purple:
            {
                colorKeys[0] = new GradientColorKey(Color.purple, 0f);
                break;
            }
        }
        trailGradient.SetKeys(colorKeys, alphaKeys);
        
        _trailRenderer.colorGradient = trailGradient;
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
