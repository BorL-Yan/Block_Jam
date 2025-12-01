using UnityEngine;
using UnityEngine.EventSystems;

public class MobSelector : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;
    private Camera _mainCamera;

    private void Awake()
    {
        // Кэшируем камеру один раз при старте
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        // Проверка для Unity Editor (ПК) и для Телефона (первое касание)
        if (Input.GetMouseButtonDown(0))
        {
            // ВАЖНО: Если мы нажали на кнопку UI, то не пускаем луч в игру
            if (IsPointerOverUI()) return;

            CastRay(Input.mousePosition);
        }
    }

    private void CastRay(Vector3 screenPosition)
    {
        Ray ray = _mainCamera.ScreenPointToRay(screenPosition);
        
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
    private bool IsPointerOverUI()
    {
        // Проверка для мыши или тача
        if (EventSystem.current.IsPointerOverGameObject()) return true;

        // Дополнительная проверка специально для тачей (иногда IsPointerOverGameObject тупит на Андроиде с id -1)
        if (Input.touchCount > 0)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return true;
        }

        return false;
    }
}