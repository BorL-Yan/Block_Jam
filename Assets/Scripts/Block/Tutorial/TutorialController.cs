using System;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private RectTransform _root;
    
    [SerializeField] private Cell _cell_1;
    [SerializeField] private Cell _cell_2;
    [SerializeField] private Cell _cell_3;


    [SerializeField] private RectTransform _pos_1;
    [SerializeField] private RectTransform _pos_2;
    [SerializeField] private RectTransform _pos_3;

    private void Start()
    {
        Move_1();
    }

    private void Move_1()
    {
        _root.position = _pos_1.position;  
    }
    
    private void Move_2()
    {
        _root.position = _pos_2.position;  
    }
    private void Move_3()
    {
        _root.position = _pos_3.position;  
    }

    private void Diactivate()
    {
        _root.gameObject.SetActive(false);  
        
    }


    private void OnEnable()
    {
        _cell_1.BLockActivate +=  Move_2;
        _cell_2.BLockActivate +=  Move_3;
        _cell_3.BLockActivate +=  Diactivate;
    }

    private void OnDisable()
    {
        _cell_1.BLockActivate -=  Move_2;
        _cell_2.BLockActivate -=  Move_3;
        _cell_3.BLockActivate -=  Diactivate;
    }
}
