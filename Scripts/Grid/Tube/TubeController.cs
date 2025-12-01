
using TMPro;
using UnityEngine;

public class TubeController: MonoBehaviour
{
    [SerializeField] private GameObject _visualModel; 
    [SerializeField] private TMP_Text _text;

    [SerializeField] private Cell _cell;

    private void Start()
    {
        UpdateTubeText();
    }
    
    public void Connect(Cell cell)
    {
        if(cell == null) return;
        _cell = cell;
    
        // Безопасная проверка на наличие данных
        if (_cell.BlockData != null && _cell.BlockData.Items != null)
        {
            _text.text = _cell.BlockData.Items.Count.ToString();
        }
    }

    public void UpdateTubeText()
    {
        if(_cell != null && _cell.BlockData != null && _cell.BlockData.Items != null)
            _text.text = _cell.BlockData.Items.Count.ToString();
    }

    public void SetRotation(TubeDirection direction)
    {
        float yRotation = 0;
        switch (direction)
        {
            case TubeDirection.Left:
            {
                yRotation = 0;
                break;
            }
            case TubeDirection.Right:
            {
                yRotation = 180;
                break;
            }
            case TubeDirection.Up:
            {
                yRotation = 90 ;
                break;
            }
            case TubeDirection.Down:
            {
                yRotation = -90 ;
                break;
            }

        }
        _visualModel.transform.localRotation = Quaternion.Euler(0, yRotation, 0);
    }
}

public enum TubeDirection
{
    Left = 0,
    Right = 1,
    Up = 2,
    Down = 3
}