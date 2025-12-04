
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class OpenScene : MonoBehaviour
{
    [SerializeField] private UIButton _uIButton;
    
    
    private void Open()
    {
        Debug.Log("Open Scene ");
        GameSettings setings = GameSave.GetSettings();
        int level = setings.Level;
        if (level < 15)
        {
            level++;
        }
        SceneManager.LoadScene(level);
        
        
    }

    private void OnEnable() 
    {
        _uIButton.AddListner(Open);
    }

    private void OnDestroy() {
        _uIButton.RemoveListener(Open);
    }
}
