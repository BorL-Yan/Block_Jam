using UnityEngine;

namespace UIScript
{
    public class CleraHistory : MonoBehaviour
    {
        [SerializeField] private UIButton _button;

        private void Restart()
        {
            GameSettings setings = GameSave.GetSettings();

            setings.Level = 0;
        
            GameSave.SetSettings(setings);
            GameSave.Save();
        }


        private void OnEnable()
        {
            _button.AddListner(Restart);
        }

        private void OnDisable()
        {
            _button.RemoveListener(Restart);
        }
    }
}