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
            //
            if (GameManager.Instance.isWin)
            {
                _winObj.SetActive(true);
                GameManager.Instance.isWin = false;
            }

            // if (GameSave.GetSettings().Level == 0)
            // {
            //     GameManager.Instance.ActivateSceneController.ActivateScene(1);
            // }
        }
    }
}