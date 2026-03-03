using DG.Tweening;
using UnityEngine;


public class BlockRanState : BlockStateBase
{
    public BlockRanState(BlockStateMachine stateMachine, BlockController controller) 
        : base(stateMachine, controller)
    { }

    private Vector3 _targetPoint;
    private MergePoint _mergePoint;
    private bool _finalPoint;
    
    public override void Enter()
    {
        base.Enter();
        _blockController.ActivateTrailRenderer();
        
        SoundManager.Instance.PlayOneShot(SoundType.Activate);
        
        _finalPoint = false;
        
        _blockController.Animation.Play(BlockAnimationType.Run_Activate);
        _blockController.MaterilaController.SetEyes(_blockController.BlockColor, BlockEyesType.Happy);
        
        Sequence scaleSequence = DOTween.Sequence();
        
        float inDuration = 0.1f;
        float outDuration = 0.05f;
        
        scaleSequence.Append(_blockController.Root.DOScale(Vector3.one * 1.2f, inDuration))
            .Append(_blockController.Root.DOScale(Vector3.one, outDuration));
        
        NextPoint();
    }

    public override void Update()
    {
        base.FixedUpdate();
        Move();
    }

    private void Move()
    {
        Vector3 vectorToTarget = _targetPoint - _blockController.Position;
    
        if (vectorToTarget.sqrMagnitude < 0.1f)
        {
            HandleArrival();
            return;
        }
        Vector3 direction = vectorToTarget.normalized;
    
        _blockController.Move(direction);

        if (direction != Vector3.zero)
        {
            _blockController.Rotate(direction, 0.07f);
        }
    }

    private void HandleArrival()
    {
        if (!_finalPoint)
        {
            NextPoint();
        }
        else
        {
            _blockController.Root.localRotation = Quaternion.Euler(new Vector3(345, 180, 0));
            _blockController.Root.position = _targetPoint;
        
            _blockController.Rotate(Vector3.back, 0.1f);
            _stateMachine.ChangeState<BlockJoinPlatformState>();
        }
    }

    private void NextPoint()
    {
        if (_blockController.Points.TryDequeue(out Vector3 nextPoint))
        {
            _targetPoint = nextPoint;
        }
        else
        {
            (Vector3, MergePoint) data = MergeController.Instance.GetPosition(_blockController.BlockColor, 
                            _blockController.GetComponent<IMovable>(), 
                            _blockController.GetComponent<Blcok.IDisposable>()
                            );
            if (data.Item2 == null)
            {
                Debug.LogError("Не удалось найти место для блока!");
                // Тут нужна логика Game Over или уничтожения блока
                return;
            }
            
            _targetPoint = data.Item1;
            _mergePoint = data.Item2;
            _finalPoint = true;
            _mergePoint.OnChangePosition += ChangePosition;
            
            _blockController.MergePoint = _mergePoint;
        } 
    }

    private void ChangePosition(MergePoint point)
    {
        _mergePoint = point;
        _blockController.MergePoint = _mergePoint;
        _targetPoint = _mergePoint.Pos;
    }
    
    public override void Exit()
    {
        if (_mergePoint != null)
        {
            _mergePoint.OnChangePosition -= ChangePosition;
        }
    }
}
