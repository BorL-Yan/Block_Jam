using System;
using TMPro;
using UnityEngine;

namespace UIScript
{
    public class CurentLevelText : MonoBehaviour
    {
        [SerializeField] private TMP_Text _levelText;

        private void Start()
        {
            SetText();
        }

        private void SetText()
        {
            int currentLevel = GameSave.GetSettings().Level;

            if (currentLevel == GameSave.MaxLevel)
            {
                _levelText.text = "MAX";
            }
            else
            {
                _levelText.text = $"Level {currentLevel + 1}";
            }
        }
        
        
        private void OnEnable()
        {
            GameSave.OnChamgeSettings += SetText;
        }

        private void OnDisable()
        {
            GameSave.OnChamgeSettings -= SetText;
        }
    }
}