using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class LoadManueController : MonoBehaviour
    {
        [SerializeField] private GameObject _loadManue;
        [SerializeField] private List<Sprite> _loadManueSprites;
        [SerializeField] private Image _image;
        
        [SerializeField] private GameObject _background;
        
        public void ActivateLoadManue(bool activate)
        {
            _loadManue.SetActive(activate);
            if(activate)
                _image.sprite = _loadManueSprites[Random.Range(0, _loadManueSprites.Count)];
            else
            {
                ExitIcon(false);
            }
        }

        public void ExitIcon(bool activate)
        {
            _background.SetActive(activate);
        }
    }
}