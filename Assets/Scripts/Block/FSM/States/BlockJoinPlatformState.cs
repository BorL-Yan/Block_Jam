using System;
using System.Collections;
using Lib;
using UnityEngine;


public class BlockJoinPlatformState : BlockStateBase
{
    public BlockJoinPlatformState(BlockStateMachine stateMachine, BlockController blockController) : base(stateMachine, blockController) { }

    private Action _callback;
    private bool _activateJoin;
    
    public override void Enter()
    {
        base.Enter();
        
        _blockController.infoUI.SetText("Jump");
        
        _blockController.Action.OnJoin += OnJoin;
        _blockController.DiactivateTrailRenderer();
        
        
        _blockController.Animation.Play(BlockAnimationType.Jump);
        
        
        _blockController.MaterilaController.SetEyes(_blockController.BlockColor, BlockEyesType.Happy);
        _activateJoin = false;
        if (_blockController.MergePoint != null) 
        {
            _blockController.MergePoint.IsStanding = true;
        }
        
        MergeController.Instance.MergeToColor(_blockController.BlockColor);
        SoundManager.Instance.PlayOneShot(SoundType.Jump);
    }

    public override void Exit()
    {
        base.Exit();
        _blockController.Action.OnJoin -= OnJoin;
    }

    public override void AnimationTriger(BlockAnimationTrigger trigger)
    {
        switch (trigger)
        {
            case BlockAnimationTrigger.End:
            {
                _blockController.Rotate(Vector3.back, 0.1f);
                _blockController.Animation.Play(BlockAnimationType.Idle);
                _blockController.infoUI.SetText("Idle");
                _blockController.MaterilaController.SetEyes(_blockController.BlockColor, BlockEyesType.Active);
                _activateJoin = true;
                _callback?.Invoke();
                
                CoroutineRunner.Instance.Run(GameEndCallback());
                
                break;
            }
        }
    }

    private IEnumerator GameEndCallback()
    {
        yield return new WaitForSeconds(0.1f);
        MergeController.Instance.DetectGameEnd();
    }
    
    public bool OnJoin(Action callback)
    {
        if(_activateJoin) return true;
        
        _callback = callback;
        return false;
    }
}
