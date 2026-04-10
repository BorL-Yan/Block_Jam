using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MobSelector : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;

    private CharacterInput _input;

    private void Awake()
    {
        _input = new CharacterInput();
        Init();
    }
    
    private void Init()
    {
        //_input.Character.Touch.performed += TouchScreen;
        _input.Character.Touch.started += TouchScreen;
        
        _input.Enable();
    }

    private void TouchScreen(InputAction.CallbackContext context)
    {
        // 1. Получаем позицию из системы ввода (UI.Point)
        Vector2 screenPosition = _input.Character.Point.ReadValue<Vector2>();

        // 2. Используем ручной Raycast вместо стандартной проверки
        if (IsPointerOverUIElement(screenPosition))
        {
            return;
        }
        
        CastRay(screenPosition);
    }
    

    // Универсальный метод для проверки UI в New Input System
    private bool IsPointerOverUIElement(Vector2 screenPosition)
    {
        // Создаем событие для Raycast-а
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = screenPosition;

        // Список, куда запишутся все объекты UI под точкой
        List<RaycastResult> results = new List<RaycastResult>();
        
        // Просим EventSystem проверить все UI объекты
        EventSystem.current.RaycastAll(eventData, results);

        // Если список не пуст — значит под пальцем/курсором есть UI
        return results.Count > 0;
    }
    
    
    // private void Update()
    // {
    //     HandleInput();
    // }
    //
    // private void HandleInput()
    // {
    //     // Проверка для Unity Editor (ПК) и для Телефона (первое касание)
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         // ВАЖНО: Если мы нажали на кнопку UI, то не пускаем луч в игру
    //         //if (IsPointerOverUI()) return;
    //
    //         CastRay(Input.mousePosition);
    //     }
    // }

    private void CastRay(Vector3 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        
        // Для дебага (будет видно только в Editor)
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 1f);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, _layerMask))
        {
            if (hit.collider.TryGetComponent<ISelect>(out var select))
            {
                select.Select();
            }
        }
    }

    // Универсальная проверка UI для ПК и Мобайла
    // private bool IsPointerOverUI()
    // {
    //     // Проверка для мыши или тача
    //     if (EventSystem.current.IsPointerOverGameObject()) return true;
    //
    //     // Дополнительная проверка специально для тачей (иногда IsPointerOverGameObject тупит на Андроиде с id -1)
    //     if (Input.touchCount > 0)
    //     {
    //         if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return true;
    //     }
    //
    //     return false;
    // }
}