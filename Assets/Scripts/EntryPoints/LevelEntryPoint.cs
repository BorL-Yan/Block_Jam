using System;
using Game;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EntryPoints
{
    public class LevelEntryPoint : MonoBehaviour
    {
        private void Awake()
        {
#if UNITY_EDITOR
            if (GameManager.Instance.Initialized == false)
            {
                InitManualTesting();
            }
#endif
        }

        private void InitManualTesting()
        {
            var gameManager = GameManager.Instance;
            GameManager.Instance.Init();
            
            LevelController.Instance.Init();
        }
        
    }
}