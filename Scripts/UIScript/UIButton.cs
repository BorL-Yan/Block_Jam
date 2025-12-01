using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIButton : MonoBehaviour, IPointerClickHandler
{
    private UnityEvent OnClick = new();

    public void AddListner(UnityAction action)
    {
        OnClick.AddListener(action);
    }

    public void RemoveListener(UnityAction action)
    {
        OnClick.RemoveListener(action);
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        OnClick?.Invoke();
    }
}
