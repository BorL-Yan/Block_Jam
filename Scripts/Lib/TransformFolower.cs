using UnityEngine;

namespace Lib
{
    public class TransformFolower : MonoBehaviour
    {
        [SerializeField] private Transform _target;

        private void Update()
        {
            this.transform.position = _target.position;
        }
        
    }
}