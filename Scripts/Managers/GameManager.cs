using Lib;
using UnityEngine;

namespace Managers
{
    public class GameManager : SingletonScene<GameManager>
    {
        protected override void Awake()
        {
            base.Awake();
            
            GameSave.Init();
        }
    }
}