using UnityEngine;
using UnityEngine.Scripting;

public class BlockAnimationEvents : MonoBehaviour
{
    [SerializeField] private BlockController _blockController;
    private BlockStateMachine _stateMachine;

    private void Start()
    {
        _stateMachine = _blockController.StateMachine;
    }

    [Preserve]
    public void AnimationTrigger()
    {
        _stateMachine.AnimtionTriggers();
    }
}

public enum BlockAnimationTrigger
{
    End,
}
