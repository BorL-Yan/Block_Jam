using Lib;
using Managers;
using UnityEngine;

namespace Game
{
    public class GameEndContoller : SingletonScene<GameEndContoller>
    {
        [SerializeField] private WinController _victory;
        [SerializeField] private LoseController _lose;
        
        
        private void OpenPanel(bool value)
        {
            if (value)
            {
                _victory.Activate(() =>
                {
                    Win();
                });
            }
            else
            {
                _lose.ActivateLoseScreen();
            }
        }

        private void Win()
        {
            var data = GameSave.GetSettings();
            
            data.Level = Mathf.Min(GameSave.MaxLevel, data.Level + 1);
            GameSave.SetSettings(data);
            GameSave.Save();

            int activateIndex = 0;
            if (data.Level <= 3)
            {
                activateIndex = data.Level+1;
                GameManager.Instance.LoadManueController.ActivateLoadManue(true);
            }
            else
            {
                GameManager.Instance.LoadManueController.ExitIcon(true);
            }
            
            GameManager.Instance.isWin = true;
            GameManager.Instance.ActivateSceneController.ActivateScene(activateIndex);
        }
        
        public void Diactivate()
        {
            _victory.Diactivate();
            _lose.DeactivateLoseScreen();
        }
        
        private void OnEnable()
        {
            if(LevelController.Instance != null)
                LevelController.Instance.levelActions.OnEndLevel += OpenPanel;
            else
            {
                Debug.Log(" Level Controller is NULL");
            }
        }

        private void OnDisable()
        {
            if(LevelController.Instance != null)
                LevelController.Instance.levelActions.OnEndLevel -= OpenPanel;
        }
    }
}