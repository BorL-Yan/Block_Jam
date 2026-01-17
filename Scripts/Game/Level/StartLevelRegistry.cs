using System;
using System.Collections.Generic;

namespace Game
{
    public static class StartLevelRegistry
    {
        private static List<Action> _startLevelActions = new List<Action>();

        public static void Register(Action startLevelAction)
        {
            _startLevelActions.Add(startLevelAction);
        }

        public static void Invoke()
        {
            foreach (var item in _startLevelActions)
            {
                item?.Invoke();
            }
            _startLevelActions.Clear();
        }
    }
}