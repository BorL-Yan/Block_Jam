using UnityEngine;

namespace UIScript
{
    public class CleraHistory : UIButton
    {
        protected override void Click()
        {
            GameSettings setings = GameSave.GetSettings();

            setings.Level = 0;
        
            GameSave.SetSettings(setings);
            GameSave.Save();
        }
    }
}