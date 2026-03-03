using Game;
using TMPro;
using UnityEngine;

namespace UIScript.Icons
{
    public class Coint : MonoBehaviour
    {
        [SerializeField] private TMP_Text _coinText;

        private void Start()
        {
            _coinText.text = GameSave.GetSettings().Coint.ToString();
        }

        private void ChangeSettings()
        {
            Start();
        }
        

        private void OnEnable()
        {
            if (LevelController.Instance != null)
                LevelController.Instance.levelActions.OnStartLevel += Start;

            GameSave.OnChamgeSettings += ChangeSettings;
        }
        private void OnDisable()
        {
            if (LevelController.Instance != null)
                LevelController.Instance.levelActions.OnStartLevel -= Start;
            
            GameSave.OnChamgeSettings -= ChangeSettings;
        }
    }
}