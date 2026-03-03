using System;
using DG.Tweening;
using Lib;
using UnityEngine;

namespace Game
{
    public class CameraShace : SingletonScene<CameraShace>
    {
        [SerializeField] private Transform camera;
         [SerializeField] private float duration = 0.5f;
         [SerializeField] private float strength = 0.5f;
         [SerializeField] private int vibrato = 10;
         [SerializeField] private float randomness = 90;
         private Sequence sequence;
         private Vector3 cameraPosition;

         private void Start()
         {
             cameraPosition = camera.position;
         }
        public void Shake()
        {
            // DOShakePosition-ը տեղաշարժում է տեսախցիկը
            // transform.DOShakePosition(տևողություն, ուժգնություն, տատանումների քանակ, պատահականություն)
            sequence?.Kill(true);
            sequence = DOTween.Sequence();
            sequence.Append(camera.DOShakePosition(duration, strength, vibrato, randomness))
                .OnComplete(() =>
                {
                    camera.position = cameraPosition;
                });


        }
    }
}