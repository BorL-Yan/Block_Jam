using TMPro;
using UnityEngine;

public class BlockInfoUI : MonoBehaviour 
{
    [SerializeField] private TMP_Text _text;

    private void Start()
    {
        Activate(false);
    }
    
    public void Activate(bool value)
    {
        _text.gameObject.SetActive(value);
    }

    public void SetText(string text)
    {
        Activate(true);
        _text.text = text;
    }
    
}
