using System.Collections;
using Lib;
using UnityEngine;

namespace Game
{
    public class LevelController : SingletonScene<LevelController>
    {
        public GameObject BlockPrefab;
        
        public LevelActions levelActions { get; set; }

        private int _blockCount;

        public void AddBlock()
        {
            _blockCount++;
        }

        public void RemoveBlock()
        {
            _blockCount--;
            if (_blockCount == 0)
            {
                //levelActions.OnEndLevel?.Invoke(true);
            }
        }


        protected override void Awake()
        {
            base.Awake();
            if (BlockPrefab == null)
            {
                BlockPrefab = Resources.Load<GameObject>("Prefab/BlockBase");
            }
            
            levelActions = new LevelActions();
        }

        private void Start()
        {
            CoroutineRunner.Instance.Run(StartLevel());
        }

        private IEnumerator StartLevel()
        {
            while (true)
            {
                if (GridController.Instance.Created)
                {
                    levelActions.OnStartLevel?.Invoke();
                    break;
                }
                yield return null;
            }
        }
    }
}