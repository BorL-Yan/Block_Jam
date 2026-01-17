using System;
using System.Collections.Generic;
using DG.Tweening;
using Game;
using UnityEngine;

public class BlockDiactivateState : BlockStateBase
    {
        public BlockDiactivateState(BlockStateMachine stateMachine, BlockController controller) 
            : base(stateMachine, controller) { }


        private Sequence _rotateSequence;
        
        public override void Enter()
        {
            base.Enter();
            
            LevelController.Instance.levelActions.OnFindPath += UpdatePath;
            
            _blockController.Body.transform.localPosition = new Vector3(0, 0, 0);
            //_blockController.Body.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            
            
            _blockController.Animation.Play(BlockAnimationType.Diactivate);
            _blockController.MaterilaController.SetEyes(_blockController.BlockColor, BlockEyesType.Diactive);
            
            _blockController.Action.OnSelect += Select;
            
            
            _blockController.ActivateMaterial(false);
            _blockController.MaterilaController.SetActivateColor(_blockController.BlockColor, false);
            
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
                    try
                    {
                        _blockController.Points.Enqueue(cell.transform.position);
                    }
                    catch (MissingReferenceException e)
                    {
                        Debug.Log(e);
                        return;
                    }
                }
                _stateMachine.ChangeState<BlockActivateState>();
            }
        }

        public override void Exit()
        {
            base.Exit();
            LevelController.Instance.levelActions.OnFindPath -= UpdatePath;
            
            _blockController.Action.OnSelect -= Select;
        }


        private void Select()
        {
            if(_rotateSequence != null) return;
            _rotateSequence?.Kill();
            _rotateSequence = DOTween.Sequence();
            
            float duration = 0.1f;
            Vector3 startRotation = _blockController.Root.transform.eulerAngles;
            Vector3 dampingValue = new Vector3(0, 40, 0);
            _rotateSequence.Append(_blockController.Root.DORotate(startRotation - dampingValue, duration).SetEase(Ease.OutQuad))
                .Append(_blockController.Root.DORotate(startRotation + dampingValue, duration * 2).SetEase(Ease.InOutQuad))
                .Append(_blockController.Root.DORotate(startRotation, duration))
                .OnComplete(()=>_rotateSequence = null);
            _rotateSequence.Play();
            
        }

        public override void Dispose()
        {
            LevelController.Instance.levelActions.OnFindPath -= UpdatePath;
            
            _blockController.Action.OnSelect -= Select;
        }
    }
