using System;
using Managers;
using UnityEngine;

namespace UIScript
{
    public class OpenCloseSettings : UIButton
    {
        [SerializeField] private OpenCloseType _type;
        [SerializeField] private GameObject _settingsObj;
        
        protected override void Click()
        {
            switch (_type)
            {
                case OpenCloseType.Open:
                {
                    _settingsObj.SetActive(true);
                    break;
                }
                case OpenCloseType.Close:
                {
                    _settingsObj.SetActive(false);
                    break;
                }
            }
        }
    }

    [Serializable]
    public enum OpenCloseType
    {
        Open,
        Close,
    }
}