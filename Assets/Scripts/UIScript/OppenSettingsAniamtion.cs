using System;
using DG.Tweening;
using UnityEngine;

namespace UIScript
{
    public class OppenSettingsAniamtion : MonoBehaviour
    {
        [SerializeField] private GameObject _obj;

        private void OnEnable()
        {
            Vector3 pos = _obj.transform.position;
            _obj.transform.position = pos + Vector3.left * 0.5f;

            Sequence sequence = DOTween.Sequence();

            sequence.Append(_obj.transform.DOMove(pos + Vector3.right * 0.1f, 0.2f))
                .Append(_obj.transform.DOMove(pos , 0.1f));
        }
    }
}