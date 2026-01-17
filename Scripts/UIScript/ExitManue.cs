using Managers;

namespace UIScript
{
    public class ExitManue: UIButton
    {
        protected override void Click()
        {
            GameManager.Instance.ActivateSceneController.ActivateScene(0);
            GameManager.Instance.LoadManueController.ExitIcon(true);
        }
    }
}