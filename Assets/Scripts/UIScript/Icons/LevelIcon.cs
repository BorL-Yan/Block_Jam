using Game;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UIScript.Icons
{
    public class LevelIcon : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        private void StartLevel()
        {
            _text.text = SceneManager.GetActiveScene().buildIndex.ToString();
        }

        private void OnEnable()
        {
            LevelController.Instance.levelActions.OnStartLevel += StartLevel;
        }
        
        private void OnDisable()
        {
            LevelController.Instance.levelActions.OnStartLevel -= StartLevel;
        }

    }
}