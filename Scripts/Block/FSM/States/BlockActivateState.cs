
using System.Collections.Generic;
using Game;
using Unity.VisualScripting;
using UnityEngine;


    public class BlockActivateState : BlockStateBase
    {
        public BlockActivateState(BlockStateMachine stateMachine, BlockController controller) 
            : base(stateMachine, controller)
        { }
        
        public override void Enter()
        {
            base.Enter();
            _blockController.Action.OnSelect += Select;
            LevelController.Instance.levelActions.OnFindPath += UpdatePath;
            
            _blockController.Rotate(Vector3.back);
        }

        public override void Exit()
        {
            base.Exit();
            _blockController.Action.OnSelect -= Select;
            
        }

        private void UpdatePath()
        {
            // PathFinder pathFinder = new PathFinder(_blockController.Cell.Grid);
            // List<Cell> path = pathFinder.FindPathToBottomLine(_blockController.Cell); 

            List<Cell> path = _blockController.Cell.Grid.PathFinder.FindPathToBottomLine(_blockController.Cell); 
            _blockController.Points = new();
            if (path != null)
            {
                foreach (var cell in path)
                    _blockController.Points.Enqueue(cell.transform.position);
            }
            
        }

        private void Select()
        {
            
            if (MergeController.Instance.OpeningPoints())return;
            MergeController.Instance.AddNewBlock();
            
            _blockController.BlockSelected();
            LevelController.Instance.levelActions.OnFindPath -= UpdatePath;
            LevelController.Instance.levelActions.OnFindPath?.Invoke();
            _stateMachine.ChangeState<BlockRanState>();
        }
        
        
    }
