using Managers;
using UnityEngine;

namespace UIScript
{
    public class ExitManue: UIButton
    {
        [SerializeField] private GameObject _settingsPanel;
        protected override void Click()
        {
            GameManager.Instance.ActivateSceneController.ActivateScene(0);
            GameManager.Instance.LoadManueController.ExitIcon(true);
            _settingsPanel.SetActive(false);
        }
    }
}