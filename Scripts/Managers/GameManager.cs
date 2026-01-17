using System;
using Game;
using Lib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameManager : SingletonSceneAutoCreated<GameManager>
    { 
        private string _levelPrefabPath = "Level/Level";
        private string _gameHiderPrefabPath = "Game/Hider";
        
        public LoadManueController LoadManueController { get; private set; }
        public ActivateSceneController ActivateSceneController { get; private set; }

        public bool isWin;
        
        public bool Initialized { get; private set; }
        

        public void Init()
        {
            Application.targetFrameRate = 120;
            Initialized = true;
            GameSave.Init();
            
            ActivateSceneController = new();
            
            LevelController levelController = CreatePrefab(_levelPrefabPath).GetComponent<LevelController>();
            levelController.gameObject.SetActive(false);
            LoadManueController = CreatePrefab(_gameHiderPrefabPath).GetComponent<LoadManueController>();
            LoadManueController.ActivateLoadManue(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        public GameObject CreatePrefab(string path)
        {
            GameObject levelPrefab = Resources.Load<GameObject>(path);
            var obj =  GameObject.Instantiate(levelPrefab);
            obj.transform.SetParent(this.transform);
            return obj;
        }
        
        
    }
}