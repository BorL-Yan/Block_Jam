using System;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UIScript
{
    public class RestartScene : UIButton
    {
        protected override void Click()
        {
            GameManager.Instance.ActivateSceneController.ActivateScene(SceneManager.GetActiveScene().buildIndex);
            GameManager.Instance.LoadManueController.ExitIcon(true);
        }
    }
}