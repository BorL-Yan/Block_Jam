using UnityEngine;

namespace Game
{
    public class AnimationTrigger : MonoBehaviour
    {
        [SerializeField] private WinController winController;
        
        public void Trigger(TriggerType trigger)
        {
            winController.AnimationTrigger(trigger);
        }
    }

    public enum TriggerType
    {
        Jam,
        Partical,
        Text,
        End,
    }
}