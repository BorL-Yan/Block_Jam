using UnityEngine;

namespace UIScript
{
    public class CleraHistory : UIButton
    {
        protected override void Click()
        {
            GameSettings setings = GameSave.GetSettings();

            setings = new GameSettings();
        
            GameSave.SetSettings(setings);
            GameSave.Save();
        }
    }
}