using System;
using System.Collections.Generic;


    public class BlockStateMachine
    {
        private Dictionary<Type, BlockStateBase> _states = new();
        private BlockStateBase _currentState;
        public BlockStateBase CurrentState => _currentState;
        
        private readonly BlockController _blockController;

        public BlockStateMachine(BlockController blockController)
        {
            _blockController = blockController;
        }

        public void Init(BlockController blockController)
        {
            AddState(typeof(BlockDiactivateState), new BlockDiactivateState(this, blockController));
            AddState(typeof(BlockActivateState), new BlockActivateState(this, blockController));
            AddState(typeof(BlockIdleState), new BlockIdleState(this, blockController));
            AddState(typeof(BlockRanState), new BlockRanState(this, blockController));
            
            
        }

        private void AddState(Type type, BlockStateBase state)
        {
            _states.Add(type, state);
        }

        public void ChangeState<T>() where T : BlockStateBase
        {
            if (_currentState is T) return;
            
            _currentState?.Exit();
            _currentState = _states[typeof(T)];
            _currentState.Enter();
        }

        public bool InState<T>() where T : BlockStateBase
        {
            return _currentState is T;
        }
        
        public void Update()
        {
            if(_currentState == null) return;
            _currentState.Update();
        }

        public void FixedUpdate()
        {
            if(_currentState == null) return;
            _currentState.FixedUpdate();
        }
    }
