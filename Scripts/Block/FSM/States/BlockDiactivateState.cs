using System.Collections.Generic;
using Game;
using UnityEngine;


    public class BlockDiactivateState : BlockStateBase
    {
        public BlockDiactivateState(BlockStateMachine stateMachine, BlockController controller) 
            : base(stateMachine, controller) { }


        public override void Enter()
        {
            base.Enter();
            LevelController.Instance.levelActions.OnFindPath += UpdatePath;
            
            UpdatePath();
        }

        private void UpdatePath()
        {
            List<Cell> path = _blockController.Cell.Grid.PathFinder.FindPathToBottomLine(_blockController.Cell);
            
            _blockController.Points = new();
            if (path != null)
            {
                foreach (var cell in path)
                {
                    _blockController.Points.Enqueue(cell.transform.position);
                }
                
                _stateMachine.ChangeState<BlockActivateState>();
            }
        }

        public override void Exit()
        {
            base.Exit();
            LevelController.Instance.levelActions.OnFindPath -= UpdatePath;
        }
        
    }
