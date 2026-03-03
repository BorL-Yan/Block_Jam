using Game;
using UnityEngine;

public class BoxController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    
    [SerializeField] private string _idleAniamtion;
    [SerializeField] private string _breakAniamtion;

    public bool Active;

    public void SetActive(bool active)
    {
        if(active) Init();
        else EndAniamtion();
    }
    
    public void Init()
    {
        gameObject.SetActive(true);
        _animator.Play(_idleAniamtion);
        Active = true;
    }

    public void Activate()
    {
        _animator.Play(_breakAniamtion);
        SoundManager.Instance.PlayOneShot(SoundType.BoxBreak, Random.Range(0.98f, 1.02f));
        MobileVibration.Vibrate(VibrationType.strong);
        CameraShace.Instance.Shake();
    }

    public void EndAniamtion()
    {
        gameObject.SetActive(false);
        Active = false;
    }
    
}
