using UnityEngine;


    [CreateAssetMenu(fileName = "BlockMovmentSO", menuName = "Block MovmentSO")]
    public class BlockMovmentSO : ScriptableObject
    {
        [field: SerializeField] public float Speed { get; }
    }
