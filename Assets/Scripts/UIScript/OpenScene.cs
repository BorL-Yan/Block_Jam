using Managers;


public class OpenScene : UIButton
{
    protected override void Click()
    {
        GameSettings setings = GameSave.GetSettings();
        int level = setings.Level;
        if (level < GameSave.MaxLevel)
        {
            level++;
        }
        GameManager.Instance.ActivateSceneController.ActivateScene(level);
        GameManager.Instance.LoadManueController.ActivateLoadManue(true);
        SoundManager.Instance.PlayOneShot(SoundType.Activate);
    }
}
