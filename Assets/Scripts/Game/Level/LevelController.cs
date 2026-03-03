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
        [SerializeField] private GameObject _canvas;

        public void AddBlock()
        {
            _blockCount++;
        }

        public void RemoveBlock()
        {
            _blockCount--;
            if (_blockCount == 0)
            {
                _canvas.SetActive(false);
                levelActions?.OnEndLevel(true);
            }
        }

        public void GameEnd()
        {
            _canvas.SetActive(false);
            levelActions?.OnEndLevel(false);
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

        public void Init()
        {
            _blockCount = 0;
            _canvas.SetActive(true);
            CoroutineRunner.Instance.Run(StartLevel());
            Activate(true);
        }

        public void NewSceneActivate()
        {
            GameEndContoller.Instance.Diactivate();
        }

        public void Activate(bool value)
        {
            gameObject.SetActive(value);
        }

        private IEnumerator StartLevel()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.1f);
                if (GridController.Instance.Created)
                {
                    levelActions.OnStartLevel?.Invoke();
                    StartLevelRegistry.Invoke();
                    break;
                }
                yield return null;
            }
        }
    }
}