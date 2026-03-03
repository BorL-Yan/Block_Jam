using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UIScript
{
    public class TutorialTextAnimation : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        public Color _color_1;
        public Color _color_2;
        
        
        private void Start()
        {
            _text.color = _color_1;

            Sequence sequence = DOTween.Sequence();

            sequence.Append(_text.DOColor(_color_2, 0.5f).SetEase(Ease.Linear))
                .AppendInterval(1f)
                .Append(_text.DOColor(_color_1, 0.5f).SetEase(Ease.Linear))
                .AppendInterval(1f)
                .SetLoops(-1, LoopType.Yoyo);
            
            Sequence sequenceScail = DOTween.Sequence();
            sequenceScail.Append(_text.DOScale(1.1f, 1f).SetEase(Ease.Linear))
                .Append(_text.DOScale(1f, 1f).SetEase(Ease.Linear))
                .Append(_text.DOScale(1.1f, 1f).SetEase(Ease.Linear))
                .SetLoops(-1, LoopType.Yoyo);
        }
        
    }
}