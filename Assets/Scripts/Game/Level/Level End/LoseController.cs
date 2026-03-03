
using com.cyborgAssets.inspectorButtonPro;
using DG.Tweening;
using Game;
using TMPro;
using UnityEngine;

public class LoseController : MonoBehaviour
{
    [SerializeField] private GameObject _loseScreen;
    
    [SerializeField] private Transform _healthIcon;
    private Sequence _healthSequence;
    
    [SerializeField] private TMP_Text _loseText;

    [SerializeField] private UIButton _exitButton;
    [SerializeField] private UIButton _tryAgainButton;

    private void Start()
    {
        _loseScreen.SetActive(false);
    }
    [ProButton]
    public void ActivateLoseScreen()
    {
        _exitButton.OnClick += DeactivateLoseScreen;
        _tryAgainButton.OnClick += RestartLevel;
        
        _loseScreen.SetActive(true);
        
        _healthSequence?.Kill();
        _healthSequence = DOTween.Sequence();

        _healthSequence.Append(_healthIcon.DOScale(1.03f, 0.3f).SetEase(Ease.OutQuad)) 
            .Append(_healthIcon.DOScale(1f, 0.3f).SetEase(Ease.InQuad))               
            .SetLoops(-1);

        string text = $"Level {GameSave.GetSettings().Level} \n Failed";
        _loseText.text = text;
    }

    public void DeactivateLoseScreen()
    {
        _exitButton.OnClick -= DeactivateLoseScreen;
        _tryAgainButton.OnClick -= RestartLevel;
        _loseScreen.SetActive(false);
        _healthSequence?.Kill();
    }

    public void RestartLevel()
    {
        DeactivateLoseScreen();
    }
    
}
