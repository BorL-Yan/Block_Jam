using System;
using System.Collections;
using DG.Tweening;
using Lib;
using UnityEngine;

public class MovmentControl
    {
        private readonly BlockController _blockController;
        private Action _callback;
        
        private Sequence _rotateSequence;
        private Sequence _jumpSequence;
        
        public MovmentControl(BlockController blockController)
        {
            _blockController = blockController;
        }

        public void Move(Vector3 direction, float speed)
        {
            // Vector3 pos = transform.position;
            // pos += direction * _speed * Time.fixedTime;
            // _root.position = pos;
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
                })
                .OnComplete(() =>
                {
                    _blockController.transform.position = targetPosition;
                    _blockController.Root.rotation = Quaternion.LookRotation(Vector3.back);
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
        public void Jump(int index, Vector3 position, Action callBack)
        {
            _callback = callBack;
            BlockAnimationType animationType = BlockAnimationType.Merge_Midle;
            switch (index)
            {
                case 1:
                {
                    _angle = 30f;
                    animationType = BlockAnimationType.Merge_Right;
                    SoundManager.Instance.PlayOneShot(SoundType.Merge);
                    break;
                }
                case 3:
                {
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
                _blockController.Animation.Play(animationType);
                CoroutineRunner.Instance.StartCoroutine(BlockUp(position));
            }
        }
        
        private float timeElapsed = 0;

        private IEnumerator BlockUp(Vector3 horizontalDirection)
        {
            timeElapsed = 0f;
            Vector3 startPosition = _blockController.Root.position;
            while (timeElapsed <= 1)
            {
                timeElapsed += Time.deltaTime * _blockController.jumpSpeed;
                    
                float normalizedTime = Mathf.Clamp01(timeElapsed);
                float angle = Mathf.Lerp(0, _angle, normalizedTime);
                
                        
                float curveMultiplier = _blockController._jumpAniamtionCurve.Evaluate(normalizedTime) ;

                Vector3 pos = startPosition + Vector3.up * curveMultiplier * _blockController.jumpAmplitude;

                _blockController.Root.position = pos;
                _blockController.Root.rotation = Quaternion.Euler(new Vector3(0f, 180 + angle, 0f));
                yield return null;
            }
            CoroutineRunner.Instance.StartCoroutine(BlockHorizontal(horizontalDirection));
        }
        
        private IEnumerator BlockHorizontal(Vector3 targetPosition)
        {
            timeElapsed = 0f;
            Vector3 startPosition = _blockController.Root.position;
            targetPosition.y = startPosition.y;

            Vector3 startRoation = _blockController.Root.eulerAngles;
            
            while (timeElapsed <= 1)
            {
                timeElapsed += Time.deltaTime * _blockController.mergeSpeed;
                    
                float normalizedTime =  Mathf.Clamp01(timeElapsed);
                float angle = Mathf.Lerp(startRoation.y, 180, normalizedTime);
                        
                float curveMultiplier = _blockController._mergeAniamtionCurve.Evaluate(normalizedTime);
                
                _blockController.Root.position = Vector3.Lerp(startPosition, targetPosition, curveMultiplier * _blockController.mergeAmplitude);
                _blockController.Root.rotation = Quaternion.Euler(new Vector3(0f, angle, 0f));
                
                yield return null;
            }
            _blockController.Partical();

            _blockController.Root.DOScale(Vector3.zero, 0.1f)
                .OnComplete(() => _callback?.Invoke());
        }

        
        
    }
