using System;

namespace Game
{
    public class LevelActions
    {
        public Action OnFindPath;
        public Action OnStartLevel;
        public Action OnActivateScene;
        public Action<bool> OnEndLevel;
    }
}