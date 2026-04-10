using UnityEngine;

namespace UIScript
{
    public class DiactivateButton : UIButton
    {
        [SerializeField] private GameObject _obj;
        
        protected override void Click()
        {
            _obj.gameObject.SetActive(false);
        }
    }
}