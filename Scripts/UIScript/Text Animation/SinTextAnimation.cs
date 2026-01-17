using System;
using System.Collections.Generic;
using UnityEngine;

namespace UIScript
{
    public class SinTextAnimation: MonoBehaviour
    {
        [SerializeField] private List<RectTransform> _text;
        [SerializeField] private float _delta = 0.1f;
        
        [SerializeField] private  float amplitude = 1.0f; 
        [SerializeField] private  float frequency = 1.0f; 
        
        [SerializeField] private float _rotationSpeed = 1.0f;

        private float timeElapsed = 0f;
        private void Update()
        {
            timeElapsed += Time.deltaTime * frequency;
            float tempElapsed = timeElapsed;
            for (int i = 0; i < _text.Count; i++)
            {
                tempElapsed += _delta * i;
                
                float sin = Mathf.Sin(tempElapsed);
                float yOffset = sin * amplitude;
                Vector3 position = new Vector3(_text[i].localPosition.x, 
                    yOffset, _text[i].localPosition.z);
                _text[i].transform.localPosition = position;
                
                _text[i].localRotation = Quaternion.Euler(0, 0, sin * _rotationSpeed);
            }
        }
    }
 }