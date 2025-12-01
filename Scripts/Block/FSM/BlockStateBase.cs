

    public class BlockStateBase
    {
        protected readonly BlockStateMachine _stateMachine;
        protected readonly BlockController _blockController;

        public BlockStateBase(BlockStateMachine stateMachine, BlockController blockController)
        {
            _stateMachine = stateMachine;
            _blockController = blockController;
        }
        
        public virtual void Enter() { }

        public virtual void Exit() { Restart(); }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void AnimationTriger() { }
        public virtual void Restart(){}
    }
