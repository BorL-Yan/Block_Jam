
using System.Collections;
using Lib;
using UnityEngine;


    public class BlockIdleState : BlockStateBase
    {
        public BlockIdleState(BlockStateMachine stateMachine, BlockController blockController) : base(stateMachine, blockController)
        { }

        public override void Enter()
        {
            base.Enter();
            CoroutineRunner.Instance.Run(IdleDealay());
        }

        private IEnumerator IdleDealay()
        {
            yield return new WaitForSeconds(0.2f);
            MergeController.Instance.MergeToColor(_blockController.BlockColor);
        }
    }
