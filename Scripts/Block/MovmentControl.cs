using System;
using System.Collections;
using System.IO;
using DG.Tweening;
using Lib;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;


    public class MovmentControl
    {
        private readonly BlockController _blockController;
        private Action _callback;
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
        
        public void MoveOneShut(Vector3 direction)
        {
            Vector3 targetPosition = _blockController.Root.position + direction;
            // Убить старый твин перед запуском нового, чтобы избежать конфликтов
            _blockController.Root.DOKill(); 
            _blockController.Root.DOMove(targetPosition, 0.1f);
        }

        public void Rotate(Vector3 direction)
        {
            _blockController.Root.rotation = Quaternion.LookRotation(direction);
            //_blockController.Root.DORotate(direction, 0.1f);
        }

        
        private float _angle = 0;
        public void Jump(int index, Action callBack)
        {
            _callback = callBack;
            Vector3 directionToMerge = Vector3.zero;
            switch (index)
            {
                case 1:
                {
                    _angle = 30f;
                    directionToMerge = Vector3.right;
                    break;
                }
                case 3:
                {
                    _angle = -30f;
                    directionToMerge = Vector3.left;
                    break;
                }
            }
            
            Debug.Log($"Moving to {directionToMerge}");
            
            _blockController.ActivateCoroutine(BlockUp(directionToMerge));
            
            //Sequence sequence = DOTween.Sequence();
            
            // sequence.Append(_blockController.Root.DOMove(_blockController.Root.position + Vector3.up , 0.5f))
            //     .Append(_blockController.Root.DOMove(_blockController.Root.position + directionToMerge, 1f))
            //     .SetEase(_blockController._mergeAniamtionCurve)
            //     .JoinCallback(() =>
            //     {
            //         // Merge Effects
            //         
            //     })
            //     .AppendCallback(()=>);
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

            _blockController.ActivateCoroutine(BlockHorizontal(horizontalDirection));
        }
        
        private IEnumerator BlockHorizontal(Vector3 direction)
        {
            timeElapsed = 0f;
            Vector3 startPosition = _blockController.Root.position;
            while (timeElapsed <= 1)
            {
                timeElapsed += Time.deltaTime * _blockController.mergeSpeed;
                    
                float normalizedTime =  Mathf.Clamp01(timeElapsed);
                        
                float curveMultiplier = _blockController._mergeAniamtionCurve.Evaluate(normalizedTime);

                Vector3 pos = startPosition + direction * curveMultiplier * _blockController.mergeAmplitude;

                _blockController.Root.position = pos;
                yield return null;
            }
            Debug.Log("Effect ! !");
            _callback?.Invoke();
        }
        
        
    }
