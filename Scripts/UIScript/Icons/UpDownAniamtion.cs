using System;
using DG.Tweening;
using UnityEngine;

namespace UIScript.Icons
{
    public class UpDownAniamtion : MonoBehaviour
    {
        [SerializeField] private float _upDistance;
        [SerializeField] private float _duration;
        
        [SerializeField] private Transform _icon;
        Sequence _iconSequence;


        private void Start()
        {
            _iconSequence = DOTween.Sequence();
            
            float randomPos = _icon.position.y + UnityEngine.Random.Range(_upDistance, -_upDistance);
            _icon.position = new Vector3(_icon.position.x, randomPos, _icon.position.z);
            
            _iconSequence.Append(_icon.DOLocalMoveY(_icon.localPosition.y + _upDistance, _duration))
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void OnDestroy()
        {
            _iconSequence?.Kill();
        }
    }
}