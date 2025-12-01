using UnityEngine;
using UnityEngine.UIElements;

public class ClearSave : MonoBehaviour
{
    public void Start()
    {
        Clear();
    }
    
    public void Clear()
    {
        var save = GameSave.GetSettings();

        save.Level = 0;
        GameSave.SetSettings(save);
        GameSave.Save();
    }
}
