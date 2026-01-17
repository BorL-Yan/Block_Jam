using System;
using Managers;
using UnityEngine;

namespace EntryPoints
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private GameObject _winObj;
        
        private void Awake()
        {
            if (!GameManager.Instance.Initialized)
            {
                GameManager.Instance.Init();
            }
            
            Debug.Log($"Game Win {GameManager.Instance.isWin}");
            if (GameManager.Instance.isWin)
            {
                _winObj.SetActive(true);
                GameManager.Instance.isWin = false;
            }
        }
    }
}