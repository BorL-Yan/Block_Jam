using System;
using Lib;
using UnityEngine;

namespace Game
{
    public class GameEndContoller : SingletonScene<GameEndContoller>
    {
        [SerializeField] private GameObject _hider;
        [SerializeField] private GameObject _gamePanel;
        
        [SerializeField] private GameObject _victory;
        [SerializeField] private GameObject _death;
        
        
        private void OpenPanel(bool value)
        {
            _hider.SetActive(true);    
            _gamePanel.SetActive(false);    
            _victory.SetActive(value);
            _death.SetActive(!value);

            if (value)
            {
                
                //Save Content is Null
                var data = GameSave.GetSettings();
                data.Level++;
                GameSave.SetSettings(data);
                GameSave.Save();
            }
        }
        
        private void OnEnable()
        {
            LevelController.Instance.levelActions.OnEndLevel += OpenPanel;
        }

        private void OnDisable()
        {
            if(LevelController.Instance != null)
                LevelController.Instance.levelActions.OnEndLevel -= OpenPanel;
        }
    }
}