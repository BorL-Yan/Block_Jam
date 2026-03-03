using UnityEngine;

namespace UIScript
{
    public class DiactivateButton : UIButton
    {
        [SerializeField] private GameObject _obj;
        
        protected override void Click()
        {
            Debug.Log("Button");
            _obj.gameObject.SetActive(false);
        }
    }
}