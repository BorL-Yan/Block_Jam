using DG.Tweening;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;


    public class MovmentControl
    {
        private readonly BlockController _blockController;

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
        }
        
    }
