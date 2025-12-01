using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UIScript
{
    public class RestartScene : MonoBehaviour
    {
        [SerializeField] private UIButton _button;

        private void Restart()
        {
            int currentindex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentindex);
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