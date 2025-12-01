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
            _finalPoint = false;
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
                _blockController.Rotate(direction);
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
                _blockController.Rotate(Vector3.back);
                _blockController.Root.position = _targetPoint;
            
                if (_mergePoint != null) 
                {
                    _mergePoint.IsStanding = true;
                }
            
                _stateMachine.ChangeState<BlockIdleState>();
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
                                _blockController.GetComponent<IDisposable>()
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
            } 
        }

        private void ChangePosition(MergePoint point)
        {
            _mergePoint = point;
            _targetPoint = _mergePoint.Pos;
        }

        public override void AnimationTriger()
        {
            base.AnimationTriger();
        }

        public override void Restart()
        {
            base.Restart();
        }

        public override void Exit()
        {
            if (_mergePoint != null)
            {
                _mergePoint.OnChangePosition -= ChangePosition;
            }
        }
    }
