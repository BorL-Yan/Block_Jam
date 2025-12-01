using UnityEngine;


    public interface IMovable
    {
        public void MoveOneShot(Vector3 direction);
        public void JumpAndMerge(int index);
    }
