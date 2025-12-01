using UnityEngine;
using UnityEngine.SceneManagement;

namespace UIScript
{
    public class ExitManue: MonoBehaviour
    {
        [SerializeField] private UIButton _button;

        private void Open()
        {
            SceneManager.LoadScene(0);
        }
        
        private void OnEnable()
        {
            _button.AddListner(Open);
        }

        private void OnDisable()
        {
            _button.RemoveListener(Open);
        }
    }
}