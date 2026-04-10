using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game;
using Lib;
using UnityEngine;


public class BlockActivateState : BlockStateBase
{
    private Vector2 _helloDelay = new Vector2(10, 15);
    private float _helloSpiteDiactivateDelay = 0.5f;
    
    private Coroutine _coroutine;
    private Coroutine _helloCoroutine;
    
    
    public BlockActivateState(BlockStateMachine stateMachine, BlockController controller) 
        : base(stateMachine, controller)
    { }
    
    public override void Enter()
    {
        base.Enter();
        
        
        LevelController.Instance.AddBlock();
        
        _blockController.Action.OnSelect += Select;
        LevelController.Instance.levelActions.OnFindPath += UpdatePath;
        
        _blockController.infoUI.SetText("Active");
        
        _blockController.Animation.Play(BlockAnimationType.Activate);
        _coroutine = CoroutineRunner.Instance.StartCoroutine(HelloDelay());

        _blockController.Body.transform.localPosition = new Vector3(0, 0, 0.2f);
        _blockController.Body.transform.localRotation = Quaternion.Euler(345, 0, 0);

        UpdatePath();
        _blockController.MaterilaController.SetEyes(_blockController.BlockColor, BlockEyesType.Active);

        _blockController.Action.InitilaHello += HelloAnimation;


        _blockController.ActivateMaterial(true);
        _blockController.MaterilaController.SetActivateColor(_blockController.BlockColor, true);
    }

    public override void Exit()
    {
        base.Exit();
        _blockController.Action.OnSelect -= Select;
        if (_coroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(_coroutine);
        }

        if (_helloCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(_helloCoroutine);
        }
        _blockController.Action.InitilaHello -= HelloAnimation;
    }

    private void UpdatePath()
    {
        if (_blockController == null) 
        {
            return;
        }
        
        if (_blockController.Cell == null || _blockController.Cell.Grid == null)
        {
            return;
        }

        List<Cell> path = new();
        if (_blockController.Cell.Column != 0)
        {
            path = _blockController.Cell.Grid.PathFinder.FindPathToBottomLine(_blockController.Cell); 
        }
        _blockController.Points = new();
        
        
        if (path != null)
        {
            foreach (var cell in path)
            {
                if(cell != null)
                    _blockController.Points.Enqueue(cell.transform.position);
                else
                {
                   Debug.LogWarning("Null path found");
                }
            }
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
        MobileVibration.Vibrate(VibrationType.weak);
    }



    private IEnumerator HelloDelay()
    {
        float time = Time.time + Random.Range(_helloDelay.x, _helloDelay.y);
        while (true)
        {
            if (time < Time.time)
            {
                if (_blockController == null) break;
                _blockController.Animation.Play(BlockAnimationType.Hello);
                _blockController.MaterilaController.SetEyes(_blockController.BlockColor, BlockEyesType.Happy);
                _helloCoroutine = CoroutineRunner.Instance.StartCoroutine(HelloSpriteDiactive());
                time = Time.time + Random.Range(_helloDelay.x, _helloDelay.y);
            }
            yield return null;
        }
    }

    private IEnumerator HelloSpriteDiactive()
    {
        yield return new WaitForSeconds(_helloSpiteDiactivateDelay);
        _blockController.MaterilaController.SetEyes(_blockController.BlockColor, BlockEyesType.Active);
        _helloCoroutine = null;
    }


    private void HelloAnimation()
    {
        float randomStart = Random.Range(0.1f, 0.3f);
        Sequence sequence = DOTween.Sequence();
        sequence
            .AppendInterval(randomStart)
            .AppendCallback(() =>
            {
                _blockController.Animation.Play(BlockAnimationType.Hello);
            });
    }

    public override void Dispose()
    {
        base.Dispose();
        _blockController.Action.OnSelect -= Select;
        if (_coroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(_coroutine);
        }

        if (_helloCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(_helloCoroutine);
        }
        _blockController.Action.InitilaHello -= HelloAnimation;
        
        if (_blockController != null && _blockController.Points != null)
            _blockController.Points.Clear();
    }
}
