using UnityEngine;


public interface IMovable
{
    public Vector3 Position { get; }
    public void MoveOneShot(Vector3 direction);
    public void MoveOneShot(Vector3 direction, float duration);
    public void JumpAndMerge(int index, Vector3 position);
}
