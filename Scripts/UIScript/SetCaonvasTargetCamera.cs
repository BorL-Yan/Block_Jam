using System;
using Unity.Content;
using UnityEngine;

namespace UIScript
{
    public class SetCaonvasTargetCamera : MonoBehaviour
    {
        private void Start()
        {
            Canvas canvas = GetComponent<Canvas>();
        
            if (canvas == null)
            {
                Debug.LogError("На этом объекте нет компонента Canvas!");
                return;
            }

            // Сначала меняем режим
            canvas.renderMode = RenderMode.ScreenSpaceCamera;

            // Пытаемся получить камеру
            Camera mainCam = Camera.main;

            if (mainCam != null)
            {
                canvas.worldCamera = mainCam;
                // Важно! Устанавливаем дистанцию, чтобы UI не "проваливался" в камеру или не улетал далеко
            }
            else
            {
                Debug.LogError("Не найдена камера с тегом 'MainCamera' или камера выключена!");
            }
        }
    }
}