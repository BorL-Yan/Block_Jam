using System.Collections.Generic;
using Game;
using UnityEngine;


public class BlockController : MonoBehaviour, IMovable, ISelect, IDisposable
{
    [field: SerializeField] public Transform Root;
    [field: SerializeField] public Transform Body;
    
    [SerializeField] private BoxController _blockedObject;
    [SerializeField] private Collider _selectCollider;


    [SerializeField] private TrailController _trailController;
    [SerializeField] private ParticleSystem _trailParticleSystem;

    [SerializeField] private List<SetBlockMaterial> blockMaterials;
    
    [field: SerializeField] public BlockAniamtionController Animation {get; private set; }
    [field: SerializeField] public BlockMaterilaController MaterilaController { get; private set; }
    
    public AnimationCurve _mergeAniamtionCurve;
    public float mergeAmplitude;
    public float mergeSpeed;
    
    public AnimationCurve _jumpAniamtionCurve;
    public float jumpAmplitude;
    public float jumpSpeed;
    
    public Vector3 Position => Root.position;
    public Vector3 MergeControllerPoint { get; private set; }
    
    public Queue<Vector3> Points { get; set; }
    
    [SerializeField] private float _speed;
    
    private MovmentControl _movment;
    public BlockAction Action;
    public MergePoint MergePoint;
    
    public BlockStateMachine StateMachine { get; private set; }
    [SerializeField] public Cell Cell;
    
    
    [SerializeField] private BlockColor _blockColor;
    public BlockColor BlockColor => _blockColor;
    
    
    private void Awake()
    {
        _movment = new MovmentControl(this);
        StateMachine = new BlockStateMachine(this);
        StateMachine.Init(this);
        
        if( Action == null ) Action = new BlockAction();
        DiactivateTrailRenderer();
    }

    public void Init(BlockColor blockColor, Cell cell)
    {
        _blockColor = blockColor;
        Cell = cell;
      
        MaterilaController.SetColor(blockColor);
        
        if (Cell != null)
        {
            Cell.BlockData.BlockColor = _blockColor;
        }

        StateMachine.ChangeState<BlockDiactivateState>(); 
    }

    
    /// <summary>
    /// Инициализация в редакторе — безопасно!
    /// </summary>
    public void InitEditor(BlockColor blockColor, Cell cell, bool blocked)
    {
#if UNITY_EDITOR
        BlockedObject(blocked);
        _selectCollider.enabled = !blocked;
        
        _blockColor = blockColor;
        Cell = cell;

        
        MaterilaController.SetColor(blockColor);
        //MaterilaController.SetEyes(blockColor, BlockEyesType.Active);
        
        ActivateMaterial(false);
#endif
    }

    public void BlockedObject(bool blocked)
    {
        _blockedObject.SetActive(blocked);
        MaterilaController.gameObject.SetActive(!blocked);
    }
    
    public void StartScene()
    {
        if (_blockedObject.isActiveAndEnabled) _blockedObject.Active = true;
        else StateMachine.ChangeState<BlockDiactivateState>();
        Action.InitilaHello?.Invoke();
    }

    private void Update()
    {
       StateMachine.Update(); 
    }

    private void FixedUpdate()
    {
        StateMachine.FixedUpdate();
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
    public void Rotate(Vector3 direction, float duration)
    {
        _movment.Rotate(direction, duration);
    }

    public void MoveOneShot(Vector3 targetPosition)
    {
        _movment.MoveToPlatform(targetPosition);
    }

    public void MoveOneShot(Vector3 targetPosition, float duration)
    {
        _movment.MoveToPlatform(targetPosition, duration);
    }

    public void MoveToCell(Vector3 direction)
    {
        _movment.MoveToCell(direction);
    }
    
    public void JumpAndMerge(int index, Vector3 position)
    {
        _movment.Jump(index, position, DestroyBlock);

    } 

    #endregion

    public void Select()
    {
        if (_blockedObject.Active == false)
        {
            Action.OnSelect?.Invoke();
        }
        else
        {
            Debug.Log("Blocked Box Active");
        }
    }

    public void BlockSelected()
    {
        Cell.BlockSelected();
       
    }

    public void ActivateBlockedBlock()
    {
        _blockedObject.Activate();
        MaterilaController.gameObject.SetActive(true);
        Cell.BlockData.BlockType = BlockType.Normal;
        _selectCollider.enabled = true;
        StateMachine.ChangeState<BlockActivateState>();
    }

    public void DiactivateTrailRenderer()
    {
        _trailController.Activate(false);
        _trailParticleSystem.Clear();
        _trailParticleSystem.gameObject.SetActive(false);
    }

    public void ActivateTrailRenderer()
    {
        _trailParticleSystem.gameObject.SetActive(true);
        _trailController.Activate(true);
        
        _trailController.Init(_blockColor);
    }

    public void ActivateMaterial(bool value)
    {
        foreach (var item in blockMaterials)
        {
            item.Activate(value);
        }
    }
    
    public void Partical()
    {
        GameObject partical = BlockJamParticals.Instance.GetPartical(_blockColor);
        partical.SetActive(true);
        partical.transform.position = Root.position;
    }
    
    public void DestroyBlock()
    {
        LevelController.Instance.RemoveBlock();
        StateMachine.CurrentState.Exit();
        StateMachine.Dispose();
        Destroy(Root.gameObject);
    }
    
    private void OnEnable()
    {
        if(LevelController.Instance!= null)
            LevelController.Instance.levelActions.OnStartLevel += StartScene;
        else
        {
            StartLevelRegistry.Register(StartScene);
        }
    }

    private void OnDisable()
    {
        if(LevelController.Instance != null)
            LevelController.Instance.levelActions.OnStartLevel -= StartScene;
    }

    private void OnDestroy()
    {
        if(LevelController.Instance != null)
            LevelController.Instance.levelActions.OnStartLevel -= StartScene;
    }
}

