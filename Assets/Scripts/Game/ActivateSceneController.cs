using System.Collections;
using Lib;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class ActivateSceneController 
    {
        public void ActivateScene(int index)
        {
            CoroutineRunner.Instance.Run(Load(index));
            LevelController.Instance.NewSceneActivate();
        }
        
        private IEnumerator Load(int index)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(index);

            while (!asyncOperation.isDone)
            {
                yield return null;
            }
            
            GameManager.Instance.LoadManueController.ActivateLoadManue(false);

            if (index != 0)
            {
                LevelController.Instance.Init();
                Debug.Log("Init LevelController");
            }
            else
            {
                LevelController.Instance.Activate(false);
            }
        }
    }
}