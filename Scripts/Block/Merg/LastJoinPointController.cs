using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LastJoinPointController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _joinIcon;
    
    Sequence _iconSequence;
    public bool ativated {get; private set;} 
    
    public void Actviate()
    {
        _iconSequence?.Kill();
        _iconSequence = DOTween.Sequence();
        ativated = true;

        float duration = 0.5f;

        _iconSequence.Append(_joinIcon.DOColor(new Color(1, 0, 0), duration))
            .Join(_joinIcon.gameObject.transform.DOScale(0.95f, duration))
            .Append(_joinIcon.DOColor(Color.white, duration))
            .Join(_joinIcon.gameObject.transform.DOScale(1f, duration))
            .SetLoops(-1);

    }

    public void Diactivate()
    {
        ativated = false;
        _joinIcon.color = Color.white;
        _joinIcon.gameObject.transform.localScale = Vector3.one;
        _iconSequence.Kill();
    }
}
