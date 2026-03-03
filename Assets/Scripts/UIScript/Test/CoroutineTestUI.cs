using Lib;
using TMPro;
using UnityEngine;


    public class CoroutineTestUI : SingletonScene<CoroutineTestUI>
    {
        [SerializeField] private TMP_Text _text;

        private void Start()
        {
            Activate(true);
        }
        
        
        public void Activate(bool value)
        {
            _text.gameObject.SetActive(value);
        }

        public void SetText(string text)
        {
            Activate(true);
            _text.text = text;
        }
    }
