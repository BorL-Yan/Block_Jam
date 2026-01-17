using System;
using UnityEngine;

namespace UIScript
{
    public class ChangeCanvasDistance : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        
        [SerializeField] private UIButton _button;
        [SerializeField] private OpenCloseType _openCloseType;
        
        [SerializeField] private int _currentDistance;
        [SerializeField] private int _targetDistance;

        
        
        private void SetDistance()
        {
            if (_openCloseType == OpenCloseType.Close)
            {
                _canvas.planeDistance = _currentDistance;
            }else if (_openCloseType == OpenCloseType.Open)
            {
                _canvas.planeDistance = _targetDistance;
            }
        }
        
        private void OnEnable()
        {
            _canvas.planeDistance = _currentDistance;
            _button.OnClick += SetDistance;
        }

        private void OnDisable()
        {
            _button.OnClick -= SetDistance;
        }
    }
}