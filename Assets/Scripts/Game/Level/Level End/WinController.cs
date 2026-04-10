using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using com.cyborgAssets.inspectorButtonPro;
using Managers;
using UnityEngine.UI;

namespace Game
{
    public class WinController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _winPartical;
        private int _activeCount;
        [SerializeField] private GameObject _jamPartical;

        [SerializeField] private Animator _animator;
        [SerializeField] private string _winAnimationName = "Win";
        
        [SerializeField] private List<GameObject> _text;
        [SerializeField] private Transform _textRoot;
        [SerializeField] private Vector3 _startTextTransform;
        private Sequence _sequence;

        [SerializeField] private List<BlockAniamtionController> _blockTriggers;
        [SerializeField] private Image _endImage;
        
        [SerializeField] private Image _backImage;
        [SerializeField] private Color _backColor;
        
        
        
        private Action _callback;
        
        private void Start()
        {
            _backImage.gameObject.SetActive(false);
            transform.rotation = Quaternion.Euler(new Vector3(55,0,0));
            Reset();
        }

        public void AnimationTrigger(TriggerType trigger)
        {
            switch (trigger)
            {
                case TriggerType.Partical:
                {
                    ActivateWinPartical();
                    break;
                }
                case TriggerType.Jam:
                {
                    _jamPartical.SetActive(true);
                    break;
                }
                case TriggerType.Text:
                {
                    ActivateText();
                    break;
                }
                case TriggerType.End:
                {
                    _endImage.gameObject.SetActive(true);
                    _endImage.DOColor(Color.black, 0.2f)
                        .OnComplete(() =>
                        {
                            _callback?.Invoke();
                            Reset();
                            _backImage.gameObject.SetActive(false);
                        });
                    break;
                }
            }
        }

        


        [ProButton]
        private void ActivateButton()
        {
            if (Application.isPlaying)
            {
                Activate(() => { });
            }
        }

        public void Activate(Action callback)
        {
            _callback = callback;
           
            _backImage.color = new Color(0, 0, 0, 0);
            _backImage.gameObject.SetActive(true);
            _backImage.DOColor(_backColor, 0.2f);
            
            
            ActivateWinAniamtion();
        }

        private void Reset()
        {
            _sequence?.Kill();
            
            _activeCount = 0;
            _textRoot.gameObject.SetActive(false);
            _blockTriggers.ForEach(b => b.gameObject.SetActive(false));
            _endImage.color = new Color(0, 0, 0, 0);
            _endImage.gameObject.SetActive(false);
            
            _jamPartical.SetActive(false);
            _winPartical.ForEach(w => w.SetActive(false));
            
            _textRoot.localPosition = _startTextTransform;
            _textRoot.localScale = Vector3.one * 1.2f;
            
            _text.ForEach(w => w.transform.localScale = Vector3.zero);
        }
        private void ActivateWinAniamtion()
        {
            SoundManager.Instance.PlayOneShot(SoundType.Win);

            Reset();

            _textRoot.gameObject.SetActive(true);
            
            _blockTriggers.ForEach(b => b.gameObject.SetActive(true));
            _blockTriggers[0].Play(BlockAnimationType.Final_Win_Left);
            _blockTriggers[1].Play(BlockAnimationType.Final_Win_Midle);
            _blockTriggers[2].Play(BlockAnimationType.Final_Win_Right);
            
            _activeCount = 0;
            _animator.Play(_winAnimationName);
        }
        
        private void ActivateText()
        {
            _textRoot.localScale = Vector3.one * 1.2f;
            
            _text.ForEach(w => w.transform.localScale = Vector3.zero);
    
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            
            _textRoot.gameObject.SetActive(true);
            
            _sequence.Append(_textRoot.DOMove(_textRoot.position + _textRoot.up * 0.5f, 2).SetEase(Ease.Linear));
            _sequence.Join(_textRoot.DOScale(Vector3.one * 1.45f, 2f).SetEase(Ease.Linear));

            
            float delayBetweenLetters = 0.05f; 

            for (int i = 0; i < _text.Count; i++)
            {
                var item = _text[i];
                float startTime = i * delayBetweenLetters;
                
                Sequence charSeq = DOTween.Sequence();
                charSeq.Append(item.transform.DOScale(Vector3.one * 3.5f, 0.1f).SetEase(Ease.Linear))
                    .Append(item.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InOutBack));
                
                _sequence.Insert(startTime, charSeq);
            }

            _sequence.Play();
        }
        
        
        private void ActivateWinPartical()
        {
            if(_activeCount < _winPartical.Count)
              _winPartical[_activeCount++].SetActive(true);  
        }

        public void Diactivate()
        {
            Reset();
        }
        
        
    }
}