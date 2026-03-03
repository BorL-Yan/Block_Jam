using System;
using DG.Tweening;
using UnityEngine;

public class MovmentControl
    {
        private readonly BlockController _blockController;
        private Action _callback;
        private Action _mergeCallBack;
        
        private Sequence _rotateSequence;
        private Sequence _jumpSequence;
        
        public MovmentControl(BlockController blockController)
        {
            _blockController = blockController;
        }

        public void Move(Vector3 direction, float speed)
        {
            _blockController.infoUI.SetText("Move");
            _blockController.Root.Translate(direction * speed * Time.deltaTime, Space.World);
        }

        public void MoveToCell(Vector3 direction)
        {
            Vector3 targetPosition = _blockController.Root.position + direction;
            Sequence sequence = DOTween.Sequence();
            
            _blockController.transform.localScale = Vector3.one * 0.5f;
            _blockController.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InCubic);
            _blockController.Animation.Play(BlockAnimationType.Run_Activate);
            
            Rotate(direction);
            Rotate(Vector3.back, 0.5f);
            
            float duration = 0.3f;
            
            sequence.Append(_blockController.Root.DOMove(targetPosition, duration).SetEase(Ease.InCubic))
                .AppendCallback(() =>
                {
                    _blockController.StateMachine.ChangeState<BlockActivateState>();
                    _blockController.Animation.Play(BlockAnimationType.Idle);
                    _blockController.BlockedObject(false);
                    _blockController.infoUI.SetText("Move");
                })
                .OnComplete(() =>
                {
                    _blockController.transform.position = targetPosition;
                    _blockController.Root.rotation = Quaternion.LookRotation(Vector3.back);
                    _blockController.infoUI.SetText("Idle");
                });
        }
        
        public void MoveToPlatform(Vector3 targetPosition, float duration = 0.2f)
        {
            _jumpSequence?.Kill(true);
            _jumpSequence = DOTween.Sequence();

            Vector3 direction = (targetPosition - _blockController.Root.position).normalized;
            if(direction != Vector3.zero) Rotate(direction, duration / 2);
            
            _blockController.Animation.Play(BlockAnimationType.Run_Activate);
            _jumpSequence.Append(_blockController.Root.DOMove(targetPosition, duration).SetEase(Ease.InCubic))
                .AppendCallback(() =>
                {
                    _blockController.StateMachine.ReactivateState<BlockJoinPlatformState>();
                    Rotate(Vector3.back, 0.2f);
                })
                .OnComplete(() =>
                {
                    _blockController.transform.position = targetPosition;
                    Rotate(Vector3.back);
                });
        }

        public void Rotate(Vector3 direction)
        {
            _blockController.Root.rotation = Quaternion.LookRotation(direction);
        }

        public void Rotate(Vector3 direction, float duration)
        {
            _rotateSequence?.Kill();
            _rotateSequence = DOTween.Sequence();

            Quaternion targetRotation = Quaternion.LookRotation(direction);

            _rotateSequence.Append(_blockController.Root.DORotateQuaternion(targetRotation, duration));
        }

        private float _angle = 0;
        public void Jump(int index, Vector3 position, Action callBack, Action mergeCallback)
        {
            _callback = callBack;
            BlockAnimationType animationType = BlockAnimationType.Merge_Midle;
            switch (index)
            {
                case 1:
                {
                    _blockController.infoUI.SetText($"Merge left");
                    _angle = 30f;
                    animationType = BlockAnimationType.Merge_Right;
                    break;
                }
                case 2:
                {
                    _blockController.infoUI.SetText($"Merge midle");
                    _mergeCallBack = mergeCallback;
                    animationType = BlockAnimationType.Merge_Midle;
                    _angle = 0;
                    SoundManager.Instance.PlayOneShot(SoundType.Merge);
                    //MobailVibration.Vibration(VibrationType.average);
                    
                    break;
                }
                case 3:
                {
                    _blockController.infoUI.SetText($"Merge right");
                    _angle = -30f;
                    animationType = BlockAnimationType.Merge_Left;
                    break;
                }
            }

            try
            {
                if (_blockController.Action.OnJoin(Activate))
                {
                    Activate();
                }
            }
            catch (NullReferenceException e)
            {
                Activate();
            }
            
            void Activate()
            {
                //CoroutineTestUI.Instance.SetText("Activate");
                _blockController.Animation.Play(animationType);
                BlcokAnimation(position + Vector3.up*_blockController.jumpAmplitude);
            }
        }
        
        private void BlcokAnimation(Vector3 position)
        {
            Vector3 direction = (_blockController.Root.position - position).normalized;

            Sequence sequence = DOTween.Sequence();
            _blockController.infoUI.SetText($"StartAnim");
            sequence.Append(
                    _blockController.transform.DOMoveY(position.y, _blockController.jumpSpeed).SetEase(Ease.InCubic))
                .Join(_blockController.transform.DORotate(new Vector3(0f, 180 + _angle, 0f),
                    _blockController.jumpSpeed))
                .AppendInterval(_blockController.upPause)
                .Append(_blockController.transform.DOMoveX(_blockController.Root.position.x + direction.x * _blockController.mergeAmplitude,
                    _blockController.mergeSpeed / 3).SetEase(Ease.InCubic))
                .AppendCallback(() =>
                {
                    MobileVibration.Vibrate(VibrationType.weak);
                    _mergeCallBack?.Invoke();
                })
                .Append(_blockController.transform.DOMoveX(position.x, _blockController.mergeSpeed * 2 / 3).SetEase(Ease.OutSine))
                .AppendCallback(() =>
                {
                    _blockController.infoUI.SetText($"end Move");
                    _blockController.Partical();
                })
                .Append(_blockController.transform.DOScale(0, 0.1f))
                .OnComplete(() =>
                {
                    _blockController.infoUI.SetText($"EndAnim");
                    _callback?.Invoke();
                });
        }
    }
